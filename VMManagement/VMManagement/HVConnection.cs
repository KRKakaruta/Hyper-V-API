using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HyperVManagement
{
    public static class HVConnection
    {
        public static ManagementScope GetHVScope(string hostName)
        {
            return new ManagementScope(@"\\" + hostName + @"\root\virtualization\v2");
        }

        public static ManagementScope GetHVScope(string hostName, ConnectionOptions connOpt)
        {
            return new ManagementScope(@"\\" + hostName + @"\root\virtualization\v2", connOpt);
        }

        public static ConnectionOptions GetConnectionOptions(string user, string domain, string password)
        {
            return new ConnectionOptions() { Username = user, Authority = "ntlmdomain:" + domain, Password = password };
        }

    }
}
