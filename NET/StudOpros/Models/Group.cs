using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace StudOpros.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StudentId { get; set; }

        public List<Student> Students { get; set; }

    }
}