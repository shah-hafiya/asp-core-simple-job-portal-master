using JobPortal.Models;
using JobPortal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using JobPortal.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;

namespace JobPortal.Controllers
{
    [Route("accounts/")]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("employer/register")]
        public IActionResult EmployerRegister()
        {
            return View();
        }

        [HttpPost]
        [Route("employer/register")]
        public async Task<IActionResult> EmployerRegister(
            [Bind("FirstName", "LastName", "Email", "Password", "ConfirmPassword","UserName","Gender")]
            EmployerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                //IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole("Employee"));

                if (result.Succeeded)
                {
                    bool checkRole = await _roleManager.RoleExistsAsync("Employer");
                    if (!checkRole)
                    {
                        var role = new IdentityRole();
                        role.Name = "Employer";
                        await _roleManager.CreateAsync(role);

                        await _userManager.AddToRoleAsync(user, "Employer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Employer");
                    }

                    //await _signManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View();
        }

        [HttpGet]
        [Route("employee/register")]
        public IActionResult EmployeeRegister()
        {
            return View();
        }

        [HttpPost]
        [Route("employee/register")]
        public async Task<IActionResult> EmployeeRegister(
            [Bind("FirstName", "LastName", "Email", "Password", "ConfirmPassword","Gender")]
            EmployeeRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.FirstName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Gender = model.Gender
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                //IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole("Employee"));

                if (result.Succeeded)
                {
                    bool checkRole = await _roleManager.RoleExistsAsync("Employee");
                    if (!checkRole)
                    {
                        var role = new IdentityRole();
                        role.Name = "Employee";
                        await _roleManager.CreateAsync(role);

                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                    else
                    {

                        await _userManager.AddToRoleAsync(user, "Employee");
                    }

                    //await _signManager.SignInAsync(user, false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string returnUrl = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region old comment
                /*
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                var userName = user.UserName;

                var result = await _signManager.PasswordSignInAsync(userName,
                    model.Password, model.RememberMe, lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                */

                #endregion


                using (var httpCllient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }))
                {
                    httpCllient.DefaultRequestHeaders.Accept.Clear();
                    httpCllient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpCllient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                    var data = new Dictionary<string, string>
                    {
                        { "email", model.Email},
                        { "password", model.Password }
                    };

                    var httpContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    //POST the object to the specified URI 
                    var response = await httpCllient.PostAsync("https://caswebapi13082020.azurewebsites.net/api/auth/login", httpContent);

                    //Read back the answer from server
                    var responseString = await response.Content.ReadAsStringAsync();

                    // deserialize using json
                    OperationResult<AccessToken> result = JsonConvert.DeserializeObject<OperationResult<AccessToken>>(responseString);
                    if (result.Succeeded)
                    {

                        HttpContext.Session.SetString("token", result.Entity.Token);
                        //HttpContext.Items.Add("token", result.Entity.Token);
                        httpCllient.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Entity.Token}");

                        //var httpContent1 = new StringContent(JsonConvert.SerializeObject(data1), Encoding.UTF8, "application/json");
                        var response1 = await httpCllient.GetAsync($"https://caswebapi13082020.azurewebsites.net/api/user/{model.Email}");
                        var responseString1 = await response1.Content.ReadAsStringAsync();

                        OperationResult<UserWithRoleDto> result1 = JsonConvert.DeserializeObject<OperationResult<UserWithRoleDto>>(responseString1);

                        //HttpContext.Session.SetString("userwithrole", result1.Entity.Roles.Select(x => x.Name).ToString());



                        //HttpContext.Session.Set("userwithroles", Encoding.UTF8.GetBytes((result1.Entity));
                        HttpContext.Session.SetObjectAsJson("userwithroles", result1.Entity);
                        //IList<RoleDto> rolesdto = result1.Entity.Roles;

                        //HttpContext.Session.SetString("rolename", result1.Entity);



                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt");
                        return View(model);
                    }
                }



            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("employee/edit-profile")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            return View(user);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("employee/update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] User model)
        {
            //            _logger.LogError(model.Gender.ToString());
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return RedirectToActionPermanent("EditProfile", "Account");
        }
    }
}