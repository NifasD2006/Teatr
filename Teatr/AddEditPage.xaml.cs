using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private string currentImagePath = "";
        private Play _currentDevice = new Play();
        private int devid;

        public AddEditPage(Play SelectDevice)
        {

            InitializeComponent();
            Manager.RepertiureFrame = RepertiureFrame;
            if (SelectDevice != null)
            {
                _currentDevice = SelectDevice;
                if (_currentDevice.PlayID != 0) 
                {
                    GenreCombo.SelectedIndex = _currentDevice.PlayGanreID-1;
                    RatingCB.SelectedIndex = _currentDevice.PlayRatingID-1;

                    if (!string.IsNullOrEmpty(_currentDevice.PlayImage))
                    {
                        try
                        {
                            string basePath = @"D:\УАТ\4 Курс\Разработка программных модулей Тимашева\Курсовая\Teatr\Teatr\";
                            string fullImagePath = System.IO.Path.Combine(basePath, _currentDevice.PlayImage);

                            if (System.IO.File.Exists(fullImagePath))
                            {
                                PosterPreview.Source = new BitmapImage(new Uri(fullImagePath));
                                currentImagePath = fullImagePath;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                        }
                    }

                }
                if (PosterPreview.Source == null)
                {
                    try
                    {
                        string defaultImagePath = @"D:\УАТ\4 Курс\Разработка программных модулей Тимашева\Курсовая\Teatr\Teatr\res\picther.png";
                        if (System.IO.File.Exists(defaultImagePath))
                        {
                            PosterPreview.Source = new BitmapImage(new Uri(defaultImagePath));
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибки загрузки заглушки
                    }
                }



                if (_currentDevice.PlayPushkinCard == true)
                {
                    PushkinCardBox.IsChecked = true;
                }
                else
                {
                    PushkinCardBox.IsChecked = false;
                }



                devid = _currentDevice.PlayID;
                Manager.RepertiureFrame = RepertiureFrame;
                RepertiureFrame.Navigate(new RepertiurePage(devid));
            }
            DataContext = _currentDevice;
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(_currentDevice.PlayName))
                errors.AppendLine("Укажите название спектакля");
            if (string.IsNullOrEmpty(_currentDevice.PlayAuthor))
                errors.AppendLine("Укажите автора");
            if (_currentDevice.PlayDuration <=0 || _currentDevice.PlayDuration>=360 )
                errors.AppendLine("Неверное значение длительности спектакля");
            if (string.IsNullOrEmpty(_currentDevice.PlayCountry))
                errors.AppendLine("Укажите страну");

            if (GenreCombo.SelectedItem == null)
                errors.AppendLine("Укажите жанр");
            if (RatingCB.SelectedItem == null)
                errors.AppendLine("Укажите рейтинг");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentDevice.PlayGanreID = GenreCombo.SelectedIndex + 1;
            _currentDevice.PlayRatingID = RatingCB.SelectedIndex + 1;
            if (PushkinCardBox.IsChecked == true)
            {
                _currentDevice.PlayPushkinCard = true;
            }
            else
            {
                _currentDevice.PlayPushkinCard= false;
            }

            // СОХРАНЕНИЕ ИЗОБРАЖЕНИЯ
            if (PosterPreview.Source != null)
            {
                try
                {
                    // Получаем полный путь к выбранному изображению
                    string fullImagePath = PosterPreview.Source.ToString().Replace("file:///", "");

                    // Извлекаем только имя файла
                    string fileName = System.IO.Path.GetFileName(fullImagePath);

                    // Формируем путь для базы данных в формате "res\имя_файла"
                    _currentDevice.PlayImage = $"res\\{fileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке изображения: {ex.Message}");
                    return;
                }
            }
            else
            {
                // Если изображение не выбрано, можно установить NULL или значение по умолчанию
                _currentDevice.PlayImage = null;
            }

            if (_currentDevice.PlayID == 0)
            {
                TeatrDBEntities2.GetContext().Play.Add(_currentDevice);
            }

            try
            {
                TeatrDBEntities2.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Путь к папке res
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string resFolderPath = System.IO.Path.Combine(basePath, "res");

            // Проверяем существование папки
            if (!System.IO.Directory.Exists(resFolderPath))
            {
                MessageBox.Show("Папка res не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем диалоговое окно выбора файла
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = resFolderPath;
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            openFileDialog.Title = "Выберите постер спектакля из папки res";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string selectedDirectory = System.IO.Path.GetDirectoryName(selectedFilePath);

                if (string.Equals(selectedDirectory, resFolderPath, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        PosterPreview.Source = new BitmapImage(new Uri(selectedFilePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите изображение только из папки res!",
                                  "Неверная папка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
