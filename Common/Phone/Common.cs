using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Phone
{
    public enum Command
    {
        Invite, //Make a call.
        Bye,    //End a call.
        Busy,   //User busy.
        OK,     //Response to an invite message. OK is sent to 
        //indicate that call is accepted.
        Null   //No command.
    }
}
