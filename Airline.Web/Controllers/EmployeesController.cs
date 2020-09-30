using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline.Web.Data;
using Airline.Web.Data.Entities;
using Airline.Web.Data.Repository_CRUD;
using Airline.Web.Helpers;
using Airline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Airline.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {

        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        private readonly ICountryRepository _countryRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeesController(IUserHelper userHelper, DataContext context, ICountryRepository countryRepository, IMailHelper mailHelper, IDepartmentRepository departmentRepository)
        {
            _userHelper = userHelper;
            _context = context;
            _countryRepository = countryRepository;
            _mailHelper = mailHelper;
            _departmentRepository = departmentRepository;
        }


        // Lista de Todos os empregados Activos
        public async Task<IActionResult> Index()
        {
            try
            {
                // Seleccionar todos os registos da tabela de detalhes
                var userEmployees = await _userHelper.GetUsersInRoleAsync("Employee");

                List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();
                // Para cada utilizador Inserir o departamento
                foreach (var item in userEmployees)
                {
                    var departmentDetails = await _departmentRepository.GetDepartmentDetailAsync(item.Id);

                    Department department = await _departmentRepository.GetByIdAsync(departmentDetails.Department.Id);

                
                    EmployeeViewModel employeeModel = new EmployeeViewModel()
                    {
                        FirstName = item.FullName,
                        UserId = item.Id,
                        Email = item.Email,
                        Department = department.Name,
                        isActive = item.isActive,
                        PhoneNumber = item.PhoneNumber
                    };

                    employeeList.Add(employeeModel);

                }

                return View(employeeList);

            }
            catch (Exception)
            {

               return NotFound();
            }         
        }


        // Lista de Todos os empregados Activos
        public async Task<IActionResult> Details(string id) // User ID
        {
            try
            {
                // Obter o user

                User user = await _userHelper.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                // Obter o detalhe do departamento
                var departmentDetails = await _departmentRepository.GetDepartmentDetailAsync(user.Id);

                // Obter o departamento
                Department department = await _departmentRepository.GetByIdAsync(departmentDetails.Department.Id);

                City city = await _countryRepository.GetCityAsync(user.CityId);

                    EmployeeViewModel employeeModel = new EmployeeViewModel()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,                       
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        TaxNumber = user.TaxNumber,
                        SocialSecurityNumber = user.SocialSecurityNumber,
                        CityId = user.CityId,
                        City = city.Name,
                        isActive = user.isActive,
                        Department = department.Name,
                        StartDate = departmentDetails.StartDate,
                        CloseDate = departmentDetails.CloseDate,
                    };         

                return View(employeeModel);

            }
            catch (Exception)
            {

                return NotFound();
            }
        }


        // Criar um novo empregado
        public IActionResult Create()
        {
          
            var model = new RegisterNewEmployeeViewModel
            {
                Departments = GetDepartments(),
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0)
            };

            return View(model);
        }

       
        // Post do Create
        [HttpPost]
        public async Task<IActionResult> Create(RegisterNewEmployeeViewModel model)
        {

            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if (user == null)
            {
                var city = await _countryRepository.GetCityAsync(model.CityId);

                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Username,
                    UserName = model.Username,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    TaxNumber = model.TaxNumber,
                    SocialSecurityNumber = model.SocialSecurityNumber,
                    CityId = model.CityId,
                    City = city,
                    isActive = true,
                    
                };

                try
                {
                    var result = await _userHelper.AddUserAsync(user, "123456");

                }
                catch (Exception)
                {
                    model.Departments = GetDepartments();
                    model.Countries = _countryRepository.GetComboCountries();
                    model.Cities = _countryRepository.GetComboCities(model.CityId);
                    this.ModelState.AddModelError(string.Empty, "The user couldn't be created. Please, confirm data");
                    return this.View(model);
                }

                try
                {
                    // Atribuir o role de Employee ao user
                    await _userHelper.AddUserToRoleAsync(user, "Employee");
                }
                catch (Exception)
                {
                    model.Departments = GetDepartments();
                    model.Countries = _countryRepository.GetComboCountries();
                    model.Cities = _countryRepository.GetComboCities(model.CityId);
                    this.ModelState.AddModelError(string.Empty, "Error adding the employee to the role! Please contact the technical support!");

                    return this.View(model);
                }


                try
                {
                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    await _userHelper.ConfirmEmailAsync(user, myToken); // Confirmar automaticamente

                }
                catch (Exception)
                {
                    model.Departments = GetDepartments();
                    model.Countries = _countryRepository.GetComboCountries();
                    model.Cities = _countryRepository.GetComboCities(model.CityId);
                    this.ModelState.AddModelError(string.Empty, "Error on the email confirmation! Please, contact the technical suppoprt! ");

                    return this.View(model);
                }

                try
                {
                    // Adicionar o user ao departamento (tabela de detalhes de departamento)
                    // Obter o departamento
                    var department = await _context.Departments.FindAsync(model.DepartmentId);
                    _context.DepartmentDetails.Add(new DepartmentDetail
                    {
                        User = user,
                        StartDate = DateTime.Today,
                        Department = department

                    });

                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    model.Departments = GetDepartments();
                    model.Countries = _countryRepository.GetComboCountries();
                    model.Cities = _countryRepository.GetComboCities(model.CityId);
                    this.ModelState.AddModelError(string.Empty, "Error! The department details wasn't updated. Please contact support! ");

                    return this.View(model);
                }


                try
                {
                    // Criar um link que vai levar lá dentro uma acção. Quando o utilizador carregar neste link, 
                    // vai no controlador Account executar a action "ChangePassword"
                    // Este ConfirmEmail vai receber um objecto novo que terá um userid e um token.

                    var myTokenReset = await _userHelper.GeneratePasswordResetTokenAsync(user);

                    var link = this.Url.Action(
                        "ResetPassword",
                        "Employees",
                        new { token = myTokenReset }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(user.Email, "Airline Password Reset", $"<h1>Airline Password Reset</h1>" +
                    $"Welcome onboard! Please, reset your password, click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");

                    ViewBag.Message = "The employee was created with sucess! Was sent an email to the employee for the password reset!";
                    return View();

                }
                catch (Exception)
                {
                    model.Departments = GetDepartments();
                    model.Countries = _countryRepository.GetComboCountries();
                    model.Cities = _countryRepository.GetComboCities(model.CityId);
                    this.ModelState.AddModelError(string.Empty, "Error on seeding the email to the employee! Please contact support!");

                    return RedirectToAction(nameof(Index)); 
                }              
            }
            
            this.ModelState.AddModelError(string.Empty, "The user already exists");
            return View(model);
        }

        public IActionResult ResetPassword(string token) //Token gerado na action Create (Post)
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


        public IActionResult Success() 
        {

            return View();
        }
       
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                // Obter o user
                User user = await _userHelper.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                // Obter o detalhe do departamento
                var departmentDetails = await _departmentRepository.GetDepartmentDetailAsync(user.Id);

                // Obter o departamento
                Department department = await _departmentRepository.GetByIdAsync(departmentDetails.Department.Id);

                City city = await _countryRepository.GetCityAsync(user.CityId);

                Country country =  _countryRepository.GetCountryAsync(city);

                EmployeeViewModel employeeModel = new EmployeeViewModel()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    TaxNumber = user.TaxNumber,
                    SocialSecurityNumber = user.SocialSecurityNumber,
                    CityId = user.CityId,
                    City = city.Name,
                    isActive = user.isActive,
                    Department = department.Name,
                    StartDate = departmentDetails.StartDate,
                    CloseDate = departmentDetails.CloseDate,
                    Departments = GetDepartments(),
                    CountryId = country.Id,
                    Countries = _countryRepository.GetComboCountries(),
                    Cities = _countryRepository.GetComboCities(city.Id)
                };

                return View(employeeModel);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }

     
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Edit(EmployeeViewModel employeeViewModel)
          {

            if (ModelState.IsValid)
            {
                // Nunca me fio no que vem da View, fazer sempre o check com a base de dados.
                var user = await _userHelper.GetUserByEmailAsync(employeeViewModel.Email);

                if (user != null)
                {
                    user.FirstName = employeeViewModel.FirstName;
                    user.LastName = employeeViewModel.LastName;
                    user.Email = employeeViewModel.Email;
                    user.Address = employeeViewModel.Address;
                    user.PhoneNumber = employeeViewModel.PhoneNumber;
                    user.TaxNumber = employeeViewModel.TaxNumber;
                    user.SocialSecurityNumber = employeeViewModel.SocialSecurityNumber;
                    user.CityId = employeeViewModel.CityId;
                    user.isActive = employeeViewModel.isActive;

                    // Obter o detalhe do departamento
                    var departmentDetails = await _departmentRepository.GetDepartmentDetailAsync(user.Id);

                    if (departmentDetails == null)
                    {
                        return NotFound();
                    }

                    departmentDetails.StartDate = employeeViewModel.StartDate;
                    departmentDetails.CloseDate = employeeViewModel.CloseDate;

                    var response = await _userHelper.UpdateUserAsync(user); // Actualizar o User

                    var response2 = await _departmentRepository.UpdateDepartmentDetailsAsync(departmentDetails); // Actualizar o User

                    // Actualizar os detalhes

                    if (response.Succeeded && response2 == true)
                    {
                        ViewBag.Message = "Employee Updated!";
                        return View();
                    }


                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    return View(employeeViewModel);
                    
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "User not found!");

                    return View(employeeViewModel);
                }
            }
            else
            {
                return NotFound();
            }          
          }


        public async Task<JsonResult> GetCitiesAsync(int? countryId)
        {
            if (countryId == 0)
            {
                Country country1 = new Country(){ Id=0, };
                return this.Json(country1);
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId.Value);
            return this.Json(country.Cities.OrderBy(c => c.Name));
        }


        public IEnumerable<SelectListItem> GetDepartments()
        {
            var list = _context.Departments.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a department...)",
                Value = "0"
            });

            return list;
        }
    }
}
