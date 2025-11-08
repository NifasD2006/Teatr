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
    /// Логика взаимодействия для AddPerformance.xaml
    /// </summary>
    public partial class AddPerformance : Page
    {
        private Repertoire _currentPort = new Repertoire();

        private int _currentDevice;
        public AddPerformance(int DeviceID)
        {
            InitializeComponent();
            _currentDevice = DeviceID;
        }

        private void SavePerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверка номера порта
            if (string.IsNullOrEmpty(PerformanceDatePicker.Text))
            {
                errors.AppendLine("Укажите дату выступления");
            }

            // Проверка Времени
            if (PerformanceTimeTextBox.Text==null)
            {
                errors.AppendLine("Укажите время");
            }
            else
            {
                if (!TimeSpan.TryParse(PerformanceTimeTextBox.Text, out TimeSpan time))
                {
                    errors.AppendLine("Неправильный формат времени. Используйте ЧЧ:мм");
                }
                else
                {
                    // Дополнительная проверка на диапазон 00:00 - 23:59
                    if (time < TimeSpan.Zero || time >= TimeSpan.FromHours(24))
                    {
                        errors.AppendLine("Время должно быть в диапазоне от 00:00 до 23:59");
                    }
                }
            }

            // Проверка статуса порта
            if (PerformanceCostTextBox.Text == null)
            {
                errors.AppendLine("Укажите цену входа");
            }
            else
            {
                if (!decimal.TryParse(PerformanceCostTextBox.Text, out decimal cost))
                {
                    errors.AppendLine("Цена должна быть числом");
                }
                else
                {
                    if (cost <= 0)
                    {
                        errors.AppendLine("Цена должна быть больше 0");
                    }
                    else if (cost > 100000)
                    {
                        errors.AppendLine("Цена не может превышать 100 000 рублей");
                    }
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка! Перепроверьте данные!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Преобразуем цену в decimal
                _currentPort.RepertoireCost = decimal.Parse(PerformanceCostTextBox.Text);

                // Получаем дату из DatePicker и время из TextBox
                DateTime selectedDate = PerformanceDatePicker.SelectedDate ?? DateTime.Now;
                TimeSpan time = TimeSpan.Parse(PerformanceTimeTextBox.Text);

                // Объединяем дату и время
                _currentPort.RepertoireData = selectedDate.Date;
                _currentPort.RepertoireTime = time;
                _currentPort.RepertoirePlayID = _currentDevice;

                if (_currentPort.RepertoireID == 0)
                {
                    TeatrDBEntities2.GetContext().Repertoire.Add(_currentPort);
                }

                TeatrDBEntities2.GetContext().SaveChanges();
                MessageBox.Show("Выступление успешно добавлено!");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelPerformanceBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}
