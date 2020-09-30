using Airline.Web.Data.Entities;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;


        //Construtor
        public DepartmentRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }


        // Adicionar determinado empregado ao Departamento
        public async Task<bool> AddEmployeeToDepartmentAsync(string userId, int deptId)
        {
            // Verificar se o departamento existe
            var department = _context.Departments.Find(deptId);

            if (department == null)
            {
                return false;
            }

            // Verificar se o utilizador existe
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            // Verificar se o role "Employee" existe, caso não exista é necessário criá-lo
            await _userHelper.CreateRoleAsyn("Employee");


            // Verificar se o utilizador já tem o role de employee e se não tiver atribuir o role
            var role = await _userHelper.IsUserInRoleAsync(user, "Employee");

            if (!role) // Não tem atribuído o role de funcionário --> Atribuir
            {

                await _userHelper.AddUserToRoleAsync(user, "Employee");
            }

            // Adicionar o registo à tabela de Detalhes de departamentos (Já se confirmou que existe o departamento, que o user existe e que lhe foi atribuido o role de employee)


            _context.DepartmentDetails.Add(new DepartmentDetail
            {
                User = user,
                Department = department,
                StartDate = DateTime.Today,
            });

            await _context.SaveChangesAsync(); // Salvar as alterações

            return true;
        }


        //Todos os empregados que já trabalharam e trabalham em determinado departamento
        public async Task<IQueryable<Department>> GetAllEmployeesFromDepartment(int id)
        {

            // Verificar se existe algum departamento com o id recebido
            var dpt = await _context.Departments.FindAsync(id);

            if (dpt == null)
            {
                return null;
            }


            // Caso o departamento exista
            return _context.Departments
               .Include(o => o.Items)
               .ThenInclude(i => i.User)
               .Where(o => o.Id == id);

        }

        // Obter o departamento Details de determinado empregado
        public async Task<DepartmentDetail> GetDepartmentDetailAsync(string idUser)
        {
            // Verificar se o user existe
            var user = await _userHelper.GetUserByIdAsync(idUser);

            if (user == null)
            {
                return null;
            }

            return await _context.DepartmentDetails
                .Include(dpt => dpt.Department)
                .Include(dpt => dpt.User)  
                .Where(dpt => dpt.User.Id == idUser).FirstOrDefaultAsync();
        }


        //Todos os empregados actuais de determinado departamento
        public async Task<IQueryable<DepartmentDetail>> GetEmployeesFromDepartment(int idDepartment)
        {
            // Verificar se existe algum departamento com o id recebido
            var dpt = await _context.Departments.FindAsync(idDepartment);

            if (dpt == null)
            {
                return null;
            }

            // Ir à tabela de detalhe dos departamentos e seleccionar todos os que têm o id do departamento e a data de fim nula
            // Caso o departamento exista
            return _context.DepartmentDetails
                     .Include(dts => dts.User)
                     .Where(dts => dts.Department == dpt && dts.CloseDate == null);
        }


        // Obter os empregados de determinado Departamento
        public async Task<Department> GetDepartmentWithEmployeesAsync(int id)
        {
            return await _context.Departments
             .Include(c => c.Items)
             .Where(c => c.Id == id)
             .FirstOrDefaultAsync();

        }


        public async Task<bool> UpdateDepartmentDetailsAsync(DepartmentDetail departmentDetail)
        {
            _context.Set<DepartmentDetail>().Update(departmentDetail);

            await _context.SaveChangesAsync();

            return true;


        }


        public async Task<bool> CreateDepartmentDetailsAsync(DepartmentDetail departmentDetail)
        {
          
            _context.Set<DepartmentDetail>().Add(departmentDetail);

            await _context.SaveChangesAsync();

            return true;

        }

    }
}
