using System.Collections.Generic;
using System.Windows;
using WCF_DB_CHAT.ServiceReference1;

namespace WCF_DB_CHAT
{
    public partial class login_User : Window
    {
        public login_User()
        {
            InitializeComponent();
        }

        private void reg_Click(object sender, RoutedEventArgs e) //функція реєстрації користувача 
        {
            bool chek = false;
            List<Contacts> list_contacts = new List<Contacts>();
            Service1Client service = new Service1Client();

            list_contacts.AddRange(service.GetAllContacts());
            if (TB_name.Text.Length < 3 || TB_name.Text.Length > 10)
            {
                MessageBox.Show("Довжина імені повинна бути не менше 3 сим. і не більше 10\n");
            }
            else if (TB_login_reg.Text.Length < 3 || TB_password_reg.Text.Length < 3)
            {
                MessageBox.Show("Довжина пароля або логіна повинна бути не менше 3 сим.\n");
            }
            else
            {
                for (int i = 0; i < list_contacts.Count; i++)
                {
                    if (list_contacts[i].name == TB_name.Text)
                    {
                        chek = true;
                        MessageBox.Show("Користувач з таким ім'ям вже існує!\n");
                        break;
                    }
                    if (list_contacts[i].login == TB_login_reg.Text)
                    {
                        chek = true;
                        MessageBox.Show("Користувач з таким іменем вже існує!\n");
                        break;
                    }
                }

                if (chek == false)
                {
                    service = new Service1Client();
                    Contacts contacts = new Contacts();

                    contacts.name = TB_name.Text;
                    contacts.login = TB_login_reg.Text;
                    contacts.password = TB_password_reg.Text;

                    service.registration(contacts);

                    new MainWindow(TB_name.Text, TB_login_reg.Text).Show();
                    Close();
                }
            }
        }

        private void voit_Click(object sender, RoutedEventArgs e) //функція входу в обліковий запис
        {
            bool chek = false;
            List<Contacts> list_contacts = new List<Contacts>();
            Service1Client service = new Service1Client();

            list_contacts.AddRange(service.GetAllContacts());

            if (TB_login_voit.Text.Length < 3 || TB_password_voit.Text.Length < 3)
            {
                MessageBox.Show("Довжина пароля або логіна повинна бути не менше ніж 3 сим.\n");
            }
            else
            {
                for (int i = 0; i < list_contacts.Count; i++)
                {
                    if (list_contacts[i].login != TB_login_voit.Text)
                    {
                        chek = false;
                    }
                    else
                    {
                        chek = true;
                        if (list_contacts[i].password != TB_password_voit.Text)
                        {
                            MessageBox.Show("Пароль введений не вірно!\n");
                            break;
                        }
                        else
                        {
                            new MainWindow(list_contacts[i].name, list_contacts[i].login).Show();
                            Close();
                            break;
                        }
                    }
                }

                if (chek == false)
                {
                    MessageBox.Show("Користувача, з таким логіном не існує !\n");
                }
            }
        }

        private void regREG_Click(object sender, RoutedEventArgs e)
        {
            regLOG.BorderBrush = null;
            regREG.BorderBrush = System.Windows.Media.Brushes.White;
            
            logBox.Visibility = Visibility.Hidden;
            regBox.Visibility = Visibility.Visible;
            
        }

        private void regLOG_Click(object sender, RoutedEventArgs e)
        {
            regREG.BorderBrush = null;
            regLOG.BorderBrush = System.Windows.Media.Brushes.White;

            regBox.Visibility = Visibility.Hidden;
            logBox.Visibility = Visibility.Visible;
            
        }
    }
}
