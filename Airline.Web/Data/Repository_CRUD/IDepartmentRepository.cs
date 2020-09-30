using Airline.Web.Data.Entities;
using Airline.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Repository_CRUD
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        // Todos os departamentos onde determinado empregado com detalhes (user) já trabalhou
        Task<DepartmentDetail> GetDepartmentDetailAsync(string idUser);


        //Todos os empregados actuais de determinado departamento
        Task<IQueryable<DepartmentDetail>> GetEmployeesFromDepartment(int idDepartment);


        //Todos os empregados que já trabalharam e trabalham em determinado departamento
        Task<IQueryable<Department>> GetAllEmployeesFromDepartment(int idDepartment);


        // Adicionar determinado empregado ao Departamento
        Task<bool> AddEmployeeToDepartmentAsync(string userId, int deptId);

        // Obter os empregados de determinado Departamento
        Task<Department> GetDepartmentWithEmployeesAsync(int id);


        // Mudança de departamento
        Task <bool> UpdateDepartmentDetailsAsync(DepartmentDetail departmentDetail);


        Task<bool> CreateDepartmentDetailsAsync(DepartmentDetail departmentDetail);
    }
}
