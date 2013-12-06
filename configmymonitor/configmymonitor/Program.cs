using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace configmymonitor
{
    class Program
    {
        static void Main(string[] args)
        {

            int loc;
            if ((loc = findString(args, "list")) != -1)
            {
                if (loc + 1 < args.Length && args[loc + 1].StartsWith("v"))
                {
                    int devId = -1;

                    if ((devId = findDevId(args[loc + 1])) != -1)
                    {
                        DisplayManager.DisplayDevice dev = listDevices(devId, false);
                        DisplayManager.DisplayDevice dev1 = default(DisplayManager.DisplayDevice);

                        if (dev.Equals(dev1))
                            Console.WriteLine("No such video device found");
                        else
                        {
                            Console.WriteLine("\nListing all modes on device: " + dev.DeviceString);
                            listModes(dev.DeviceName, true, -1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No such video device found");
                    }

                }
                else
                {
                    Console.WriteLine("\nListing all available video devices on this machine\n");
                    listDevices(-1, true);//list all the devices
                }
            }
            else
                if ((loc = findString(args, "disable")) != -1)
                {
                    if (loc + 1 < args.Length && args[loc + 1].StartsWith("v"))
                    {
                        int devId = -1;

                        if ((devId = findDevId(args[loc + 1])) != -1)
                        {
                            DisplayManager.DisplayDevice dev = listDevices(devId, false);
                            DisplayManager.DisplayDevice dev1 = default(DisplayManager.DisplayDevice);

                            if (dev.Equals(dev1))
                                Console.WriteLine("No such video device found");
                            else
                            {

                                Console.WriteLine(DisplayManager.disableDevice(dev));
                            }
                        }
                        else
                            Console.WriteLine("No such video device found");
                    }
                }
                else
                    if ((loc = findString(args, "enable")) != -1)
                    {
                        if (loc + 1 < args.Length && args[loc + 1].StartsWith("v"))
                        {
                            int devId = -1;

                            if ((devId = findDevId(args[loc + 1])) != -1)
                            {
                                DisplayManager.DisplayDevice dev = listDevices(devId, false);
                                DisplayManager.DisplayDevice dev1 = default(DisplayManager.DisplayDevice);

                                if (dev.Equals(dev1))
                                    Console.WriteLine("No such video device found");
                                else
                                {

                                    Console.WriteLine(DisplayManager.enableDevice(dev));
                                }
                            }
                            else
                                Console.WriteLine("No such video device found");
                        }
                    }
                else
                    if (args.Length > 1)
                    {
                        int devId = -1;
                        int resId = -1;

                        foreach (string s in args)
                        {
                            if (devId == -1)
                                devId = findDevId(s);

                            if (resId == -1)
                                resId = findResId(s);
                        }

                        DisplayManager.DisplayDevice dev = listDevices(devId, false);
                        DisplaySettings set = listModes(dev.DeviceName, false, resId);

                        DisplayManager.DisplayDevice dev1 = default(DisplayManager.DisplayDevice);
                        DisplaySettings set1 = default(DisplaySettings);

                        if (dev.Equals(dev1))
                            Console.WriteLine("No such display device found!");
                        else
                            if (set.Equals(set1))
                                Console.WriteLine("No such display settings found for device {0}!", dev.DeviceString);

                            else
                            {
                                Console.WriteLine(changeSettings(set, dev, findString(args, "primary") != -1));
                            }
                    }
                    else
                        printUsage();

            
        }

        private static void printUsage()
        {
            string nl = "\n";
            string inst = "Usage: "+nl;
            
            inst += "1. To view a list of available devices use parameter \"list\"" + nl;
            inst += "2. To view a list of possible resolutions on a display device use list with additional parameter of device index like \"list v0\""+nl;
            inst += "will display all possible display settings of display device 0" + nl;
            inst += "3. To set a display setting you may use parameters \"r12 v0\" which will set the resolution number 12 on device 0" + nl;
            inst += "4. To enable device \"enable v1\" it will enable display device 1 as listed from fist command"+nl;
            inst += "5. To disable device \"disable v1\" it will disable the display device 1"+nl;



            Console.WriteLine(inst);
        }

        private static int findString(string[] args, string find)
        {
            for(int i=0; i< args.Length; i++)
            {
                if(args[i].Equals(find))
                    return i;
            }

            return -1;
        }

        private static int findDevId(string arg)
        {
            if (arg.StartsWith("v"))
            {
                int res = -1;
                if (int.TryParse(arg.Substring(1), out res))
                {
                    return res;
                }
                else
                    return -1;
            }
            else
                return -1;
        }

        private static int findResId(string arg)
        {
            if (arg.StartsWith("r"))
            {
                int res = -1;
                if (int.TryParse(arg.Substring(1), out res))
                {
                    return res;
                }
                else
                    return -1;
            }
            else
                return -1;
        }


        private static DisplayManager.DisplayDevice listDevices(int id, bool print)
        {
            IEnumerator<DisplayManager.DisplayDevice> enums = DisplayManager.GetDisplayDevicesEnumerator();

            DisplayManager.DisplayDevice dev = new DisplayManager.DisplayDevice();

            int len = 35;
            bool found = false;
            string formatter = "{0,-7}{1,-35}{2,-35}";

            if(print)
               Console.WriteLine(formatter, "Index", "Name", "Status");


            while (enums.MoveNext())
            {
                dev = enums.Current;
                //set.
                string state = dev.StateFlags.ToString();

                if (state.Equals("0"))
                    state = "";


                if(print)
                    Console.WriteLine(formatter, dev.Index, dev.DeviceString.Substring(0, Math.Min(len, dev.DeviceString.Length)), state.Substring(0,Math.Min(len, state.Length)));

                if (dev.Index == id)
                {
                    found = true;
                    break;
                }
            }

            if (found)
                return dev;
            else
                return default(DisplayManager.DisplayDevice);
        }

        private static DisplaySettings listModes(string devName, bool print, int id)
        {
            IEnumerator<DisplaySettings> enumerator = DisplayManager.GetModesEnumerator(@devName);

            DisplaySettings set = default(DisplaySettings);

            bool found = false;
            string formatter = "{0,-7}{1,-20}{2,-10}{3,-15}{4, -15}";
            if (print)
               Console.WriteLine(formatter, "Index", "Resolution", "Bits", "Frequency", "Orientation");

            while (enumerator.MoveNext())
            {
                set = enumerator.Current;

                if (set.Index == id)
                {
                    found = true;
                    break;
                }

                //set.
                if (print)
                    Console.WriteLine(formatter, set.Index, set.Width + " x " + set.Height, set.BitCount, set.Frequency,set.Orientation);
            }

            if (found)
                return set;
            else
                return default(DisplaySettings);
        }

        private static string changeSettings(DisplaySettings set, DisplayManager.DisplayDevice dev, bool prim)
        {
            return DisplayManager.SetDisplaySettings(set, dev, prim);
        }
    }
}
