using System.Runtime.Serialization;

namespace WcfToDb
{
    [DataContract]
    public class Contacts
    {
        [DataMember]
        public int id_name { get; set; } // змінна для зберігання id користувача з бази данних
        [DataMember]
        public string name { get; set; } // змінна для зберігання імені користувача  з бази данних
        [DataMember]
        public string login { get; set; }// змінна для зберігання логіна користувача  з бази данних
        [DataMember]
        public string password { get; set; }// змінна для зберігання пароля користувача  з бази данних
    }
}
