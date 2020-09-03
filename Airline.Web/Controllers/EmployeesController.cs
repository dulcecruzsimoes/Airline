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


        [Authorize(Roles = "Admin")]
        // Lista de Todos os empregados Activos
        public IActionResult Index()
        {
              // Obtem a Lista de UsersID´s sem data de fecho de contrato 
                var DetailsList = _context.DepartmentDetails
                .Include(x => x.User)
                .Include(x => x.Department)
                .Where(x => x.CloseDate == null).ToList();

            // Passar para uma lista de EmployeeViewModel
            List<EmployeeViewModel> employeeViewModelsList = new List<EmployeeViewModel>();

            foreach (var item in DetailsList)
            {
               
                employeeViewModelsList.Add(new EmployeeViewModel
                {
                    UserId = item.User.Id,
                    FirstName = item.User.FirstName,
                    LastName = item.User.LastName,
                    Email = item.User.Email,
                    PhoneNumber = item.User.PhoneNumber,
                    Department = item.Department.Name
                });
            }
            
            return View(employeeViewModelsList);
        }


        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
                    
                };


                var result = await _userHelper.AddUserAsync(user, "123456");


                if (result != IdentityResult.Success)
                {
                    this.ModelState.AddModelError(string.Empty, "The user couldn't be created");
                    return this.View(model);
                }

                // Atribuir o role de Employee ao user
                await _userHelper.AddUserToRoleAsync(user, "Employee");


                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);


                await _userHelper.ConfirmEmailAsync(user, myToken); // Confirmar automaticamente



                // Adicionar o user ao departamento (tabela de detalhes de departamento)
                // Obter o departamento
                var department = await _context.Departments.FindAsync(model.DepartmentId);
                

                _context.DepartmentDetails.Add(new DepartmentDetail 
                {
                    User= user,
                    StartDate = DateTime.Today,
                    Department = department

                });

                await _context.SaveChangesAsync();


                // Criar um link que vai levar lá dentro uma acção. Quando o utilizador carregar neste link, 
                // vai no controlador Account executar a action "ChangePassword"
                // Este ConfirmEmail vai receber um objecto novo que terá um userid e um token.

                var myTokenReset = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Employees",
                    new { token = myTokenReset }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(user.Email, "Airline Password Reset", $"<h1>Airline Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

                return RedirectToAction("Success");

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



        [Authorize(Roles = "Admin")]
        // Editar o funcionário === Ainda não está feito
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter o user
            var user = await _userHelper.GetUserByIdAsync(id);

            user.City = await _countryRepository.GetCityAsync(user.CityId);

            
            if (user == null)
            {
                return NotFound();
            }

            // Obter o id do departamento a partir da tabela de detalhes
            var departamentName = _context.DepartmentDetails
                                .Include(x => x.Department)
                                .Where(x => x.User.Id == id)
                                .ToList()
                                .FirstOrDefault();


            if (string.IsNullOrEmpty(departamentName.Department.Name))
            {
                return NotFound();
            }


            var country = _countryRepository.GetCountryAsync(user.City);

            // modelo a passar para a view
            EmployeeViewModel model = new EmployeeViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                TaxNumber = user.TaxNumber,
                SocialSecurityNumber = user.SocialSecurityNumber,
                Department = departamentName.Department.Name,
                CountryId = country.Id,
                CityId = user.CityId,
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(country.Id),
                DepartmentId = departamentName.Department.Id,               

            };

            return View(model);
        }

          [Authorize(Roles = "Admin")]
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
                    user.Address = employeeViewModel.Address;
                    user.PhoneNumber = employeeViewModel.PhoneNumber;
                    user.TaxNumber = employeeViewModel.TaxNumber;
                    user.SocialSecurityNumber = employeeViewModel.SocialSecurityNumber;
                    user.CityId = employeeViewModel.CityId;
                    

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.Message = "Employee Updated!";
                        return View(employeeViewModel);
                    }

                    //              
                    
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


        [Authorize(Roles = "Admin")]
        // GET: Employee/ChangeDepartment/5

        public async Task<IActionResult> ChangeDepartment(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter o user
            var user = await _userHelper.GetUserByIdAsync(id);
          

            if (user == null)
            {
                return NotFound();
            }

            // Obter o id e o nome do departamento a partir da tabela de detalhes
            var oldDepartmentDetail = _context.DepartmentDetails
                                .Include(x => x.Department)
                                .Where(x => x.User.Id == id && x.CloseDate == null)
                                .ToList()
                                .FirstOrDefault();


            // modelo a passar para a view
            ChangeDptEmployeeViewModel model = new ChangeDptEmployeeViewModel()
            { 
                UserId = user.Id,
                
                FirstName = user.FirstName,
                
                LastName = user.LastName,

                OldDepartment= oldDepartmentDetail.Department.Name,

                Departments = GetDepartments(),

                BeginOldDepartment = oldDepartmentDetail.StartDate,              

                OldDepartmentDetailId = oldDepartmentDetail.Id,      

            };


            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangeDepartment(ChangeDptEmployeeViewModel model)
        {
            

            if (ModelState.IsValid)
            {
                // Nunca me fio no que vem da View, fazer sempre o check com a base de dados.
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if (user != null)
                {
                 


                    // Verificar se o Id do detalhe do departamento antigo passou e obtê-lo  
                    
                    var oldDepartmentDetail = _context.DepartmentDetails
                                  .Include(x => x.Department)
                                  .Where(x => x.User.Id == model.UserId && x.CloseDate == null)
                                  .ToList()
                                  .FirstOrDefault();

                    // Actualizar a entrada de detalhes para inserir a data de fecho do empregado no departamento                    
                    oldDepartmentDetail.CloseDate = model.EndOldDepartment;

                    var response = await _departmentRepository.UpdateDepartmentDetailsAsync(oldDepartmentDetail);



                    // Inserir o novo departamentodetail
                    // Obter o departamento primeiro

                    var newDepartment = await _departmentRepository.GetByIdAsync(model.NewDepartmentId);
              
                    //var response2 = await _departmentRepository.CreateDepartmentDetailsAsync(new DepartmentDetail {

                    //    Department = newDepartment,
                    //    User = user,
                    //    StartDate = model.BeginNewDepartment
                    //});

                    //if (response2)
                    //{
                    //    ViewBag.Message = "Update completed!";
                    //    return View(model);
                    //}

                    //              

                    ModelState.AddModelError(string.Empty, "Error in update");
                    return View(model);

                }

                else
                {
                    ModelState.AddModelError(string.Empty, "User not found!");

                    return View(model);
                }
            }

            else
            {
                return NotFound();
            }
        }

        public IActionResult DataTeste()
        {
            DateViewModel valueObject = new DateViewModel();
            valueObject.Value = new DateTime(2018, 03, 03);
            return View(valueObject);
           
        }

        [HttpPost]
        public IActionResult DataTeste(DateViewModel model) 
        {
            DateViewModel valueObject = new DateViewModel();

            //posted value is obtained from the model
            valueObject.Value = model.Value;
            return RedirectToAction(nameof(Success));
        
        }

            [Authorize(Roles = "Admin")]
        // GET: Employee/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter o user
            var user = await _userHelper.GetUserByIdAsync(id);

            user.City = await _countryRepository.GetCityAsync(user.CityId);

            if (user == null)
            {
                return NotFound();
            }

            // Obter o id do departamento a partir da tabela de detalhes
            var departamentName = _context.DepartmentDetails
                                .Include(x=>x.Department)
                                .Where(x => x.User.Id == id)
                                .ToList()
                                .FirstOrDefault();


            if (string.IsNullOrEmpty(departamentName.Department.Name))
            {
                return NotFound();
            }

            // modelo a passar para a view
            EmployeeViewModel model = new EmployeeViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                TaxNumber = user.TaxNumber,
                SocialSecurityNumber = user.SocialSecurityNumber,
                Address = user.Address,
                City = user.City.Name,
                Department = departamentName.Department.Name

            };               

            return View(model);
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
