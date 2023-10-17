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
         var animals = from a in _dataContext.Animals
                       join az in _dataContext.AnimalZoos
                       on a.Id equals az.AnimalId
                       where az.Zoo.Location == zoosList.SelectedValue.ToString()
                       select a.Name;

         assosiatedAnimalsList.ItemsSource = animals;
      }

      private void ShowAnimals()
      {
         animalsList.ItemsSource = _dataContext.Animals.Select(animal => animal.Name);
      }

      private void ShowSelectedZooInTextBox()
      {
         addZooTextBox.Text = zoosList.SelectedValue.ToString();
      }

      private void ShowSelectedAnimalInTextBox()
      {
         addAnimalTextBox.Text = animalsList.SelectedValue.ToString();
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
         _dataContext.Zoos.InsertOnSubmit(new Zoo { Location = newZoo });

         _dataContext.SubmitChanges();

         ShowZoos();
      }

      private void AddAnimal_Click(object sender, RoutedEventArgs e)
      {
         string newAnimal = addAnimalTextBox.Text;
         _dataContext.Animals.InsertOnSubmit(new Animal { Name = newAnimal });

         _dataContext.SubmitChanges();

         ShowAnimals();
      }

      private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
      {
         Zoo selectedZoo = new Zoo { Location = zoosList.SelectedValue.ToString() };
         Animal selectedAnimal = new Animal { Name = animalsList.SelectedValue.ToString() };

         _dataContext.AnimalZoos.InsertOnSubmit(new AnimalZoo { Zoo = selectedZoo, Animal = selectedAnimal });

         _dataContext.SubmitChanges();

         ShowAssosiatedAnimals();
      }
   }
}
