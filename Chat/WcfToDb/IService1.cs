using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfToDb
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Добавьте здесь операции служб
        [OperationContract]
        int SendMsg(Message p);

        [OperationContract]
        List<Message> GetAllALLMessages();

        [OperationContract]
        int registration(Contacts contacts);

        [OperationContract]
        List<Contacts> GetAllContacts();

        [OperationContract]
        List<Message> GetUNread_Messages(int ID, int ID_get, int COUNT);

        [OperationContract]
        List<Message> GetAllMessages(int ID, int ID_get);

        [OperationContract]
        Dictionary<string, int> READ_CHECK(int ID);

        [OperationContract]
        int GetDATE(int ID, int ID_get, string date1, string date2);
    }

    
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
