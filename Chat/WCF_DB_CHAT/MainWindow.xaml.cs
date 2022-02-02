using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WCF_DB_CHAT.ServiceReference1;

namespace WCF_DB_CHAT
{
    public partial class MainWindow : Window
    {
        int ID; //змінна яка  зберігає id користувача
        int ID_get = 0; // змінна яка зберігає id  вибраного вами співрозмовника
        string name_get; //змінна яка зберігає ім'я вибраного вами співрозмовника
        string login;
        string NAME_st; //змінна яка  зберігає ім'я користувача
        int COUNT; // змінна яка зберігає кількість непрочитаних повідомлень

        Thread t2;// змінні для створення окремого потоку, в данному випадку для відображення чату в окремому потоці і 
        Thread t;   // перевірки непрочитаних смс
         

        struct struct_
        {
            public string Key { get; set; }
            public int Value { get; set; }
        }

        List<struct_> LS;

        List<Contacts> list_contacts = new List<Contacts>();
        Dictionary<string, int> ID_senders = new Dictionary<string, int>();

        public MainWindow(string name, string login)
        {
            InitializeComponent();
            Load_sms.Visibility = Visibility.Hidden;
            NAME_st = name;
            NAME.Content = "Користувач: " + name;
            this.login = login;

            Service1Client service = new Service1Client();
            list_contacts.AddRange(service.GetAllContacts());

            for (int i = 0; i < list_contacts.Count; i++)
            {
                if (list_contacts[i].login == login)
                {
                    ID = list_contacts[i].id_name;
                }
            }
            list_contacts.Clear();

            t2 = new Thread(read_chek);
            t2.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //вихід з облікового запису
        {
            new login_User().Show();
            Close();
        }
        
        void read_chek()//функція перевіряє та відображає загальну кількість непрочитаних повідомленнь
        {
            try
            {
                int count;
                Service1Client service = new Service1Client();
                struct_ A = new struct_();

                while (true)
                {
                    count = 0;
                    ID_senders.Clear();
                    list_contacts.Clear();

                    ID_senders = service.READ_CHECK(ID);
                    list_contacts.AddRange(service.GetAllContacts());
                    LS = new List<struct_>();
                    foreach (var i in ID_senders)
                    {
                        A.Key = i.Key; A.Value = i.Value; LS.Add(A);
                        count += A.Value;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        Label_chek_sms.Content = "Не прочитано (" + count +") смс";
                        lbcontacts.ItemsSource = LS;
                    });
                    Thread.Sleep(5000);
                }
            }
            catch (Exception)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //функція відкриття вікна з статистикою спілкування
        {
            list_contacts.Clear();
            Service1Client service = new Service1Client();
            if (ID_get == 0)
            {
                MessageBox.Show("Не выбраний контакт!");
            }
            else
            {
                new Statistics(ID, ID_get).ShowDialog();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e) // функція відправки повідомлення натисканням ЛКМ
        {
            if (tbMessage.Text != String.Empty)
            {
                Message p = new Message
                {
                    message = tbMessage.Text,
                    id_name = ID,
                    data = DateTime.Now,
                    read_ = false,
                    id_name_get = ID_get
                };

                Service1Client service = new Service1Client();

                service.SendMsg(p);
                tbMessage.Text = "";
            }
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)// функція відправки повідомлення натисканням клавіші Enter
        {
            if (e.Key == Key.Enter)
            {
                if (tbMessage.Text != String.Empty)
                {
                    Message p = new Message
                    {
                        message = tbMessage.Text,
                        id_name = ID,
                        data = DateTime.Now,
                        read_ = false,
                        id_name_get = ID_get
                    };

                    Service1Client service = new Service1Client();

                    service.SendMsg(p);
                    tbMessage.Text = "";
                }
            }
        }


        private void Load_sms_Click(object sender, RoutedEventArgs e) 
        {
            if (t != null)
            {
                t.Abort();
            }

            if (lbcontacts.SelectedItem != null)
            {
                var st = lbcontacts.SelectedItem;
                name_get = ((struct_)st).Key;
            }

            list_contacts.Clear();

            ID_get = 0;
            Service1Client service = new Service1Client();
            list_contacts.AddRange(service.GetAllContacts());

            for (int i = 0; i < list_contacts.Count; i++)
            {
                if (name_get == list_contacts[i].name)
                {
                    ID_get = list_contacts[i].id_name;
                    break;
                }
            }

            list_contacts.Clear();

            t = new Thread(getallsms);
            t.Start();
        }


        public void getallsms()//функція завантаження всіх смс 
        {
            try
            {
                List<Message> messageS = new List<Message>();
                while (true)
                {
                    messageS.Clear();
                    Service1Client service = new Service1Client();

                    messageS.AddRange(service.GetAllMessages(ID, ID_get));
                    Dispatcher.Invoke(() =>
                    {
                        lbChat.Items.Clear();

                        for (int i = 0; i < messageS.Count; ++i)
                        {
                            if (messageS[i].id_name == ID)
                            {
                                lbChat.Items.Add(NAME_st + "\t" + "[" + messageS[i].data + "]: " + messageS[i].message);
                            }
                            else
                            {
                                lbChat.Items.Add(name_get + "\t"  + "[" + messageS[i].data + "]: " + messageS[i].message);
                            }
                        }
                    }
                    );
                    Thread.Sleep(3000);
                }
            }
            catch (Exception)
            {

            }
        }

        public void getsms()// функція завантаження непрочитаних смс
        {
            try
            {
                List<Message> messageS = new List<Message>();
                while (true)
                {
                    messageS.Clear();
                    Service1Client service = new Service1Client();

                    messageS.AddRange(service.GetUNread_Messages(ID, ID_get, COUNT));

                    Dispatcher.Invoke(() =>
                    {
                        lbChat.Items.Clear();

                        for (int i = 0; i < messageS.Count; ++i)
                        {
                            if (messageS[i].id_name == ID)
                            {
                                lbChat.Items.Add(NAME_st + "\t" + "[" + messageS[i].data + "]: " + messageS[i].message);
                            }
                            else
                            {
                                lbChat.Items.Add(name_get + "\t" + "[" + messageS[i].data + "]: " + messageS[i].message);
                            }
                        }
                    }
                    );
                    Thread.Sleep(2000);
                }
            }
            catch (Exception)
            {

            }
        }

        private void Load_sms_Click2(object sender, SelectionChangedEventArgs e)
        {
            if (t != null)
            {
                t.Abort();
            }

            if (lbcontacts.SelectedItem != null)
            {
                var st = lbcontacts.SelectedItem;

                name_get = ((struct_)st).Key;
                COUNT = ((struct_)st).Value;
                Load_sms.Visibility = Visibility.Visible;
            }

            list_contacts.Clear();

            ID_get = 0;
            Service1Client service = new Service1Client();
            list_contacts.AddRange(service.GetAllContacts());

            for (int i = 0; i < list_contacts.Count; i++)
            {
                if (name_get == list_contacts[i].name)
                {
                    ID_get = list_contacts[i].id_name;
                    break;
                }
            }

            list_contacts.Clear();

            t = new Thread(getsms);
            t.Start();
        }
    }
}