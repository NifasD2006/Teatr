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

namespace Teatr
{
    /// <summary>
    /// Логика взаимодействия для RepertiurePage.xaml
    /// </summary>
    public partial class RepertiurePage : Page
    {
        private int DevID;

        public RepertiurePage(int Dev)
        {
            InitializeComponent();
            DevID = Dev;
            UpdatePorts();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePorts();
        }
        public void UpdatePorts()
        {
            var currentPorts = TeatrDBEntities2.GetContext().Repertoire.Where(p => p.RepertoirePlayID == DevID).ToList();
            PerformancesListView.ItemsSource = currentPorts;
        }

        private void DelPerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentPort = (sender as Button).DataContext as Repertoire;

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    TeatrDBEntities2.GetContext().Repertoire.Remove(currentPort);
                    TeatrDBEntities2.GetContext().SaveChanges();
                    PerformancesListView.ItemsSource = TeatrDBEntities2.GetContext().Repertoire.ToList();
                    UpdatePorts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            UpdatePorts();
        }

        private void AddPerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.RepertiureFrame.Navigate(new AddPerformance(DevID));
            UpdatePorts();
        }
    }
}
