using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestServer.Models
{
    public class MainViewModel
    {
        public ClientCollection Clients { get; private set; }
        

        public MainViewModel(ClientCollection clients)
        {
            Clients = clients;
        }
    }
}
