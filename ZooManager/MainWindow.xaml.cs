using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ZooManager
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private readonly LinqToSqlDataClassDataContext _dataContext;

      public MainWindow()
      {
         InitializeComponent();

         string connectionString = ConfigurationManager
            .ConnectionStrings["ZooManager.Properties.Settings.SqlTutorialDBConnectionString"]
            .ConnectionString;

         _dataContext = new LinqToSqlDataClassDataContext(connectionString);

         ShowZoos();
         ShowAnimals();
      }

      private void ShowZoos()
      {
         zoosList.ItemsSource = _dataContext.Zoos.Select(zoo => zoo.Location);
      }

      private void ShowAssosiatedAnimals()
      {
         if (zoosList.SelectedValue != null)
         {
            var animals = from a in _dataContext.Animals
                          join az in _dataContext.AnimalZoos
                          on a.Id equals az.AnimalId
                          where az.Zoo.Location == zoosList.SelectedValue.ToString()
                          select a.Name;

            assosiatedAnimalsList.ItemsSource = animals;
         }
         else
         {
            assosiatedAnimalsList.ItemsSource = null;
         }
      }

      private void ShowAnimals()
      {
         animalsList.ItemsSource = _dataContext.Animals.Select(animal => animal.Name);
      }

      private void ShowSelectedZooInTextBox()
      {
         if (zoosList.SelectedValue != null)
         {
            addZooTextBox.Text = zoosList.SelectedValue.ToString();
         }
      }

      private void ShowSelectedAnimalInTextBox()
      {
         if (animalsList.SelectedValue != null)
         {
            addAnimalTextBox.Text = animalsList.SelectedValue.ToString();
         }
      }

      private void ZoosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         ShowAssosiatedAnimals();
         ShowSelectedZooInTextBox();
      }

      private void AnimalsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         ShowSelectedAnimalInTextBox();
      }

      private void AddZoo_Click(object sender, RoutedEventArgs e)
      {
         string newZoo = addZooTextBox.Text;

         if (IsZooDuplicate(newZoo))
         {
            MessageBox.Show($"The Zoo in {newZoo} is already in the list");
            return;
         }

         _dataContext.Zoos.InsertOnSubmit(new Zoo { Location = newZoo });

         _dataContext.SubmitChanges();

         ShowZoos();
      }

      private void AddAnimal_Click(object sender, RoutedEventArgs e)
      {
         string newAnimal = addAnimalTextBox.Text;

         if (IsAnimalDuplicate(newAnimal))
         {
            MessageBox.Show($"The {newAnimal} is already in the list");
            return;
         }

         _dataContext.Animals.InsertOnSubmit(new Animal { Name = newAnimal });

         _dataContext.SubmitChanges();

         ShowAnimals();
      }

      private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
      {
         Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));
         Animal selectedAnimal = _dataContext.Animals.FirstOrDefault(zoo => zoo.Name.Equals(animalsList.SelectedValue));

         _dataContext.AnimalZoos.InsertOnSubmit(new AnimalZoo { Zoo = selectedZoo, Animal = selectedAnimal });

         _dataContext.SubmitChanges();

         ShowAssosiatedAnimals();
      }

      private void DeleteZoo_Click(object sender, RoutedEventArgs e)
      {
         Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));
         _dataContext.Zoos.DeleteOnSubmit(selectedZoo);

         _dataContext.SubmitChanges();

         ShowZoos();
      }

      private void RemoveAnimalFromZoo_Click(object sender, RoutedEventArgs e)
      {
         Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));
         AnimalZoo selectedAnimalZoo = _dataContext.AnimalZoos
            .FirstOrDefault(az => az.Zoo.Equals(selectedZoo) && az.Animal.Name.Equals(assosiatedAnimalsList.SelectedValue));

         _dataContext.AnimalZoos.DeleteOnSubmit(selectedAnimalZoo);

         _dataContext.SubmitChanges();

         ShowAssosiatedAnimals();
      }

      private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
      {
         Animal selectedAnimal = _dataContext.Animals.FirstOrDefault(a => a.Name.Equals(animalsList.SelectedValue));
         _dataContext.Animals.DeleteOnSubmit(selectedAnimal);

         _dataContext.SubmitChanges();

         ShowAnimals();
      }

      private bool IsZooDuplicate(string newZoo)
      {
         return _dataContext.Zoos.Any(zoo => zoo.Location == newZoo);
      }

      private bool IsAnimalDuplicate(string newAnimal)
      {
         return _dataContext.Animals.Any(a => a.Name == newAnimal);
      }
   }
}