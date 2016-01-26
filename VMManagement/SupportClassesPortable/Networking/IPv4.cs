using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportClassesPortable.Networking
{
    public class IPAddressNotValid : Exception
    {
        public IPAddressNotValid()
        {
        }

        public IPAddressNotValid(string message) : base(message)
        {
        }

        public IPAddressNotValid(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
    public class IPv4
    {
        public int a;
        public int b;
        public int c;
        public int d;

        public IPv4(string address)
        {
            string[] x = address.Split('.');

            if (x.Count() != 4)
            {
                throw new IPAddressNotValid("IPv4 address not valid" + address);
            }
            else
            {
                try
                {
                    this.a = Int32.Parse(x[0]);
                    this.b = Int32.Parse(x[1]);
                    this.c = Int32.Parse(x[2]);
                    this.d = Int32.Parse(x[3]);
                }
                catch
                {
                    throw new IPAddressNotValid("IPv4 address not valid" + address);
                }
            }
        }

        public IPv4(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public string GetString()
        {
            return this.a + "." + this.b + "." + this.c + "." + this.d;
        }
    }

    public static class AddressValidation
    {
        public static bool IPv4(int a, int b, int c, int d)
        {
            if (126 < a && a <= 255)
            {
                if (0 <= b && b <= 255)
                {
                    if (0 <= c && c <= 255)
                    {
                        if (0 <= d && d <= 255)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    

    public class NetworkAddressIPv4
    {
        IPv4 address;
        IPv4 gateway;
        IPv4 dns1;
        IPv4 dns2;

        int NetMask;

        public void SetAddress(string address)
        {
            this.address = new IPv4(address);
        }

        public void SetAddress(IPv4 address)
        {
            this.address = address;
        }

        public void SetAddress(int a, int b, int c, int d)
        {
            this.address = new IPv4(a, b, c, d);
        }

        public void SetGateway(string gateway)
        {
            this.gateway = new IPv4(gateway);
        }

        public void SetGateway(IPv4 gateway)
        {
            this.gateway = gateway;
        }

        public void SetGateway(int a, int b, int c, int d)
        {
            this.gateway = new IPv4(a, b, c, d);
        }

        private bool isValidGateway(int a, int b, int c, int d)
        {
            if (a < 0 || b < 0 || c < 0 || d < 0 || a > 255 || b > 255 || c > 255 || d > 255)
                return false;

            if(a < 255)
            {
                if(b != 0 || c != 0 || d != 0)
                {
                    return false;
                }
            }
            else if(a == 255)
            {
                if(b < 255)
                {
                    if (c != 0 || d != 0)
                    {
                        return false;
                    }
                }
                else if (b == 255)
                {
                    if(c < 255)
                    {
                        if( d != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void SetNetMask(int netmask)
        {
            this.NetMask = netmask;
        }

        public void SetNetMask(int a, int b, int c, int d)
        {
            int tempNetMask = 0;

            if (AddressValidation.IPv4(a, b, c, d) && isValidGateway(a, b, c, d))
            {
                if (a == 255)
                {
                    tempNetMask += 8;

                    if (b == 255)
                    {
                        tempNetMask += 8;

                        if (c == 255)
                        {
                            tempNetMask += 8;

                            getRestOfIPAddress(ref d, ref tempNetMask);

                        }
                        else
                        {
                            getRestOfIPAddress(ref c, ref tempNetMask);

                        }

                    }
                    else
                    {
                        getRestOfIPAddress(ref b, ref tempNetMask);
                    }

                }
                else
                {
                    getRestOfIPAddress(ref a, ref tempNetMask);
                }
            }
            else
            {
                throw new IPAddressNotValid("Gateway not valid!");
            }

            this.NetMask = tempNetMask;
        }

        public void SetNetMask(string gateway)
        {
            IPv4 tmp = new IPv4(gateway);

            this.SetNetMask(tmp.a, tmp.b, tmp.c, tmp.d);

            tmp = null;
        }

        private static void getRestOfIPAddress(ref int x, ref int tempNetMask)
        {
            int i = 7;
            while (x > 0)
            {
                x -= (int)Math.Pow(2, i);
                i--;
                tempNetMask++;
            }
            if (x < 0)
                throw new IPAddressNotValid();
        }

        public void SetPrimaryDNS(string dns1)
        {
            this.dns1 = new IPv4(dns1);
        }

        public void SetPrimaryDNS(int a, int b, int c, int d)
        {
            this.dns1 = new IPv4(a, b, c, d);
        }

        public void SetSecondaryDNS(string dns2)
        {
            this.dns2 = new IPv4(dns2);
        }

        public void SetSecondaryDNS(int a, int b, int c, int d)
        {
            this.dns2 = new IPv4(a, b, c, d);
        }

        public IPv4 GetAddress()
        {
            return address;
        }

        public IPv4 GetGateway()
        {
            return gateway;
        }

        public IPv4 GetDNSPrimary()
        {
            return dns1;
        }

        public IPv4 GetDNSSecondary()
        {
            return dns2;
        }

        public int GetNetmask()
        {
            return NetMask;
        }
    }
}
