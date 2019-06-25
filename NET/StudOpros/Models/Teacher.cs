using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudOpros.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }

        public string GroupsString { get; set; }

        public List<Group> Groups { get; set; }
        public List<Rating> Ratings { get; set; }
        
        public Teacher()
        {
            Ratings = new List<Rating>();
            Groups = new List<Group>();
        }

        public double AverageScoreByPersonal { get; set; }
        public double AverageScoreByTech { get; set; }
        public double AverageScoreByProf { get; set; }
        public double SummaryAverageScore { get; set; }
        
    }
}