
using StudOpros.Func;
using StudOpros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace StudOpros.Controllers
{
    [RoutePrefix("api/FullApi")]
    public class FullApiController : ApiController
    {
        private string pathToConfigs = HostingEnvironment.MapPath("~/Configs/");
        private Functional Functional;
        private Utills Utills;
        public FullApiController()
        {
            Functional = new Functional(pathToConfigs);
            Utills = new Utills(pathToConfigs);
        }
        
        [HttpPost]
        [Route("createStudent")]
        public void CreateStudent([FromBody]Student student)
        {
            if (Functional.CheckObj(student))
            {
                student.Hash = Utills.GenerateHash();
                Functional.SaveStudent(student);
            }
        }

        [HttpPost]
        [Route("createPoint")]
        public void CreatePoint([FromBody] Point point)
        {
            if (Functional.CheckObj(point))
            {
                Functional.SavePoint(point);
            }
        }

        [HttpPost]
        [Route("createRating")]
        public void CreateRating([FromBody] ICollection<Rating> ratings)
        {
            var points = Functional.GetPoints();
            if (ratings.Count() == points.Count())
            {
                foreach (var rating in ratings)
                {
                    if (Functional.CheckObj(rating))
                    {
                        Functional.SaveRating(rating);
                    }
                }
            }
        }



        [HttpPost]
        [Route("createTeacher")]
        public void CreateTeacher([FromBody]Teacher teacher)
        {
            if (Functional.CheckObj(teacher))
            {
                Functional.SaveTeacher(teacher);
            }
        }

        [HttpPost]
        [Route("createGroup")]
        public void CreateGroup([FromBody]Group group)
        {
            if (Functional.CheckObj(group))
            {
                Functional.SaveGroup(group);
            }
        }

        [HttpGet]
        [Route("getGroups")]
        public List<Group> GetGroups()
        {
            var groups = Functional.GetGroups();
            groups = Utills.GetStudentResult(groups);
            return groups;
        }

        [HttpGet]
        [Route("getTeachers")]
        public List<Teacher> GetTeachers()
        {
            var teachers = Functional.GetTeachers(); 
            return teachers;
        }

        [HttpGet]
        [Route("getPoints")]
        public List<Point> GetPoints()
        {
            var points = Functional.GetPoints();
            return points;
        }
 
    }
}