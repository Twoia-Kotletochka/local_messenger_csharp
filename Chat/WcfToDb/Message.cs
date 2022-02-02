using System;
using System.Runtime.Serialization;

namespace WcfToDb
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public int id { get; set; } // змінна для зберігання id повідомлення з бази данних
        [DataMember]
        public string message { get; set; } // змінна для зберігання тексту повідомлення з бази данних;
        [DataMember]
        public int id_name { get; set; } // змінна для зберігання id користувача з бази данних;
        [DataMember]
        public DateTime data { get; set; } // змінна типу DateTime зберігання часу відправки повідомлення;
        [DataMember]
        public bool read_ { get; set; } // змінна типу bool для перевірки чи прочитане данне повідомлення;
        [DataMember]
        public int id_name_get { get; set; } // змінна для зберігання id користувача котрий відправив повідомлення;
    }
}
