using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class TransExpressionTests
    {
        [TestMethod()]
        public void TransTest()
        {
            Student stu1 = new(1, "Carlos", "This is a good student.");
            Student stu2 = new();
            for (int i = 0; i < 5000000; i++) stu2 = TransExpression<Student, Student>.Trans(stu1);
            Console.WriteLine($"stu1:(id={stu1.Id},name={stu1.Name},desc={stu1.Description})");
            Console.WriteLine($"stu2:(id={stu2.Id},name={stu2.Name},desc={stu2.Description})");
        }
        [TestMethod()]
        public void CloneTest()
        {
            Student stu1 = new(1, "Carlos", "This is a good student.");
            Student stu2 = new();
            for(int i = 0;i <5000000;i++)
            {
                stu2.Id = stu1.Id;
                stu2.Name = stu1.Name;
                stu2.Description = stu1.Description;
            }
            Console.WriteLine($"stu1:(id={stu1.Id},name={stu1.Name},desc={stu1.Description})");
            Console.WriteLine($"stu2:(id={stu2.Id},name={stu2.Name},desc={stu2.Description})");
        }
    }
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Student() { }
        public Student(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
