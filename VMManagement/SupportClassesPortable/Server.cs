using SupportClassesPortable.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace SupportClassesPortable
{
    public enum ServerType
    {
        HyperVv1,
        HyperVv2,
        ESXi
    }
    public interface IServer
    {
        void SetHostName(string name);
        string GetHostName();
        void SetIPv4Address(string address, string gateway, int netMask);
        void SetIPV4Address(NetworkAddressIPv4 ip);
        void SetHypervisorType(ServerType type);
        List<VirtualMachine> GetVirtualMachines();
        VirtualMachine GetVirtualMachine(string name);
        VirtualMachine CreateVirtualMachine(string name);
        void DestroyVirtualMachine(VirtualMachine machine);
        void MigrateVirtualMachine(VirtualMachine machine, IServer destination);
        void UpdateVirtualMachineList();
    }

    public abstract class Server : IServer
    {
        string hostName;
        NetworkAddressIPv4 IPv4Address;

        ServerType hypervisorType;

        protected List<VirtualMachine> virtualMachines;

        public Server()
        {
            this.virtualMachines = new List<VirtualMachine>();
        }

        public Server(string name) : this()
        {
            this.hostName = name;
        }

        public void SetHostName(string name)
        {
            this.hostName = name;
        }

        public string GetHostName()
        {
            return this.hostName;
        }

        public void SetIPv4Address(string address, string gateway = null, int netMask = 0)
        {
            if (this.IPv4Address == null)
                this.IPv4Address = new NetworkAddressIPv4();

            this.IPv4Address.SetAddress(address);

            if (gateway != null)
                this.IPv4Address.SetGateway(gateway);

            if (netMask != 0)
                this.IPv4Address.SetNetMask(netMask);

        }        

        public void SetIPV4Address(NetworkAddressIPv4 ip)
        {
            this.IPv4Address = ip;
        }

        public void SetHypervisorType(ServerType type)
        {
            this.hypervisorType = type;
        }

        public List<VirtualMachine> GetVirtualMachines()
        {
            return virtualMachines;
        }

        public VirtualMachine GetVirtualMachine(string name)
        {
            return virtualMachines.FirstOrDefault((x => x.GetName().Equals(name)));
        }

        public abstract void AddVirtualMachine(VirtualMachine machine);

        public abstract VirtualMachine CreateVirtualMachine(string name);
        
        public abstract void DestroyVirtualMachine(VirtualMachine machine);

        public abstract void MigrateVirtualMachine(VirtualMachine machine, IServer destination);

        public abstract void UpdateVirtualMachineList();
    }
}
