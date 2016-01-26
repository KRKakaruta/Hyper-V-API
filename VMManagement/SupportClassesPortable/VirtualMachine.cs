using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportClassesPortable
{
    enum VMStates
    {
        Unknown = 0,
        Enabled = 2,
        Disabled = 3,
        Paused = 32768,
        Suspended = 32769,
        Snapshooting = 32771,
        Saving = 32773,
        Stopping = 32774,
        Pausing = 32776,
        Resuming = 32777
    }
    public class VirtualMachine
    {
        string name;
        IServer Server;
        VMStates state;

        public VirtualMachine(string name, IServer server)
        {
            this.name = name;
            this.Server = server;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void UpdateState(int state)
        {
            this.state = (VMStates)state;
        }
    }
}
