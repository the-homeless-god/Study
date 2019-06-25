using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudOpros.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Hash { get; set; }
        public int GroupId { get; set; }

        public List<Rating> Ratings { get; set; }

        public Student()
        {
            Ratings = new List<Rating>();
        }

        public string Email { get; set; }
        public int Result { get; set; }
    }
}