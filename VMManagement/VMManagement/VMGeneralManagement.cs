using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WMILibrary;

namespace VMManagement
{
    public static class VMGeneralManagement
    {
        /// <summary>
        /// Gets the Msvm_ComputerSystem instance that matches the requested virtual machine name.
        /// </summary>
        /// <param name="name">The name of the virtual machine to retrieve the path for.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The Msvm_ComputerSystem instance.</returns>
        public static ManagementObject GetVirtualMachine(string name, ManagementScope scope)
        {
            return GetVmObject(name, "Msvm_ComputerSystem", scope);
        }

        /// <summary>
        /// Gets the Msvm_ComputerSystem instance that matches the requested virtual machine name.
        /// </summary>
        /// <param name="name">The name of the virtual machine to retrieve the path for.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The Msvm_ComputerSystem instance.</returns>
        public static ManagementObjectCollection GetVirtualMachines(ManagementScope scope)
        {
            string vmQueryWql = string.Format(CultureInfo.InvariantCulture,
                "SELECT * FROM {0} WHERE Caption=\"{1}\"", "Msvm_ComputerSystem", "Virtual Machine");

            SelectQuery vmQuery = new SelectQuery(vmQueryWql);

            using (ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, vmQuery))
                
            {
                ManagementObjectCollection vmCollection = vmSearcher.Get();
                return vmCollection;
            }
        }


        /// <summary>
        /// Gets the Msvm_PlannedComputerSystem instance matching the requested virtual machine name.
        /// </summary>
        /// <param name="name">The name of the virtual machine to retrieve the path for.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The Msvm_PlannedComputerSystem instance.</returns>
        public static ManagementObject GetPlannedVirtualMachine(string name,ManagementScope scope)
        {
            return GetVmObject(name, "Msvm_PlannedComputerSystem", scope);
        }

        /// <summary>
        /// Gets the first virtual machine object of the given class with the given name.
        /// </summary>
        /// <param name="name">The name of the virtual machine to retrieve the path for.</param>
        /// <param name="className">The class of virtual machine to search for.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The instance representing the virtual machine.</returns>
        private static ManagementObject GetVmObject(string name, string className, ManagementScope scope)
        {
            string vmQueryWql = string.Format(CultureInfo.InvariantCulture,
                "SELECT * FROM {0} WHERE ElementName=\"{1}\"", className, name);

            SelectQuery vmQuery = new SelectQuery(vmQueryWql);

            using (ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(scope, vmQuery))
            using (ManagementObjectCollection vmCollection = vmSearcher.Get())
            {
                if (vmCollection.Count == 0)
                {
                    throw new ManagementException(string.Format(CultureInfo.CurrentCulture,
                        "No {0} could be found with name \"{1}\"",
                        className,
                        name));
                }

                //
                // If multiple virtual machines exist with the requested name, return the first 
                // one.
                //
                ManagementObject vm = WMIUtils.GetFirstObjectFromCollection(vmCollection);

                return vm;
            }
        }


        /// <summary>
        /// Gets the virtual machine's configuration settings object.
        /// </summary>
        /// <param name="virtualMachine">The virtual machine.</param>
        /// <returns>The virtual machine's configuration object.</returns>
        public static ManagementObject GetVirtualMachineSettings(ManagementObject virtualMachine)
        {
            using (ManagementObjectCollection settingsCollection =
                    virtualMachine.GetRelated("Msvm_VirtualSystemSettingData", "Msvm_SettingsDefineState",
                    null, null, null, null, false, null))
            {
                ManagementObject virtualMachineSettings =
                    WMIUtils.GetFirstObjectFromCollection(settingsCollection);

                return virtualMachineSettings;
            }
        }

        /// <summary>
        /// Gets the Msvm_ComputerSystem instance that matches the host computer system.
        /// </summary>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The Msvm_ComputerSystem instance for the host computer system.</returns>
        public static ManagementObjectGetHostComputerSystem( ManagementScope scope)
        {
            //
            // The host computer system uses the same WMI class (Msvm_ComputerSystem) as the 
            // virtual machines, so we can simply reuse the GetVirtualMachine with the name
            // of the host computer system.
            //
            return GetVirtualMachine(Environment.MachineName, scope);
        }

        /// <summary>
        /// Gets the Msvm_ComputerSystem instance for the host computer system with name hostName.
        /// </summary>
        /// <param name="hostName">Host computer system name.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The Msvm_ComputerSystem instance for the host computer system.</returns>
        public static ManagementObject GetHostComputerSystem(string hostName, ManagementScope scope)
        {
            //
            // The host computer system uses the same WMI class (Msvm_ComputerSystem) as the 
            // virtual machines, so we can simply reuse the GetVirtualMachine with the name
            // of the host computer system.
            //
            return GetVirtualMachine(hostName, scope);
        }
        

        /// <summary>
        /// Gets the virtual system management service.
        /// </summary>
        /// <param name="scope">The scope to use when connecting to WMI.</param>
        /// <returns>The virtual system management service.</returns>
        public static ManagementObject GetVirtualMachineManagementService(ManagementScope scope)
        {
            using (ManagementClass managementServiceClass =
                new ManagementClass("Msvm_VirtualSystemManagementService"))
            {
                managementServiceClass.Scope = scope;

                ManagementObject managementService =
                    WMIUtils.GetFirstObjectFromCollection(managementServiceClass.GetInstances());

                return managementService;
            }
        }

        /// <summary>
        /// Gets the virtual system management service setting data.
        /// </summary>
        /// <param name="scope">The scope to use when connecting to WMI.</param>
        /// <returns>The virtual system management service settings.</returns>
        public static ManagementObject GetVirtualMachineManagementServiceSettings(ManagementScope scope)
        {
            using (ManagementClass serviceSettingsClass =
                new ManagementClass("Msvm_VirtualSystemManagementServiceSettingData"))
            {
                serviceSettingsClass.Scope = scope;

                ManagementObject serviceSettings =
                    WMIUtils.GetFirstObjectFromCollection(serviceSettingsClass.GetInstances());

                return serviceSettings;
            }
        }

        /// <summary>
        /// Gets the virtual system snapshot service.
        /// </summary>
        /// <param name="scope">The scope to use when connecting to WMI.</param>
        /// <returns>The virtual system snapshot service.</returns>
        public static ManagementObject GetVirtualMachineSnapshotService(ManagementScope scope)
        {
            using (ManagementClass snapshotServiceClass =
                new ManagementClass("Msvm_VirtualSystemSnapshotService"))
            {
                snapshotServiceClass.Scope = scope;

                ManagementObject snapshotService =
                    WMIUtils.GetFirstObjectFromCollection(snapshotServiceClass.GetInstances());

                return snapshotService;
            }
        }
    }
}
