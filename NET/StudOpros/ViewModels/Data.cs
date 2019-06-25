using StudOpros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudOpros.ViewModels
{
    public class Data
    {
        public Data()
        {
            Teachers = new List<Teacher>();
            Students = new List<Student>();
            Groups = new List<Group>();
        }
        public List<Teacher> Teachers { get; set; }
        public List<Student> Students { get; set; }
        public List<Group> Groups { get; set; }
    }
}