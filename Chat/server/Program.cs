using System;
using System.ServiceModel;


namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WcfToDb.Service1)))
            {
                host.Open();
                Console.WriteLine("Сервер запущений!");
                Console.ReadLine();
            }
        }
    }
}
