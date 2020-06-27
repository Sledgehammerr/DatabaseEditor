﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>    
    public partial class App : Application
    {

        public static Model.EFContext _db;
        public Model.EFContext db
        {
            get { return _db; }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _db = new Model.EFContext("Database2");
            base.OnStartup(e);
            //var s = new Model.Student
            //{
            //    Id = 1,
            //    Name = "Student2",
            //    Admission = new DateTime(2020, 9, 1),
            //    Group = db.Groups.Find(0)
            //};
            //db.Students.Add(s);
            //db.SaveChanges();
            foreach (var student in db.Students)
            {
                Debug.WriteLine($"{student.Name}, {student.Group.Track}-{student.Group.Course}{student.Group.Number}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Debug.WriteLine($"ExitCode ={e.ApplicationExitCode}");
            base.OnExit(e);
        }
    }
}
