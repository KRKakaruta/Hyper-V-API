using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace VMManagement
{
    class VHDManagement
    {
        /// <summary>
        /// Gets the array of Msvm_StorageAllocationSettingData of VHDs associated with the virtual machine.
        /// </summary>
        /// <param name="virtualMachine">The virtual machine object.</param>
        /// <returns>Array of Msvm_StorageAllocationSettingData of VHDs associated with the virtual machine.</returns>
        public static ManagementObject[] GetVhdSettings(ManagementObject virtualMachine)
        {
            // Get the virtual machine settings (Msvm_VirtualSystemSettingData object).
            using (ManagementObject vssd = VMGeneralManagement.GetVirtualMachineSettings(virtualMachine))
            {
                return GetVhdSettingsFromVirtualMachineSettings(vssd);
            }
        }

        /// <summary>
        /// Gets the array of Msvm_StorageAllocationSettingData of VHDs associated with the given virtual
        /// machine settings.
        /// </summary>
        /// <param name="virtualMachineSettings">A ManagementObject representing the settings of a virtual
        /// machine or snapshot.</param>
        /// <returns>Array of Msvm_StorageAllocationSettingData of VHDs associated with the given settings.</returns>
        public static ManagementObject[] GetVhdSettingsFromVirtualMachineSettings(ManagementObject virtualMachineSettings)
        {
            const UInt16 SASDResourceTypeLogicalDisk = 31;

            List<ManagementObject> sasdList = new List<ManagementObject>();

            //
            // Get all the SASDs (Msvm_StorageAllocationSettingData)
            // and look for VHDs.
            //
            using (ManagementObjectCollection sasdCollection =
                virtualMachineSettings.GetRelated("Msvm_StorageAllocationSettingData",
                    "Msvm_VirtualSystemSettingDataComponent",
                    null, null, null, null, false, null))
            {
                foreach (ManagementObject sasd in sasdCollection)
                {
                    if ((UInt16)sasd["ResourceType"] == SASDResourceTypeLogicalDisk)
                    {
                        sasdList.Add(sasd);
                    }
                    else
                    {
                        sasd.Dispose();
                    }
                }
            }

            if (sasdList.Count == 0)
            {
                return null;
            }
            else
            {
                return sasdList.ToArray();
            }
        }
    }
}
