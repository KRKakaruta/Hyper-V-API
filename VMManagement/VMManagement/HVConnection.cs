using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace VMManagement
{
    public static class HVConnection
    {
        public static ManagementScope GetHVScope(string hostName)
        {
            return new ManagementScope(@"\\" + hostName + "\root\virtualization");
        }
        
        
    }
}
