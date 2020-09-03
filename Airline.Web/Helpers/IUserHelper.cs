using Airline.Web.Data.Entities;
using Airline.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Helpers
{
    public interface IUserHelper
    {
        // Método que vai servir para criar um utilizador. Este AddUserAsync é que vai chamar o método do UserManager chamado CreateAsync
        Task<IdentityResult> AddUserAsync(User user, string password);


        // Método que permite obter um user através do seu email
        Task<User> GetUserByEmailAsync(string email);

        //Método para o Login (utilização da classe do core: SignInResult)
        Task<SignInResult> LoginAsync(LoginViewModel loginViewModel);


        //Método para o LogOut
        Task LogoutAsync();

        // Fazer o Update do User
        Task<IdentityResult> UpdateUserAsync(User user);


        // Verificar a Existência do Role
        Task<bool> CheckRoleAsyn(string roleName);


        //Fazer o Update da password
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        //Método para confirmar se a password está correcta antes de gerar o Token
        Task<SignInResult> ValidatePasswordAsync(User user, string password);
        
        
        // Saber se o user tem o role associado
        Task <bool> IsUserInRoleAsync(User user, string roleName);

        // Mete o user dentro do role
        Task AddUserToRoleAsync(User user, string roleName);


        //Método que gera o email par confirmação com o respectivo Tolken
        Task<string> GenerateEmailConfirmationTokenAsync(User user);


        // Método que verifica se o email foi confirmado e se o Token é válido
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        // Método que pesquisa o user por email
        Task<User> GetUserByIdAsync(string userId);

        //Método para recuperar password
        Task <string> GeneratePasswordResetTokenAsync(User user);

        //Método para realizar o reset à password
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        // Método para obter todos os roles existentes
        IQueryable<IdentityRole> GetAll();

        // Método para obter determinado role a partir do seu id
        Task<IdentityRole> GetRoleAsync(string roleId);


        // Método para actualizar determinado role
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);


        // Método para criar o role
        Task<IdentityResult> CreateRoleAsyn(string roleName);

        // Método para apagar o role
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);

        // Método para obter o role a partir do nome
        Task<IdentityRole> GetRoleByNameAsync(string name);
    }
}
