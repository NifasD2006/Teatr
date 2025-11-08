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
using System.Data.Entity;

namespace Teatr
{
    /// <summary>
    /// Логика взаимодействия для PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page
    {
        int MaxCountPlays;
        public PlayPage()
        {
            InitializeComponent();
            var currentPlayes = TeatrDBEntities2.GetContext().Play.Include(d => d.Repertoire).ToList();

            WendorComboBox.SelectedIndex = 0;
            PlayListView.ItemsSource = currentPlayes;
            int CountDevices = PlayListView.Items.Count;
            MaxCountPlays = currentPlayes.Count;
            MaxCountTBlock.Text = MaxCountPlays.ToString();
            UpdatePlay();
        }
        private void UpdatePlay()
        {
            var currentPlayes = TeatrDBEntities2.GetContext().Play.Include(d => d.Genre).Include(d => d.Rating).ToList();
            MaxCountPlays = currentPlayes.Count;
            MaxCountTBlock.Text = MaxCountPlays.ToString();
            if (WendorComboBox.SelectedIndex == 1)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Комедия").ToList();
            if (WendorComboBox.SelectedIndex == 2)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Драма").ToList();
            if (WendorComboBox.SelectedIndex == 3)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Милодрама").ToList();
            if (WendorComboBox.SelectedIndex == 4)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Мюзикл").ToList();
            if (WendorComboBox.SelectedIndex == 5)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Трагедия").ToList();
            if (WendorComboBox.SelectedIndex == 6)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Опера").ToList();
            if (WendorComboBox.SelectedIndex == 7)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Сатира").ToList();
            if (WendorComboBox.SelectedIndex == 8)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Феерия").ToList();
            if (WendorComboBox.SelectedIndex == 9)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Сказка").ToList();
            if (WendorComboBox.SelectedIndex == 10)
                currentPlayes = currentPlayes.Where(p => p.Genre.GenreName == "Экшен").ToList();

            currentPlayes = currentPlayes.Where(p => p.PlayName.ToLower().Contains(TextBoxSearch.Text.ToLower()) || p.PlayAuthor.ToLower().Contains(TextBoxSearch.Text.ToLower()) ||
                          p.Rating.RatingName.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToList();



            if (RButtonDown.IsChecked.Value)
            {
                currentPlayes = currentPlayes.OrderByDescending(p => p.Rating.RatingID).ToList();
            }

            if (RButtonUP.IsChecked.Value)
            {
                currentPlayes = currentPlayes.OrderBy(p => p.Rating.RatingID).ToList();
            }
 
            PlayListView.ItemsSource = currentPlayes;
            int CountDevices = PlayListView.Items.Count;
            CountTBlock.Text = CountDevices.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int CountDevices = PlayListView.Items.Count;
            UpdatePlay();
        }
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlay();
        }

        private void WendorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePlay();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Play));
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentDevice = (sender as Button).DataContext as Play;
            var currentPortsDevice = TeatrDBEntities2.GetContext().Repertoire.ToList();
            currentPortsDevice = currentPortsDevice.Where(p => p.RepertoirePlayID == currentDevice.PlayID).ToList();
            if (currentPortsDevice.Count != 0)
            {
                MessageBox.Show("Этот спектакль имеет выступления! Сперва удалить выступления.");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        TeatrDBEntities2.GetContext().Play.Remove(currentDevice);
                        TeatrDBEntities2.GetContext().SaveChanges();
                        PlayListView.ItemsSource = TeatrDBEntities2.GetContext().Play.ToList();
                        MaxCountPlays = MaxCountPlays - 1;
                        MaxCountTBlock.Text = MaxCountPlays.ToString();
                        UpdatePlay();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void RefrashBtn_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearch.Clear();
            WendorComboBox.SelectedIndex = 0;
            RButtonUP.IsChecked = false;
            RButtonDown.IsChecked = false;
            UpdatePlay();

        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePlay();
        }

        private void RButtonUP_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePlay();
        }
    }
}
