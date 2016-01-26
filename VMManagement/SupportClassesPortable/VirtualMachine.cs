using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportClassesPortable
{
    public class VirtualMachine
    {
        string name;
        IServer Server;

        public VirtualMachine(string name, IServer server)
        {
            this.name = name;
            this.Server = server;
        }

        public string GetName()
        {
            return name;
        }
    }
}
