using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using avto.avtoDataSetTableAdapters;


namespace avto
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        typeModelTableAdapter typeModel = new typeModelTableAdapter();
        ModelsTableAdapter models = new ModelsTableAdapter();
        public MainWindow()
        {
            InitializeComponent();

            typeModelDataGrid.ItemsSource = typeModel.GetData();
            //typeModelComboBox.ItemsSource = typeModel.GetData();
            //typeModelComboBox.DisplayMemberPath = "title";


            ModelsDataGrid.ItemsSource = models.GetData();
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModelsDataGrid.Columns[0].Visibility = Visibility.Collapsed;
            ModelsDataGrid.Columns[2].Visibility = Visibility.Collapsed;

        }
    }
}
