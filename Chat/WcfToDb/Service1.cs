using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static System.Console;

namespace WcfToDb
{
    public class Service1 : IService1
    {
        public Service1()
        {
            ConnectToDb();
        }

        SqlConnection conn;
        SqlCommand comm;

        SqlConnectionStringBuilder connStringBuilder;

        void ConnectToDb() //функція підключення до БД;
        {
            connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = "DESKTOP-GGULDJI";
            connStringBuilder.InitialCatalog = "WCF_CHAT";
            connStringBuilder.Encrypt = true;
            connStringBuilder.TrustServerCertificate = true;
            connStringBuilder.ConnectTimeout = 30;
            connStringBuilder.AsynchronousProcessing = true;
            connStringBuilder.MultipleActiveResultSets = true;
            connStringBuilder.IntegratedSecurity = true;

            conn = new SqlConnection(connStringBuilder.ToString());
            comm = conn.CreateCommand();
        }


        public int GetDATE(int ID, int ID_get, string date1, string date2) //функція повертає кількість смс за певний період;
        {
            List<Message> messageS = new List<Message>();
            int Count = 0;
            try
            {
                comm.CommandText = "SELECT * FROM Message_table WHERE ((id_name = " + ID + " and id_name_get = " + ID_get + ") OR (id_name = " + ID_get + " AND id_name_get = " + ID + ")) and (data >= convert(date,'" + date1 + "') and data <= convert(date,'" + date2 + "'))";

                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Count++;
                }

                return Count;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return 0;
            }
        }


        public int registration(Contacts contacts) //функція додавання користувача в БД 
        {
            try
            {
                comm.CommandText = "INSERT INTO Contacts_table VALUES( @name, @login, @password)";
                comm.Parameters.AddWithValue("name", contacts.name);
                comm.Parameters.AddWithValue("login", contacts.login);
                comm.Parameters.AddWithValue("password", contacts.password);

                comm.CommandType = CommandType.Text;
                conn.Open();
                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return 0;
            }
        }

        public List<Contacts> GetAllContacts() //функція виводить всіх користувачів з БД 
        {
            List<Contacts> list_contacts = new List<Contacts>();
            try
            {
                comm.CommandText = "SELECT * FROM Contacts_table";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    Contacts contacts = new Contacts()
                    {
                        id_name = Convert.ToInt32(reader[0]),
                        name = reader[1].ToString(),
                        login = reader[2].ToString(),
                        password = reader[3].ToString()
                    };
                    list_contacts.Add(contacts);
                }
                conn.Close();
                return list_contacts;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return null;
            }
        }

        public List<Message> GetAllMessages(int ID, int ID_get)// функція повертає всі повідомлення з вибраним користувачем
        {
            List<Message> messageS = new List<Message>();
            try
            {
                comm.CommandText = "SELECT * FROM Message_table WHERE id_name = " + ID + " and id_name_get = " + ID_get + " OR id_name = " + ID_get + " AND id_name_get = " + ID;

                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    Message message = new Message()
                    {
                        id = Convert.ToInt32(reader[0]),
                        message = reader[1].ToString(),
                        id_name = Convert.ToInt32(reader[2]),
                        data = (DateTime)reader[3],
                        read_ = Convert.ToBoolean(reader[4]),
                        id_name_get = Convert.ToInt32(reader[5])
                    };
                    messageS.Add(message);
                }
                conn.Close();
                return messageS;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return null;
            }
        }


        public Dictionary<string, int> READ_CHECK(int ID)//функція перевіряє чи є непрочитані повідомлення з користувачем
        {
            Dictionary<int, int> ID_senders = new Dictionary<int, int>();
            Dictionary<string, int> ID_senders2 = new Dictionary<string, int>();
            List<Contacts> list_contacts = new List<Contacts>();
            list_contacts.AddRange(GetAllContacts());
            try
            {
                comm.CommandText = "SELECT * FROM Message_table WHERE id_name_get = " + ID + " and read_ = 0";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (ID_senders.ContainsKey(Convert.ToInt32(reader[2])))
                    {
                        ID_senders[Convert.ToInt32(reader[2])] += 1;
                    }
                    else
                    {
                        ID_senders.Add(Convert.ToInt32(reader[2]), 1);
                    }
                }

                for (int i = 0; i < list_contacts.Count; i++)
                {
                    if (ID_senders.ContainsKey(list_contacts[i].id_name))
                    {
                        ID_senders2.Add(list_contacts[i].name, ID_senders[list_contacts[i].id_name]);
                    }
                }
                for (int i = 0; i < list_contacts.Count; i++)
                {
                    if (ID != list_contacts[i].id_name)
                    {
                        if (!ID_senders.ContainsKey(list_contacts[i].id_name))
                        {
                            ID_senders2.Add(list_contacts[i].name, 0);
                        }
                    }

                }
                conn.Close();
                return ID_senders2;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return null;
            }
        }

        public List<Message> GetUNread_Messages(int ID, int ID_get, int COUNT) //функція повертає всі непрочитані повідомлення з БД і змінює їх статус на прочитані
        {
            List<Message> messageS = new List<Message>();
            try
            {
                comm.CommandText = "SELECT TOP " + (COUNT + 5) + " * FROM Message_table WHERE (id_name = " + ID + " and id_name_get = " + ID_get + " OR id_name = " + ID_get + " AND id_name_get = " + ID + ") ORDER BY id DESC";
                comm.CommandType = CommandType.Text;
                conn.Open();


                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    Message message = new Message()
                    {
                        id = Convert.ToInt32(reader[0]),
                        message = reader[1].ToString(),
                        id_name = Convert.ToInt32(reader[2]),
                        data = (DateTime)reader[3],
                        read_ = Convert.ToBoolean(reader[4]),
                        id_name_get = Convert.ToInt32(reader[5])
                    };
                    messageS.Add(message);
                }
                conn.Close();
                conn.Open();
                comm.CommandText = "UPDATE Message_table SET read_ = @read_ WHERE (id_name = " + ID_get + " AND id_name_get = " + ID + ") AND read_ = " + 0;

                comm.Parameters.AddWithValue("read_", 1);
                comm.CommandType = CommandType.Text;
                comm.ExecuteNonQuery();
                conn.Close();

                messageS.Reverse();

                return messageS;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return null;
            }
        }

        public List<Message> GetAllALLMessages()//функція повертає всі повідомлення з БД
        {
            List<Message> messageS = new List<Message>();
            try
            {
                comm.CommandText = "SELECT * FROM Message_table";
                comm.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Message message = new Message()
                    {
                        id = Convert.ToInt32(reader[0]),
                        message = reader[1].ToString(),
                        id_name = Convert.ToInt32(reader[2]),
                        data = (DateTime)reader[3],
                        read_ = Convert.ToBoolean(reader[4])
                    };
                    messageS.Add(message);
                }
                return messageS;
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return null;
            }
        }


        public int SendMsg(Message p) //функція додає повідомлення до БД;
        {
            try
            {
                comm.CommandText = "INSERT INTO Message_table VALUES( @message, @id_name, @data, @read_, @id_name_get)";
                comm.Parameters.AddWithValue("message", p.message);         //смс
                comm.Parameters.AddWithValue("id_name", p.id_name);         //id отправителя
                comm.Parameters.AddWithValue("data", p.data);         //дата
                comm.Parameters.AddWithValue("read_", p.read_);               //смс не прочитано
                comm.Parameters.AddWithValue("id_name_get", p.id_name_get); //id получателя

                comm.CommandType = CommandType.Text;
                conn.Open();
                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteLine("Server ERROR: " + ex.Message);
                return 0;
            }
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
