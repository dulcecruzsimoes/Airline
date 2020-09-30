using Airline.Web.Data;
using Airline.Web.Data.Entities;
using Airline.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        // Construtor
        public UserHelper(UserManager <User> userManager , SignInManager<User> signInManager, RoleManager<IdentityRole>roleManager, DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        // Método que permite obter todos os Roles
        public IQueryable<IdentityRole> GetAll() 
        {
            return _roleManager.Roles;
        }


        // Método que permite obter determinado role a partir do seu id
        public async Task<IdentityRole> GetRoleAsync(string roleId) 
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        //Método que permite fazer o update do role
        public async Task <IdentityResult> UpdateRoleAsync(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);  
        }

        //Método para apagar o Role
        public async Task <IdentityResult> DeleteRoleAsync(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }

        // Método para obter o role a partir do nome
        public async Task<IdentityRole> GetRoleByNameAsync (string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }    
        
        // Método para validar se o role está ou não vazio de users
        public async Task<bool> RoleEmptyAsync (string roleName) 
        {

            var usersList = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersList== null)
            {
                return true;
            }

            return false;

        }

        //Adicionar User
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
       
        }


        // Atribuir ao user um role
        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            return await _userManager.AddToRoleAsync(user, roleName);
        }



        // Mudar a password do utilizador
        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }


        // Confirmar a existência de determinado role
        public async Task<bool> CheckRoleAsyn(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
            
        }

        // Criar Role
        public async Task<IdentityResult> CreateRoleAsyn(string roleName)
        {
           
                return await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            
        }



        // Confirmação de email
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }


        
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }


        //Vai gerar o token
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            
            
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }



        // Oboter user através do email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            
            return await _userManager.FindByEmailAsync(email);
        }


        // Obter o user a partir do id
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }


        // Verificar se determinado user pertence a um determinado role
        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");

        }



        // Resultado de Login
        public async Task<SignInResult> LoginAsync(LoginViewModel loginViewModel)
        {
            return await _signInManager.PasswordSignInAsync(
                loginViewModel.Username,
                loginViewModel.Password,
                loginViewModel.RememberMe,
                false);

            //O 4 parÂmetro: false: se bloqueia ao fim de x tentativas --> Não vou colocar bloqueio nenhum.
            
        }


        // Resultado de LogOut
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // Fazer o reset à password
        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        //Actualizar user
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }


        //Método que valida a password
        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }



        public List<User> GetAllUsers() 
        {
            List<User> Users = new List<User>();

            Users = _userManager.Users.ToList();


            return Users;
        
        }

        //====================Customer=============

        public async Task<IList<User>> GetUsersInRoleAsync(string role) 
        {
           
            return await _userManager.GetUsersInRoleAsync(role);
        
        }

    }
}
