using System;
using System.Windows;
using WCF_DB_CHAT.ServiceReference1;

namespace WCF_DB_CHAT
{
    public partial class Statistics : Window
    {
        int ID;
        int ID_get;

        public Statistics(int ID, int ID_get)
        {
            InitializeComponent();
            this.ID = ID;
            this.ID_get = ID_get;
        }

        private void button_statistic_Click(object sender, RoutedEventArgs e) //функція відображення статистики спілкування
        {
            if (Pick_date2.SelectedDate == null || Pick_date1.SelectedDate == null)
            {
                MessageBox.Show("Виберіть першу і другу дату!");
            }
            else
            {
                DateTime dt = Pick_date2.SelectedDate.Value; // змінна типу DateTime зберігає дату вибрану з першого DatePicker
                string date1 = Pick_date1.SelectedDate.Value.ToString("yyyy/MM/dd HH:mm:ss"); // змінна типу DateTime зберігає дату вибрану з другого DatePicker
                dt = dt.AddDays(1);
                string date2 = dt.ToString("yyyy/MM/dd HH:mm:ss");

                Service1Client service = new Service1Client();
                int Count = service.GetDATE(ID, ID_get, date1, date2);

                lable_statistic.Content = "Кількість повідомлень за період\n с (" + Pick_date1.SelectedDate.Value.ToString("yyyy/MM/dd") + ") по " +
                "(" + Pick_date2.SelectedDate.Value.ToString("yyyy/MM/dd") + "): [" + Count.ToString() + "] повідомлень.";
            }
        }
    }
}
