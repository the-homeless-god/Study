using StudOpros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace StudOpros.Func
{
    public class Utills
    {
        private Functional Functional;
        private string server;
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();

        public Utills(string server)
        {
            this.server = server;
            Functional = new Functional(server);
        }

        public string GenerateHash()
        {
            var pass = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            var flag = true;
            var students = Functional.GetStudents();
            while (flag)
            {
                if (CheckDuplicate(students.ToList(), pass)) pass = new string(Enumerable.Repeat(chars, 6)
                          .Select(s => s[random.Next(s.Length)]).ToArray());
                else flag = false;
            }
            return pass;

        }

        public List<Teacher> GenerateRating()
        {
            var teachers = Functional.GetTeachers();
            var ratings = new List<Teacher>();
            var points = Functional.GetPoints();
            var rates = Functional.GetRatings();

            foreach (var teacher in teachers)
            {
                Dictionary<int, List<Rating>> sortedRates =
                    GetSortedRatesByTeacherId(
                       rates.ToList(),
                        server,
                       points.ToList(),
                        teacher.Id);
                var tch = new Teacher
                {
                    Id = teacher.Id,
                    Name = teacher.Name
                };

                var firstGroup = 0;
                var secondGroup = 0;
                var thirdGroup = 0;
                foreach (var rate in teacher.Ratings)
                {
                    var blockId = points.FirstOrDefault(p => p.Id == rate.PointId).BlockId;
                    switch (blockId)
                    {
                        case 1:
                            firstGroup += rate.Mark;
                            break;
                        case 2:
                            secondGroup += rate.Mark;
                            break;
                        case 3:
                            thirdGroup += rate.Mark;
                            break;
                    }

                }
                if (sortedRates[1].Count() == 0) firstGroup = 0;
                else firstGroup /= sortedRates[1].Count();
                if (sortedRates[2].Count() == 0) secondGroup = 0;
                else secondGroup /= sortedRates[2].Count();
                if (sortedRates[2].Count() == 0) thirdGroup = 0;
                else thirdGroup /= sortedRates[3].Count();

                teacher.AverageScoreByProf = firstGroup * 0.5;
                teacher.AverageScoreByTech = secondGroup * 0.2;
                teacher.AverageScoreByPersonal = thirdGroup * 0.3;

                teacher.SummaryAverageScore =
                    teacher.AverageScoreByProf + teacher.AverageScoreByTech + teacher.AverageScoreByPersonal;
            }

            return teachers.ToList();
        }

        private Dictionary<int, List<Rating>> GetSortedRatesByTeacherId(List<Rating> rates, string server, List<Point> points, int teacherId)
        {
            Dictionary<int, List<Rating>> result = new Dictionary<int, List<Rating>>();
            rates = rates.Where(r => r.TeacherId == teacherId).ToList();
            result[1] = new List<Rating>();
            result[2] = new List<Rating>();
            result[3] = new List<Rating>();

            foreach (var rate in rates)
            {
                var rat = points.FirstOrDefault(p => p.Id == rate.PointId);

                if (result[rat.BlockId].FirstOrDefault(r => r.Id == rate.Id) == null)
                {
                    result[rat.BlockId].Add(rate);
                }
            }

            return result;
        }

        private bool CheckDuplicate(List<Student> students, string hash)
        {
            bool flag = false;
            foreach (var student in students)
            {
                if (student.Hash.Equals(hash))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public List<Group> GetStudentResult(List<Group> groups)
        {
            var teacherCount = Functional.GetTeachers();
            var ratings = Functional.GetRatings();

            Dictionary<int, HashSet<int>> result = new Dictionary<int, HashSet<int>>();
            var studentIds = new HashSet<int>();


            foreach (var rating in ratings)
            {
                studentIds.Add(rating.StudentId);
            }

            foreach (var studentId in studentIds)
            {

                var teacherIds = new HashSet<int>();
                foreach (var rating in ratings)
                {
                    if (rating.StudentId == studentId)
                    {
                        teacherIds.Add(rating.TeacherId);
                    }
                }

                if (teacherIds.Count() != 0)
                {
                    result[studentId] = teacherIds;
                }
            }


            for (int i = 0; i < groups.Count(); i++)
            {
                var groupTeachersCount = 0;

                foreach (var teacher in teacherCount)
                {
                    if (teacher.Groups.FirstOrDefault(g => g.Id == groups[i].Id) != null)
                        groupTeachersCount += 1;
                }

                for (int j = 0; j < groups[i].Students.Count(); j++)
                {
                    if (result.ContainsKey(groups[i].Students[j].Id))
                    {
                        int countTeachersWasMarked = result[groups[i].Students[j].Id].Count();
                        int decr = (int) (((double) countTeachersWasMarked / groupTeachersCount) * 100);
                        int res = decr;
                        groups[i].Students[j].Result =
                            res;
                    }
                }
            }


            return groups;
        }
    }
}