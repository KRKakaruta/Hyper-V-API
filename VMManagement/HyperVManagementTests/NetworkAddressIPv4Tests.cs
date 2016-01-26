using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportClassesPortable.Networking;

namespace SupportClasses.Networking.Tests
{
    [TestClass()]
    public class NetworkAddressIPv4Tests
    {
        [TestMethod()]
        public void SetNetMaskTest()
        {
            NetworkAddressIPv4 test = new NetworkAddressIPv4();

            test.SetNetMask(255, 255, 255, 0);
            Assert.AreEqual(24, test.GetNetmask());

            test.SetNetMask(128, 0, 0, 0);
            Assert.AreEqual(1, test.GetNetmask());

            test.SetNetMask(255, 128, 0, 0);
            Assert.AreEqual(9, test.GetNetmask());

            test.SetNetMask("255.255.255.0");
            Assert.AreEqual(24, test.GetNetmask());

            test.SetNetMask("128.0.0.0");
            Assert.AreEqual(1, test.GetNetmask());

            test.SetNetMask("255.128.0.0");
            Assert.AreEqual(9, test.GetNetmask());

            try
            {
                test.SetNetMask(127, 0, 0, 0);
                test.SetNetMask(128, 1, 0, 0);
                test.SetNetMask(255, 128, 4, 0);
                test.SetNetMask(255, 255, 3, 0);
                
                Assert.Fail();
            }
            catch (IPAddressNotValid)
            {
                
            }

        }
    }
}