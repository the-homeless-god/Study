using StudOpros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudOpros.ViewModels
{
    public class StudentViewModel
    {
        public List<Teacher> Teachers { get; set; }
        public Student Student { get; set; }

        public StudentViewModel()
        {
            Teachers = new List<Teacher>();
        }

        public List<Point> Points { get; set; }

        public Dictionary<int, string> pointPairs = new Dictionary<int, string>
        {
          { 1, "Очень плохо / Полностью не согласен" },
          { 2, "Скорее плохо, чем хорошо/ В большей степени не согласен" },
          { 3, "Иногда хорошо, иногда плохо / Отчасти согласен, отчасти нет" },
          { 4, "Скорее хорошо, чем плохо / В большей степени согласен" },
          { 5, "Отлично / Полностью согласен" },
        };
    }
}