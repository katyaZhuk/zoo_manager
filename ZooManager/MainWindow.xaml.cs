using System.Configuration;
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
            ShowDefaultValueInAnimalTextBox();
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
         }
         else if (newZoo == Constants.DefaultZooTextboxValue)
         {
            MessageBox.Show($"Please, {Constants.DefaultZooTextboxValue}");
         }
         else
         {
            _dataContext.Zoos.InsertOnSubmit(new Zoo { Location = newZoo });

            _dataContext.SubmitChanges();

            ShowZoos();
         }
      }

      private void AddAnimal_Click(object sender, RoutedEventArgs e)
      {
         string newAnimal = addAnimalTextBox.Text;

         if (IsAnimalDuplicate(newAnimal))
         {
            MessageBox.Show($"{newAnimal} is already in the list");
         }
         else if (newAnimal == Constants.DefaultAnimalTextboxValue)
         {
            MessageBox.Show($"Please, {Constants.DefaultAnimalTextboxValue}");
         }
         else
         {
            _dataContext.Animals.InsertOnSubmit(new Animal { Name = newAnimal });

            _dataContext.SubmitChanges();

            ShowAnimals();
         }
      }

      private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
      {
         if (zoosList.SelectedValue != null && animalsList.SelectedValue != null)
         {

            Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));
            Animal selectedAnimal = _dataContext.Animals.FirstOrDefault(zoo => zoo.Name.Equals(animalsList.SelectedValue));

            _dataContext.AnimalZoos.InsertOnSubmit(new AnimalZoo { Zoo = selectedZoo, Animal = selectedAnimal });

            _dataContext.SubmitChanges();

            ShowAssosiatedAnimals();
         }
         else
         {
            MessageBox.Show("Please, select Zoo location and Animal to add");
         }
      }

      private void DeleteZoo_Click(object sender, RoutedEventArgs e)
      {
         if (zoosList.SelectedValue != null)
         {
            Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));

            if (ShowDeleteConfirmationBox() == MessageBoxResult.Yes)
            {
               _dataContext.Zoos.DeleteOnSubmit(selectedZoo);

               _dataContext.SubmitChanges();

               ShowZoos();
               ShowDefaultValueInZooTextBox();
            }
            else
            {
               MessageBox.Show("Delete operation terminated");
            }
         }
         else
         {
            MessageBox.Show("Please, select Zoo to remove");
         }
      }

      private void RemoveAnimalFromZoo_Click(object sender, RoutedEventArgs e)
      {
         if (zoosList.SelectedValue != null && assosiatedAnimalsList.SelectedValue != null)
         {
            Zoo selectedZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location.Equals(zoosList.SelectedValue));
            AnimalZoo selectedAnimalZoo = _dataContext.AnimalZoos
               .FirstOrDefault(az => az.Zoo.Equals(selectedZoo) && az.Animal.Name.Equals(assosiatedAnimalsList.SelectedValue));

            if (ShowDeleteConfirmationBox() == MessageBoxResult.Yes)
            {
               _dataContext.AnimalZoos.DeleteOnSubmit(selectedAnimalZoo);

               _dataContext.SubmitChanges();

               ShowAssosiatedAnimals();
               ShowDefaultValueInAnimalTextBox();
            }
            else
            {
               MessageBox.Show("Delete operation terminated");
            }
         }
         else
         {
            MessageBox.Show("Please, select Zoo location and Animal to remove");
         }
      }

      private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
      {
         if (animalsList.SelectedValue != null)
         {
            Animal selectedAnimal = _dataContext.Animals.FirstOrDefault(a => a.Name.Equals(animalsList.SelectedValue));

            if (ShowDeleteConfirmationBox() == MessageBoxResult.Yes)
            {

               _dataContext.Animals.DeleteOnSubmit(selectedAnimal);

               _dataContext.SubmitChanges();

               ShowAnimals();
               ShowAssosiatedAnimals();
               ShowDefaultValueInAnimalTextBox();
            }
            else
            {
               MessageBox.Show("Delete operation terminated");
            }
         }
         else
         {
            MessageBox.Show("Please, select Animal to delete");
         }
      }

      private void UpdateZoo_Click(object sender, RoutedEventArgs e)
      {
         if (zoosList.SelectedValue != null)
         {
            string initialZooLocation = zoosList.SelectedValue.ToString();
            string updatedZoo = addZooTextBox.Text;

            if (IsZooDuplicate(updatedZoo))
            {
               MessageBox.Show($"The Zoo in {updatedZoo} is already in the list");
               return;
            }

            Zoo initialZoo = _dataContext.Zoos.FirstOrDefault(zoo => zoo.Location == initialZooLocation);

            initialZoo.Location = updatedZoo;

            _dataContext.SubmitChanges();

            ShowZoos();
         }
         else
         {
            MessageBox.Show("Please, select Zoo to update");
         }
      }

      private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
      {
         if (animalsList.SelectedValue != null)
         {
            string initialAnimalName = animalsList.SelectedValue.ToString();
            string updatedAnimal = addAnimalTextBox.Text;

            if (IsAnimalDuplicate(updatedAnimal))
            {
               MessageBox.Show($"{updatedAnimal} is already in the list");
               return;
            }

            Animal initialAnimal = _dataContext.Animals.FirstOrDefault(an => an.Name == initialAnimalName);

            initialAnimal.Name = updatedAnimal;

            _dataContext.SubmitChanges();

            ShowAnimals();
            ShowAssosiatedAnimals();
         }
         else
         {
            MessageBox.Show("Please, select Animal to update");
         }
      }

      private bool IsZooDuplicate(string newZoo)
      {
         return _dataContext.Zoos.Any(zoo => zoo.Location == newZoo);
      }

      private bool IsAnimalDuplicate(string newAnimal)
      {
         return _dataContext.Animals.Any(a => a.Name == newAnimal);
      }

      private void ShowDefaultValueInZooTextBox()
      {
         addZooTextBox.Text = Constants.DefaultZooTextboxValue;
      }

      private void ShowDefaultValueInAnimalTextBox()
      {
         addAnimalTextBox.Text = Constants.DefaultAnimalTextboxValue;
      }

      private MessageBoxResult ShowDeleteConfirmationBox()
      {
         return MessageBox.Show("Are you sure ? ", "Delete Confirmation", MessageBoxButton.YesNo);
      }
   }
}