﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JobPortal.Models;
using JobPortal.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobPortal.Controllers
{
    public class JobController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;

        private readonly QueueClient _queueClient;
        private readonly IConfiguration _configuration;
        private const string QUEUE_NAME = "simplequeue";

        public JobController(ApplicationDbContext context, UserManager<User> userManager, ILogger<JobController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }

        [Route("jobs")]
        public IActionResult Index()
        {
            var jobs = _context.Jobs.ToList();

            return View(jobs);
        }

        [Route("jobs/create")]
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("jobs/save")]
        [Authorize(Roles = "Employer")]
        [HttpPost]
        public async Task<IActionResult> Save(Job model)
        {
            #region Comment
            /*
            if (ModelState.IsValid)
            {
                TempData["type"] = "success";
                TempData["message"] = "Job posted successfully";
                //_logger.LogInformation(model.ToString());
                var user = await _userManager.GetUserAsync(HttpContext.User);
                model.User = user;
                _context.Jobs.Add(model);

                await _context.SaveChangesAsync();

                return RedirectToActionPermanent("Index", "Home");
            }
            */
            #endregion

            if(ModelState.IsValid)
            {
                TempData["type"] = "success";
                TempData["message"] = "Jobs posted successfully";
                using (var httpCllient = new HttpClient())
                {
                    httpCllient.DefaultRequestHeaders.Accept.Clear();
                    httpCllient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpCllient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                    var data = new Dictionary<string, string>
                {
                    { "title", model.Title},
                    { "description", model.Description },
                    { "location", model.Location },
                    { "type", model.Type},
                    { "createdat", model.CreatedAt.ToString() },
                    { "lastdate", model.LastDate.ToString()},
                    { "companyname", model.CompanyName },
                    { "companydescription", model.CompanyDescription},
                    { "website", model.Website }



                };

                var httpContent = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

                    // POST the object to the specified URI
                    var response = await httpCllient.PostAsync("https://caswebapi13082020.azurewebsites.net/api/job/update", httpContent);

                    //Read back the answer from server
                    var responseString = await response.Content.ReadAsStringAsync();

                    // deserialize using json
                    OperationResult<AccessToken> result = JsonConvert.DeserializeObject<OperationResult<AccessToken>>(responseString);
                    if (result.Succeeded)
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt");
                        return View(model);
                    }




                }





            }

            return View("Create", model);
        }

        [HttpPost]
        //[Authorize(Roles = "Employee")]
        public async Task<IActionResult> Apply(int id)
        {
            var job = _context.Jobs.SingleOrDefault(x => x.Id == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(user == null)
            {
                return RedirectToActionPermanent("Login", "Account");
            }
            else
            {
                if(!User.IsInRole("Employee"))
                {
                    TempData["message"] = "You can't do this action";
                    return RedirectToActionPermanent("JobDetails", "Home", new { id });
                }
            }
            var apply = new Applicant
            {
                User = user,
                Job = job,
                CreatedAt = DateTime.Now
            };

            _context.Applicants.Add(apply);

            await _context.SaveChangesAsync();

            return RedirectToActionPermanent("JobDetails", "Home", new { id });
        }

        [Route("mark-as-filled/{id}")]
        //[Authorize(Roles = "Employer")]
        public async Task<IActionResult> MarkAsFilled(int id)
        {
            var job = _context.Jobs.SingleOrDefault(x => x.Id == id);
            job.isFilled = true;
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();

            return RedirectToActionPermanent("Index", "Dashboard");
        }
        
        [HttpPost]
        [Authorize(Roles = "Employer")]
        [Route("employer/jobs/{id}/destroy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Destroy(int id)
        {
            var job = _context.Jobs.SingleOrDefault(x => x.Id == id);
            if(job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            TempData["type"] = "success";
            TempData["message"] = "Job deleted successfully";

            return RedirectToActionPermanent("Index", "Dashboard");
        }
    }
}