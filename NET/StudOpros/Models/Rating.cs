using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudOpros.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int Mark { get; set; }
        
        public int PointId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }

    }
}