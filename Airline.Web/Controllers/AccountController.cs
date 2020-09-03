using Airline.Web.Data.Entities;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Airline.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IConfiguration configuration, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        public IActionResult Login()
        {
            // Vai ver se está autenticado
            if (this.User.Identity.IsAuthenticated)
            {
                //Redireccionamento para o index do controlador Home
                return this.RedirectToAction("Index", "Home");
            }


            // Se não estiver autenticado vou apresentar a view para fazer o login
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _userHelper.LogoutAsync();

            return this.RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        //Direção de retorno
                        return this.Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }

            }

            this.ModelState.AddModelError(string.Empty, "Failed to login.");
            return this.View(model);
        }

        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
           

                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                   
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,                      
                    };


                    var result = await _userHelper.AddUserAsync(user, model.Password);


                    if (result != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return this.View(model);
                    }

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    // Criar um link que vai levar lá dentro uma acção. Quando o utilizador carregar neste link, 
                    // vai no controlador Account executar a action "ConfirmEmail"(Ainda será feita)
                    // Este ConfirmEmail vai receber um objecto novo que terá um userid e um token.
                    var tokenLink = this.Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken,

                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                       $"To allow the user, " +
                       $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                    this.ViewBag.Message = "The instructions to allow your user has been sent to email.";


                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The user already exists");

            
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            // Se o user nao estiver na base de dados
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Confirmar o Email
            var result = await _userHelper.ConfirmEmailAsync(user, token);

            // Caso corra alguma coisa mal
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }


        public async Task<IActionResult> ChangeUser() 
        {
            // Agarrar no user que está logado
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var model = new ChangeUserViewModel();

            //Garantir que existe um user logado
            if (user != null)
            {
                //O mail nunca muda porque é único, nunca vou deixar mudar o mail
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model) 
        {
            if (ModelState.IsValid)
            {
                // Nunca me fio no que vem da View, fazer sempre o check com a base de dados.
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User update!";

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }

            }

            else
            {
                ModelState.AddModelError(string.Empty,"User not found!");
            }

            return View(model);
        }


        public async Task<IActionResult> ChangePasswordEmployee(string userId, string token)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            // Se o user nao estiver na base de dados
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Confirmar o Email
            var result = await _userHelper.ConfirmEmailAsync(user, token);

            // Caso corra alguma coisa mal
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return this.RedirectToAction("ChangePassword", "Account");
        }

        public IActionResult ChangePassword() 
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) 
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                       ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description); //Mostrar o primeiro erro que aparece
                    }
                }
                else
                {
                   ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }


      

        [HttpPost] //Método para proteger a API
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid) // Ver se o modelo é válido
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null) // Ver se o user existe
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded) // Ver se a password está correcta
                    {
                        var claims = new[] // Claims (perfil de utilizador)
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                        //Algoritmo de encriptação do Token(SymetricSecurityKey)
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); // É aqui que vai aceder ao JSON app, através do configuration
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15), //Quando é que o token expira (os bancos expiram muito rapidamente)
                            signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results); //Não retorna uma view, retorna um token
                        // 
                    }
                }
            }

            return this.BadRequest();
        }

        public IActionResult RecoverPassword() 
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return this.View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Email, "Airline Password Reset", $"<h1>Airline Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");
                this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return this.View();

            }

            return this.View(model);
        }


        public IActionResult ResetPassword(string token) //Token gerado na action RecoverPassword (Post)
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successfully.";
                    return RedirectToAction("Success");
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found.";
            return View(model);
        }

        public IActionResult NotAutorized() 
        {

            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
    
