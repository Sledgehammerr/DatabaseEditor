﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WpfApp4.Model;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CollectionViewSource studentViewSource;
        CollectionViewSource groupViewSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = (Application.Current as App).db;
            db.Students.Load();
            db.Groups.Load();


            studentViewSource = ((CollectionViewSource)(this.FindResource("studentViewSource")));
            studentViewSource.Source = db.Students.Local;

            groupViewSource = ((CollectionViewSource)(this.FindResource("groupViewSource")));
            groupViewSource.Source = db.Groups.Local;

        }

        private void StudentsFilter(object sender, FilterEventArgs e)
        {
            var student = e.Item as Model.Student;
            var group = groupSelectList.SelectedItem as Model.Group;
            if (group != null)
                e.Accepted = student.GroupId == group.Id;
            else
                e.Accepted = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var db = (Application.Current as App).db;
            var button = sender as Button;

            switch (button.Tag)
            {
                case "addStudent":
                    var studentDialog = new NewStudent();
                    if (studentDialog.ShowDialog() == true)
                    {
                        db.Students.Add(studentDialog.FindResource("newStudent") as Model.Student);
                    }
                    studentViewSource.Filter -= StudentsFilter;
                    studentViewSource.Filter += StudentsFilter;
                    break;
                case "removeStudent":
                    db.Students.Remove(listBox.SelectedItem as Model.Student);
                    break;
                case "addGroup":
                    var groupDialog = new NewGroup();
                    if (groupDialog.ShowDialog() == true)
                    {
                        db.Groups.Add(groupDialog.FindResource("newGroup") as Model.Group);
                    }
                    break;
                case "removeGroup":
                    db.Groups.Remove(groupListBox.SelectedItem as Model.Group);
                    break;
                default:
                    MessageBox.Show(button.Tag?.ToString() ?? "");
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var db = (Application.Current as App).db;
            if (db.ChangeTracker.HasChanges())
            {
                var dialog = MessageBox.Show("Save changes?", "Exit", MessageBoxButton.OKCancel);
                if (dialog == MessageBoxResult.OK)
                {
                    db.SaveChanges();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            studentViewSource.Filter -= StudentsFilter;
            studentViewSource.Filter += StudentsFilter;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var studentsCollection = studentViewSource.Source as ObservableCollection<Model.Student>;

            var currentGroup = groupSelectList.SelectedItem as Model.Group;

            var data = from student in studentsCollection
                       where student.Group == currentGroup
                       select student;

            var export = new XLSExportProvider(data);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel|*.xlsx";
            saveDialog.AddExtension = true;
            if (saveDialog.ShowDialog() == true)
            {
                var fileName = saveDialog.FileName;
                if (fileName.IndexOf("xls") == -1)
                {
                    fileName += ".xlsx";
                }
                export.ExportTo(fileName);
            }
        }
    }
}
