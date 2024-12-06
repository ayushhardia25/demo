using FileUploadExample.Context;
using FileUploadExample.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment Environment;
        private DBContext _dbContext;

        public HomeController(ILogger<HomeController> logger,IWebHostEnvironment _environment,DBContext dBContext)
        {
            _logger = logger;
            Environment = _environment;
            _dbContext = dBContext;

        }
        [HttpGet]
        public IActionResult CSVUpload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadCSV(IFormFile csvFile)
        {
            if (csvFile != null && csvFile.Length > 0)
            {
                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                {
                    List<Employee> employees = new List<Employee>();
                    List<Employee> existingEmployees = new List<Employee>();

                    // Skip the header line
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        var employee = new Employee
                        {
                            ID = int.Parse(values[0]),
                            Name = values[1],
                            Email = values[2],
                            Phone = values[3]
                        };

                        // Check if the employee already exists in the database
                        if (_dbContext.Employees.Any(e => e.ID == employee.ID))
                        {
                            existingEmployees.Add(employee);
                        }
                        else
                        {
                            employees.Add(employee);
                        }
                    }

                    if (existingEmployees.Any())
                    {
                        ViewBag.ExistingEmployees = existingEmployees;
                        return View("DataExists", existingEmployees);
                    }

                    if (employees.Any())
                    {
                        _dbContext.Employees.AddRange(employees);
                        _dbContext.SaveChanges();
                    }
                }
            }

            return RedirectToAction("CSVUpload");
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(List<IFormFile> postedFiles)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;

            string path = Path.Combine(this.Environment.WebRootPath, "uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                    ViewBag.Message += fileName + ",";
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
