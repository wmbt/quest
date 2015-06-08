using System.ServiceModel;
using System.Windows;
using QuestServer.Models;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        public Provider Provider { get; private set; }
        public ClientCollection Clients { get; private set; }
        public ServiceHost ServiceHost;
        
        public App()
        {
            Startup += OnStartup;
            Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (ServiceHost == null) 
                return;
            
            ServiceHost.Close();
            ServiceHost = null;
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            Clients = new ClientCollection();
            Provider = new Provider();
            var service = new NetworkService.QuestService(Clients);
            ServiceHost = new ServiceHost(service);
            ServiceHost.Open();
        }

        public static App GetApp()
        {
            return (App) Current;
        }
    }
}
