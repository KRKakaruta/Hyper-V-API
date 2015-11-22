using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Xml;
using System.IO;

namespace WMILibrary
{
    public static class WMIUtils
    {

        enum JobState
        {
            New = 2,
            Starting = 3,
            Running = 4,
            Suspended = 5,
            ShuttingDown = 6,
            Completed = 7,
            Terminated = 8,
            Killed = 9,
            Exception = 10,
            CompletedWithWarnings = 32768
        }
        
        /// <summary>
        /// Validates the output parameters of a method call and prints errors, if any.
        /// </summary>
        /// <param name="outputParameters">The output parameters of a WMI method call.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <param name="throwIfFailed"> If true, the method throws on failure.</param>
        /// <param name="printErrors">If true, Msvm_Error messages are displayed.</param>
        /// <returns><c>true</c> if successful and not firing an alert; otherwise, <c>false</c>.</returns>
        public static bool ValidateOutput(ManagementBaseObject outputParameters, ManagementScope scope, bool throwIfFailed = true, bool printErrors= false)
        {
            bool succeeded = true;
            string errorMessage = "The method call failed.";

            if ((uint)outputParameters["ReturnValue"] == 4096)
            {
                using (ManagementObject job = new ManagementObject((string)outputParameters["Job"]))
                {
                    job.Scope = scope;

                    while (!IsJobComplete(job["JobState"]))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        
                        job.Get();
                    }

                    if (!IsJobSuccessful(job["JobState"]))
                    {
                        succeeded = false;
                        
                        if (!string.IsNullOrEmpty((string)job["ErrorDescription"]))
                        {
                            errorMessage = (string)job["ErrorDescription"];
                        }

                        if (printErrors)
                        {
                            //PrintMsvmErrors(job);
                        }

                        if (throwIfFailed)
                        {
                            throw new ManagementException(errorMessage);
                        }
                    }
                }
            }
            else if ((uint)outputParameters["ReturnValue"] != 0)
            {
                succeeded = false;

                if (throwIfFailed)
                {
                    throw new ManagementException(errorMessage);
                }
            }

            return succeeded;
        }

        /// <summary>
        /// Gets the CIM_ResourcePool derived instance matching the specified type, subtype and
        /// pool id.
        /// </summary>
        /// <param name="resourceType">The resource type of the resource pool.</param>
        /// <param name="resourceSubtype">The resource subtype of the resource pool.</param>
        /// <param name="poolId">The pool id of the resource pool.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The CIM_ResourcePool derived instance.</returns>
        public static ManagementObject GetResourcePool(string resourceType, string resourceSubtype, string poolId, ManagementScope scope)
        {
            string poolQueryWql;

            if (resourceType == "1") // OtherResourceType
            {
                poolQueryWql = string.Format(CultureInfo.InvariantCulture,
                    "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND " +
                    "OtherResourceType=\"{1}\" AND PoolId=\"{2}\"",
                    resourceType, resourceSubtype, poolId);
            }
            else
            {
                poolQueryWql = string.Format(CultureInfo.InvariantCulture,
                    "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND " +
                    "ResourceSubType=\"{1}\" AND PoolId=\"{2}\"",
                    resourceType, resourceSubtype, poolId);
            }

            SelectQuery poolQuery = new SelectQuery(poolQueryWql);

            using (ManagementObjectSearcher poolSearcher = new ManagementObjectSearcher(scope, poolQuery))
            using (ManagementObjectCollection poolCollection = poolSearcher.Get())
            {
                if (poolCollection.Count != 1)
                {
                    throw new ManagementException(string.Format(CultureInfo.CurrentCulture,
                        "A single CIM_ResourcePool derived instance could not be found for " +
                        "ResourceType \"{0}\", ResourceSubtype \"{1}\" and PoolId \"{2}\"",
                        resourceType, resourceSubtype, poolId));
                }

                ManagementObject pool = GetFirstObjectFromCollection(poolCollection);

                return pool;
            }
        }

        /// <summary>
        /// Gets the CIM_ResourcePool derived instances matching the specified type, and subtype.
        /// </summary>
        /// <param name="resourceType">The resource type of the resource pool.</param>
        /// <param name="resourceSubtype">The resource subtype of the resource pool.</param>
        /// <param name="scope">The ManagementScope to use to connect to WMI.</param>
        /// <returns>The CIM_ResourcePool derived instance.</returns>
        public static ManagementObjectCollection GetResourcePools(string resourceType, string resourceSubtype, ManagementScope scope)
        {
            string poolQueryWql;

            if (resourceType == "1") // OtherResourceType
            {
                poolQueryWql = string.Format(CultureInfo.InvariantCulture,
                    "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND " +
                    "OtherResourceType=\"{1}\"",
                    resourceType, resourceSubtype);
            }
            else
            {
                poolQueryWql = string.Format(CultureInfo.InvariantCulture,
                    "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND " +
                    "ResourceSubType=\"{1}\"",
                    resourceType, resourceSubtype);
            }

            SelectQuery poolQuery = new SelectQuery(poolQueryWql);

            using (ManagementObjectSearcher poolSearcher = new ManagementObjectSearcher(scope, poolQuery))
            {
                return poolSearcher.Get();
            }
        }

        /// <summary>
        /// Gets the first object in a collection of ManagementObject instances.
        /// </summary>
        /// <param name="collection">The collection of ManagementObject instances.</param>
        /// <returns>The first object in the collection</returns>
        public static ManagementObject GetFirstObjectFromCollection(ManagementObjectCollection collection)
        {
            if (collection.Count == 0)
            {
                throw new ArgumentException("The collection contains no objects", "collection");
            }

            foreach (ManagementObject managementObject in collection)
            {
                return managementObject;
            }

            return null;
        }

        /// <summary>
        /// Takes a WMI object path and escapes it so that it can be used inside a WQL query WHERE
        /// clause. This effectively means replacing '\' and '"' characters so they are treated
        /// like any other characters.
        /// </summary>
        /// <param name="objectPath">The object management path.</param>
        /// <returns>The escaped object management path.</returns>
        public static string EscapeObjectPath(string objectPath)
        {
            string escapedObjectPath = objectPath.Replace("\\", "\\\\");
            escapedObjectPath = escapedObjectPath.Replace("\"", "\\\"");

            return escapedObjectPath;
        }

        /// <summary>
        /// Verifies whether a job is completed.
        /// </summary>
        /// <param name="jobStateObj">An object that represents the JobState of the job.</param>
        /// <returns>True if the job is completed, False otherwise.</returns>
        private static bool IsJobComplete(object jobStateObj)
        {
            JobState jobState = (JobState)((ushort)jobStateObj);

            return (jobState == JobState.Completed) ||
                (jobState == JobState.CompletedWithWarnings) || (jobState == JobState.Terminated) ||
                (jobState == JobState.Exception) || (jobState == JobState.Killed);
        }

        /// <summary>
        /// Verifies whether a job succeeded.
        /// </summary>
        /// <param name="jobStateObj">An object representing the JobState of the job.</param>
        /// <returns><c>true</c>if the job succeeded; otherwise, <c>false</c>.</returns>
        private static bool IsJobSuccessful(object jobStateObj)
        {
            JobState jobState = (JobState)((ushort)jobStateObj);
            return (jobState == JobState.Completed) || (jobState == JobState.CompletedWithWarnings);
        }
        

    }
}
