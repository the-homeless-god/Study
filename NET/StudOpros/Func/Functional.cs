using StudOpros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace StudOpros.Func
{
    public class Functional
    {
        #region paths to configs
        private string pathGroup = "";
        private string pathTeacher = "";
        private string pathStudent = "";
     //   private string pathStudent2 = "";
        private string pathRating = "";
        private string pathPoint = "";
        #endregion

        #region Constructor
        public Functional(string server)
        {
            pathGroup = server + "Group.config";
            pathTeacher = server + "Teacher.config";
            pathStudent = server + "Student.config";
            pathRating = server + "Rating.config";
            pathPoint = server + "Point.config";
        }
        #endregion

        #region Operations

        #region Groups
        public List<Group> GetGroups()
        {
            var items = new List<Group>();
            XmlNode nodes = GetNodes(pathGroup, "Group");

            foreach (XmlNode xnn in nodes.ChildNodes)
            {
                var stud = GetStudents();
                stud = stud.Where(item => item.GroupId == Convert.ToInt32(xnn.Attributes[0].Value)).ToList();
                items.Add(new Group
                {
                    Id = Convert.ToInt32(xnn.Attributes[0].Value),
                    Name = xnn.Attributes[1].Value,
                    StudentId = Convert.ToInt32(xnn.Attributes[2].Value),
                    Students = stud
                });
            }


            return items;
        }

        public void SaveGroup(Group group)
        {
            if (CheckObj(group))
            {
                XElement root = new XElement("Group");
                XDocument doc = new XDocument(root);
                var list = GetGroups();
                group.Id = list.Count() + 1;
                group.Name = group.Name.Trim();
                list.Add(group);

                foreach (var item in list)
                {
                    XElement element = new XElement("Group");

                    element.SetAttributeValue("Id", item.Id);
                    element.SetAttributeValue("Name", item.Name);
                    element.SetAttributeValue("StudentId", item.StudentId);

                    doc.Root.Add(element);
                }

                doc.Save(pathGroup);
            }
        }

        #endregion

        #region Teachers
        public List<Teacher> GetTeachers()
        {
            var items = new List<Teacher>();
            XmlNode nodes = GetNodes(pathTeacher, "Teacher");

            foreach (XmlNode xnn in nodes.ChildNodes)
            {
                Teacher teacher = new Teacher
                {
                    Id = Convert.ToInt32(xnn.Attributes[0].Value),
                    Name = xnn.Attributes[1].Value,
                    ImgUrl = xnn.Attributes[2].Value,
                    Ratings = GetRatings().Where(r => r.TeacherId == Convert.ToInt32(xnn.Attributes[0].Value)).ToList(),
                    GroupsString = xnn.Attributes[3].Value,
                };
                teacher = DenormalizeTeacherGroups(teacher);

                items.Add(teacher);
            }


            return items;
        }
        public void SaveTeacher(Teacher teacher)
        {
            if (CheckObj(teacher))
            {
                XElement root = new XElement("Teacher");
                XDocument doc = new XDocument(root);
                var list = GetTeachers();
                teacher.Id = list.Count() + 1;
                list.Add(teacher);

                foreach (var item in list)
                {
                    Teacher tch = new Teacher();
                    if (item.GroupsString == null || item.GroupsString == String.Empty || item.GroupsString == "")
                    {
                        tch = NormalizeTeacherGroups(item);
                    }

                    XElement element = new XElement("Teacher");
                    element.SetAttributeValue("Id", tch.Id);
                    element.SetAttributeValue("Name", tch.Name);
                    element.SetAttributeValue("ImgUrl", tch.ImgUrl);
                    element.SetAttributeValue("GroupsString", tch.GroupsString);
                    doc.Root.Add(element);
                }
                doc.Save(pathTeacher);
            }
        }

        public Teacher NormalizeTeacherGroups(Teacher teacher)
        {
            string groupString = "";
            if (teacher.Groups.Count() == 1) groupString = teacher.Groups.ToList()[0].Id.ToString();
            else
            {
                for (int i = 0; i < teacher.Groups.Count(); i++)
                {
                    if (i == 0 && teacher.Groups.Count() == 1) groupString += teacher.Groups.ToList()[i].Id;
                    if (i == teacher.Groups.Count() - 1) groupString += teacher.Groups.ToList()[i].Id;
                    else groupString += teacher.Groups.ToList()[i].Id + ",";
                }
            }
            teacher.GroupsString = groupString;
            teacher.Groups = null;
            return teacher;
        }
        public Teacher DenormalizeTeacherGroups(Teacher teacher)
        {
            var list = GetGroups();
            var listGroups = teacher.GroupsString.Split(',');

            foreach (var group in listGroups)
            {
                var item = list.FirstOrDefault(g => g.Id == Convert.ToInt32(group));
                if (item != null) teacher.Groups.Add(item);
            }
            teacher.GroupsString = null;

            return teacher;
        }
        
        #endregion

        #region Rating
        public List<Rating> GetRatings()
        {
            var items = new List<Rating>();
            XmlNode nodes = GetNodes(pathRating, "Rating");

            foreach (XmlNode xnn in nodes.ChildNodes)
            {
                items.Add(new Rating
                {
                    Id = Convert.ToInt32(xnn.Attributes[0].Value),
                    StudentId = Convert.ToInt32(xnn.Attributes[1].Value),
                    Mark = Convert.ToInt32(xnn.Attributes[2].Value),
                    PointId = Convert.ToInt32(xnn.Attributes[3].Value),
                    TeacherId = Convert.ToInt32(xnn.Attributes[4].Value),
                });
            }


            return items;
        }

        public void SaveRating(Rating rating)
        {
            if (CheckObj(rating))
            {
                XElement root = new XElement("Rating");
                XDocument doc = new XDocument(root);
                var list = GetRatings();

                if (list.FirstOrDefault(s =>
                     s.PointId == rating.PointId &&
                     s.StudentId == rating.StudentId &&
                     s.TeacherId == rating.TeacherId) == null)
                {
                    rating.Id = list.Count() + 1;
                    list.Add(rating);

                    foreach (var item in list)
                    {
                        XElement element = new XElement("Rating");

                        element.SetAttributeValue("Id", item.Id);
                        element.SetAttributeValue("StudentId", item.StudentId);
                        element.SetAttributeValue("Mark", item.Mark);
                        element.SetAttributeValue("PointId", item.PointId);
                        element.SetAttributeValue("TeacherId", item.TeacherId);

                        doc.Root.Add(element);
                    }
                    doc.Save(pathRating);
                }
            }
        }

        #endregion

        #region Point
        public void SavePoint(Point point)
        {

            if (CheckObj(point))
            {
                XElement root = new XElement("Point");
                XDocument doc = new XDocument(root);

                var list = GetPoints();
                point.Id = list.Count() + 1;
                point.Name = point.Name.Trim();
                list.Add(point);

                foreach (var item in list)
                {
                    XElement element = new XElement("Point");

                    element.SetAttributeValue("Id", item.Id);
                    element.SetAttributeValue("Name", item.Name);
                    element.SetAttributeValue("BlockId", item.BlockId);
                    doc.Root.Add(element);
                }
                doc.Save(pathPoint);
            }
        }


        public List<Point> GetPoints()
        {
            var items = new List<Point>();
            XmlNode nodes = GetNodes(pathPoint, "Point");

            foreach (XmlNode xnn in nodes.ChildNodes)
            {
                items.Add(new Point
                {
                    Id = Convert.ToInt32(xnn.Attributes[0].Value),
                    Name = xnn.Attributes[1].Value,
                    BlockId = Convert.ToInt32(xnn.Attributes[2].Value)
                });
            }


            return items;
        }
        #endregion

        #region Student
        public List<Student> GetStudents()
        {
            var items = new List<Student>();
            XmlNode nodes = GetNodes(pathStudent, "Student");

            foreach (XmlNode xnn in nodes.ChildNodes)
            {
                items.Add(new Student
                {
                    Id = Convert.ToInt32(xnn.Attributes[0].Value),
                    Name = xnn.Attributes[1].Value,
                    GroupId = Convert.ToInt32(xnn.Attributes[2].Value),
                    Hash = xnn.Attributes[3].Value,
                });
            }

            return items;
        }
        
        public void SaveStudent(Student student)
        {

            if (CheckObj(student))
            {
                XElement root = new XElement("Student");
                XDocument doc = new XDocument(root);

                var list = GetStudents();
                student.Id = list.Count() + 1;
                student.Hash = student.Hash.Trim();
                student.Name = student.Name.Trim();
                list.Add(student);

                foreach (var item in list)
                {
                    XElement element = new XElement("Student");

                    element.SetAttributeValue("Id", item.Id);
                    element.SetAttributeValue("Name", item.Name);
                    element.SetAttributeValue("GroupId", item.GroupId);
                    element.SetAttributeValue("Hash", item.Hash);

                    doc.Root.Add(element);
                }
                doc.Save(pathStudent);
            }
        }

        public Student GetStudent(string hash)
        {
            var students = GetStudents();
            var currentStudent = new Student();
            foreach (var student in students)
            {
                if (student.Hash.Equals(hash))
                {
                    currentStudent = student;
                    break;
                }
            }
            return currentStudent;
        }

        public bool CheckHash(string hash)
        {
            var students = GetStudents();
            var status = false;
            foreach (var student in students)
            {
                if (student.Hash.Equals(hash))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
        #endregion

        #endregion

        #region Common Actions
        private XmlNode GetNodes(string pathToConfig, string configName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathToConfig);

            if (doc != null)
            {
                XmlNode xnodes = doc.SelectSingleNode(configName);

                return xnodes;
            }
            else
            {
                return null;
            }
        }


        public bool CheckObj(Object obj)
        {
            return true;
        }

        #endregion
    }
}
