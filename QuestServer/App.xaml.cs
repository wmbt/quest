using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
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
        public bool IsClosed { get; set; }
        
        public App()
        {
            //int minThreads = Environment.ProcessorCount * 8;
            // Установим максимальное количество рабочих потоков

            //int a;
            //int b;
            //ThreadPool.GetMaxThreads(out a, out b);
            //ThreadPool.GetMinThreads(out a, out b);
            //ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            
            
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(8, 8);
            Startup += OnStartup;
            Exit += OnExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            var msg = exception.Message + "\r\n" + exception.StackTrace;
            EventLog.WriteEntry("QuestServer", msg);
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (ServiceHost == null) 
                return;
            IsClosed = true;
            ServiceHost.Close();
            ServiceHost = null;
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            Clients = new ClientCollection();
            Provider = new Provider();
            //var service = new NetworkService.QuestService(/*Clients*/);
            ServiceHost = new ServiceHost(typeof(NetworkService.QuestService));
            ServiceHost.Open();
        }

        public static App GetApp()
        {
            return (App) Current;
        }
    }
}
