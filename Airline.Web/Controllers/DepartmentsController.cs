using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Web.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;


        public DepartmentsController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }


        // Apresenta a lista com todos os departamentos
        public IActionResult Index()
        {

            var departments = _departmentRepository.GetAll().OrderBy(x => x.Name);

            return View(departments);
            
        }



        // Criar um novo Departamento
        public IActionResult Create()
        {
            return View();
        }


        // Post do Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentRepository.CreateAsync(department);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Already there is a department with that name");
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }
            return View(department);
        }



        // Editar o Departamento
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentRepository.GetByIdAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                await _departmentRepository.UpDateAsync(department);
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }


        // Apagar o Departamento (apenas se não tiver empregados)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Verificar se o departamento existe E com os empregados
            var department = await _departmentRepository.GetDepartmentWithEmployeesAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }        
            
            // Se o departamento tiver empregados não pode ser apagado
            if (department.Items.ToList().Count != 0)
            {
                ViewBag.Message = "The department have employees. It can't be deleted";

                return View();
            }

            await _departmentRepository.DeleteAsync(department);

            return RedirectToAction(nameof(Index));
        }

        // Detalhes do Departamento - Apresenta o departamento com os respectivos Empregados
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter todos os users do departamento
            var departmentWithUsers = await _departmentRepository.GetAllEmployeesFromDepartment(id.Value);

            if (departmentWithUsers == null)
            {
                return NotFound();
            }
            List<DepartmentDetail> DepartmentDetailList = new List<DepartmentDetail>();


            foreach (Department item in departmentWithUsers)
            {
                DepartmentDetailList = item.Items.ToList();               
            }

          

                return View(DepartmentDetailList);
    
        }


    }
}
