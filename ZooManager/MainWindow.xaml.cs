using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

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

         string connectionString = ConfigurationManager.ConnectionStrings["ZooManager.Properties.Settings.SqlTutorialDBConnectionString"].ConnectionString;

         _dataContext = new LinqToSqlDataClassDataContext(connectionString);
      }
   }
}
