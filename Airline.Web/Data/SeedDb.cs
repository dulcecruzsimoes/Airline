using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        
        }

        

        public async Task SeedAsync() 
        {
            
            // Se a base de dados estiver criada nenhuma acção é tomada. Caso não exista, a base de dados é criada.
            await _context.Database.EnsureCreatedAsync();

            // Verificar a existência dos seguintes roles. Se não existirem os roles, estes são criados.
            await _userHelper.CreateRoleAsyn("Admin");
            await _userHelper.CreateRoleAsyn("Customer");
            await _userHelper.CreateRoleAsyn("Employee");
            await _userHelper.CreateRoleAsyn("Anonymous");


            // Adicionar os seguintes Departamentos
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new Department { Name = "Human Resources" });
                _context.Departments.Add(new Department { Name = "Marketing" });
                _context.Departments.Add(new Department { Name = "Sales" });
               
                await _context.SaveChangesAsync();
            }

            // Adicionar as seguintes cidades a Portugal
            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });
                cities.Add(new City { Name = "Coimbra" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal"
                });

                await _context.SaveChangesAsync();
            }

            // Adicionar os estados
            if (!_context.States.Any())
            {

                _context.States.Add(new State { StateName = "Active"});
                _context.States.Add(new State { StateName = "Canceled" });
                _context.States.Add(new State { StateName = "Concluded" });

                await _context.SaveChangesAsync();
            }


            // Verificar se o user já está criado ( vai procurar o user através do email. O email será utilizado para a autenticação
            var user = await _userHelper.GetUserByEmailAsync("dcruzsimoes@gmail.com");


            var user2 = await _userHelper.GetUserByEmailAsync("gatinhamariaaugusta@gmail.com");


            var user3 = await _userHelper.GetUserByEmailAsync("rafaelsantos@portugalmail.pt");
            // Se o user não existir vou criar o user
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Dulce",
                    LastName = "Simões",
                    Email = "dcruzsimoes@gmail.com",
                    UserName = "dcruzsimoes@gmail.com",
                    PhoneNumber = "967113488",
                    TaxNumber = "226250989",
                    SocialSecurityNumber = "123456789",
                    Address = "Rua Maravilha 31 2ºD",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()

                };

                // Criar o user:
                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success) // Se algo não correu bem vou lançar uma excepção
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                // Depois do user estar criado vamos verificar se o user tem role associado e se é administrador. Se não tiver, vou associar o role administrador.
                var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

                //Gerar o Token
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);


                await _userHelper.ConfirmEmailAsync(user, token);


                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user, "Admin");
                }

                if (!_context.Destinations.Any()) // Verifica se a tabela de destinos tem algum destino, caso não tenha são inseridos destinos
                {
                    AddDestination(user); // Só está criado em memória no dataContext
                    await _context.SaveChangesAsync(); // Insere os destinos na base de dados
                }

                if (!_context.Airplaines.Any())
                {
                    AddAirplaine(user);
                    await _context.SaveChangesAsync(); // Insere os aviões na base de dados
                }

            }

            if (user2 == null)
            {
                user2 = new User
                {
                    FirstName = "Maria",
                    LastName = "Augusta",
                    Email = "gatinhamariaaugusta@gmail.com",
                    UserName = "gatinhamariaaugusta@gmail.com",
                    PhoneNumber = "66666666",
                    TaxNumber = "111111111",
                    SocialSecurityNumber = "555555",
                    Address = "Rua Maravilha 31 2ºD",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };                   

                var result2 = await _userHelper.AddUserAsync(user2, "123456");                   

                if (result2 != IdentityResult.Success) // Se algo não correu bem vou lançar uma excepção
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                // Depois do user estar criado vamos verificar se o user tem role associado e se é administrador. Se não tiver, vou associar o role administrador.
                var isInRole2 = await _userHelper.IsUserInRoleAsync(user2, "Employee");

                var token2 = await _userHelper.GenerateEmailConfirmationTokenAsync(user2);
                // Confirma logo o email
                await _userHelper.ConfirmEmailAsync(user2, token2);

                if (!isInRole2)
                {
                    await _userHelper.AddUserToRoleAsync(user2, "Employee");
                }

             
                    var departmentQuery = _context.Departments.Where(o => o.Name == "Sales");

                    Department department = departmentQuery.FirstOrDefault();

                   

                    _context.DepartmentDetails.Add(new DepartmentDetail
                    {
                        User = user2,
                        Department = department,
                        StartDate = DateTime.Today,
                    });

                    await _context.SaveChangesAsync(); // Insere na base de dados
                
            }


            if (user3 == null)
            {
                user3 = new User
                {
                    FirstName = "Rafael",
                    LastName = "Santos",
                    Email = "rafaelsantos@portugalmail.pt",
                    UserName = "rafaelsantos@portugalmail.pt",
                    PhoneNumber = "125688",
                    TaxNumber = "1234561",
                    SocialSecurityNumber = "4569842",
                    Address = "Rua Maravilha 31 2ºD",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(user3, "123456");

                if (result != IdentityResult.Success) // Se algo não correu bem vou lançar uma excepção
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                // Depois do user estar criado vamos verificar se o user tem role associado e se é administrador. Se não tiver, vou associar o role administrador.
                var isInRole = await _userHelper.IsUserInRoleAsync(user3, "Employee");

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user3);
                // Confirma logo o email
                await _userHelper.ConfirmEmailAsync(user3, token);

                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user3, "Employee");
                }

                // Adicionar o user3 ao departamento de vendas
             
                    var departmentQuery = _context.Departments.Where(o => o.Name == "Sales");

                    Department department = departmentQuery.FirstOrDefault();

              

                    _context.DepartmentDetails.Add(new DepartmentDetail
                    {
                        User = user3,
                        Department = department,
                        StartDate = DateTime.Today,
                    });

                    await _context.SaveChangesAsync(); // Insere na base de dados
                
            }


        }

        private void AddAirplaine(User user) // Verifica se a tabela de aviões tem algum avião, caso não tenha são inseridos aviões
        {
            _context.Airplaines.Add(new Airplaine
            {
                Brand = "Douglas Aircraft Company",
                Model = "DC-6",
                EconomySeats = 25,
                BusinessSeats = 25,
                User = user,

            });

            _context.Airplaines.Add(new Airplaine
            {
                Brand = "Airbus",
                Model = "Airbus A300",
                EconomySeats = 200,
                BusinessSeats = 70,
                User = user,
               
            });


            _context.Airplaines.Add(new Airplaine
            {
                Brand = "Boeing",
                Model = "Boeing 707",
                EconomySeats = 150,
                BusinessSeats = 50,
                User = user,
            });
        }

        private void AddDestination(User user)
        {
            City city = _context.Cities.Where(x => x.Name == "Lisboa").FirstOrDefault();
            Country country = _context.Countries.Where(x => x.Name == "Portugal").FirstOrDefault();

            _context.Destinations.Add(new Destination
            {
                Country = country,
                City = city,
                Airport = "Humberto Delgado",
                IATA = "LIS",
                User = user,
            }) ;


            City cityPorto = _context.Cities.Where(x => x.Name == "Porto").FirstOrDefault();
            _context.Destinations.Add(new Destination
            {
                Country = country,
                City = cityPorto,
                Airport = "Francisco Sá Carneiro",
                IATA = "OPO",
                User = user,
            });


            City cityFaro = _context.Cities.Where(x => x.Name == "Faro").FirstOrDefault();

            _context.Destinations.Add(new Destination
            {
                Country = country,
                City = cityFaro,
                Airport = "Aeroporto Internacional de Faro",
                IATA = "FAO",
                User = user,
            });
        }
    }
}
