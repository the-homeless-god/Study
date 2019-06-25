
using StudOpros.Func;
using StudOpros.Models;
using StudOpros.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace StudOpros.Controllers
{
    public class HomeController : Controller
    {
        private string pathToConfigs = HostingEnvironment.MapPath("~/Configs/");
        private Functional Functional;
        private Utills Utills;
        public HomeController()
        {
            Functional = new Functional(pathToConfigs);
            Utills = new Utills(pathToConfigs);
        }

        [HttpGet]
        public ActionResult Hash()
        {
            return View();
        }

        [HttpGet]
        [Route("Home/Student/{hash}")]
        public ActionResult Student(string hash)
        {
            try
            {
                StudentViewModel studentView = new StudentViewModel
                {
                    Student = Functional.GetStudent(hash)
                };
                if (Functional.CheckObj(studentView.Student) == false)
                {
                    return View("Unauth");
                }
                else
                {
                    studentView.Points = Functional.GetPoints();
                    studentView.Teachers = Functional.
                        GetTeachers().
                        Where(t => t.Groups.FirstOrDefault(g => g.Id == studentView.Student.GroupId) != null)
                        .ToList();

                    var updatedList = new List<Teacher>();
                    var ratings = Functional.GetRatings(); 

                    foreach (var tch in studentView.Teachers)
                    {
                        var flag = false;
                        foreach (var r in tch.Ratings)
                        {
                            if (r.StudentId == studentView.Student.Id) flag = true;
                        }
                        if (!flag)
                        {
                            updatedList.Add(new Teacher
                            {
                                Id = tch.Id,
                                Name = tch.Name,
                                ImgUrl = tch.ImgUrl
                            });
                        }
                    }
                    studentView.Teachers = updatedList;
                    return View("Student", studentView);
                }
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Route("Home/Admin/{hash}")]
        public ActionResult Admin(string hash)
        {
            try
            {
                //new Utills(pathToConfigs).UpdateAllStudentPasswords();
                if (hash == null || hash == String.Empty) return View("Error");
                if (hash.Equals("1"))
                {
                    var tchs = Functional.GetTeachers();
                    var stns = Functional.GetStudents();
                    var grps = Functional.GetGroups();
                    grps = Utills.GetStudentResult(grps);
                    return View("Admin", new Data
                    {
                        Students = stns,
                        Teachers = tchs,
                        Groups = grps,
                    });
                }
                else
                    return View("Unauth");
            }
            catch
            {
                return View("Error");
            }
        }
        
        [HttpGet]
        [Route("Home/Rating")]
        public ActionResult Rating(string hash)
        {
            try
            {
                return View("Rating", new Utills(pathToConfigs).GenerateRating());
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Unauth()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Hash(string hash)
        {
            var status = false;
            if (hash != null && hash != String.Empty)
            {
                if (hash.Equals("1")) return Admin(hash);

                status = Functional.CheckHash(hash);
            }

            return status ? Student(hash) : View("Unauth");
        }
        
    }
}