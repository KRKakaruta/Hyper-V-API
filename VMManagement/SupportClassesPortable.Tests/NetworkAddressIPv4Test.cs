// <copyright file="NetworkAddressIPv4Test.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportClassesPortable.Networking;

namespace SupportClassesPortable.Networking.Tests
{
    /// <summary>This class contains parameterized unit tests for NetworkAddressIPv4</summary>
    [PexClass(typeof(NetworkAddressIPv4))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class NetworkAddressIPv4Test
    {
        /// <summary>Test stub for SetNetMask(Int32, Int32, Int32, Int32)</summary>
        [PexMethod]
        public void SetNetMaskTest(
            [PexAssumeUnderTest]NetworkAddressIPv4 target,
            int a,
            int b,
            int c,
            int d
        )
        {
            target.SetNetMask(a, b, c, d);
            // TODO: add assertions to method NetworkAddressIPv4Test.SetNetMaskTest(NetworkAddressIPv4, Int32, Int32, Int32, Int32)
        }
    }
}
