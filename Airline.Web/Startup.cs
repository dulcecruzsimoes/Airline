using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airline.Web.Data;
using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Airline.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuração do serviço do User (utilização dos roles por defeito
            // Modelo a passar para o User é o meu (estendido do IdentityUser) e vou passar o modelo do role do Core por defeito
            // Se não tivesse nenhum modelo para o user passaria: services.AddIdentity<IdentityUser, IdentityRole>
            // Quando estiver em produção teremos que vir alterar isto tudo para as passwords não terem as validações todas
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true; // Só vai deixar entrar na aplicação depois de ter confirmado o email
                cfg.User.RequireUniqueEmail = true; // Os emails terão que ser únicos
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();

            //Adicionar o DbContext
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));

            });


            // Encaminhamento para o Controlador Account e executar a Action: "NotAuthorized" quando houver uma tentativa para aceder sem autorização (role sem permissões para determinada área)
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

            // Adicionar o serviço para gerar Tokens:
            services.AddAuthentication()
            .AddCookie()
            .AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Configuration["Tokens:Issuer"],
                    ValidAudience = this.Configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(this.Configuration["Tokens:key"]))
                };
            });


            // Quando alguém precisar do SeedDb instancia. É utilizado e depois rua...
            // Só é instanciado uma vez e por isso não precisa do interface
            services.AddTransient<SeedDb>();


            // Instancia e deita fora, instancia e deita fora... (dependecy injection obriga a ter o interface e a classe)
            // Quando é compilado precisa de ter lá o interface (precisa de compilar alguma coisa), e vê quais as classes que implementam o interface
            services.AddScoped<IDestinationRepository,DestinationRepository>();

            services.AddScoped<IAirplaneRepository, AirplaneRepository>();

            services.AddScoped<ICountryRepository, CountryRepository>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            services.AddScoped<IUserHelper, UserHelper>();

            services.AddScoped<IImageHelper, ImageHelper>();

            services.AddScoped<IConverterHelper, ConverterHelper>();

            services.AddScoped<IMailHelper, MailHelper>();

            services.AddScoped<IFlightRepository, FlightRepository>();

            services.AddScoped<ITicketRepository, TicketRepository>();


            

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Para todas as páginas: Lidar com o erro 404 qd as actions não existem
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCookiePolicy();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
