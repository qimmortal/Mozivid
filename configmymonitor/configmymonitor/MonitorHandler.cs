using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace configmymonitor
{
    class MonitorHandler
    {
        delegate bool MonitorsEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorsEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);
        

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }


        [StructLayout(LayoutKind.Sequential)]
        struct MonitorInfoEx
        {
            public uint size;
            public Rect monitor;
            public Rect work;
            public uint flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            public void Init()
            {
                //this.size = 40 + 2 * 32;
               // this.DeviceName = "EMPTY";
               /// this.size = (uint)Marshal.SizeOf(this);
            }
        }  

        /// <summary>
        /// The struct that contains the display information
        /// </summary>
        public class DisplayInfo
        {
            public string Availability { get; set; }
            public string ScreenHeight { get; set; }
            public string ScreenWidth { get; set; }
            public Rect MonitorArea { get; set; }
            public Rect WorkArea { get; set; }

            public string DeviceName{get; set;}

            public override string ToString()
            {
                return DeviceName + " " + Availability + " wid:" + ScreenWidth + " hei:" + ScreenHeight;
            }
            
        }

        /// <summary>
        /// Collection of display information
        /// </summary>
        public class DisplayInfoCollection : List<DisplayInfo>
        {
        }

        /// <summary>
        /// Returns the number of Displays using the Win32 functions
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public DisplayInfoCollection GetDisplays()
        {
            DisplayInfoCollection col = new DisplayInfoCollection();
            
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,

                delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    MonitorInfoEx mi = new MonitorInfoEx();
                    mi.size = (uint)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, ref mi);
                    if (success)
                    {
                        DisplayInfo di = new DisplayInfo();
                        di.ScreenWidth = (mi.monitor.right - mi.monitor.left).ToString();
                        di.ScreenHeight = (mi.monitor.bottom - mi.monitor.top).ToString();
                        di.MonitorArea = mi.monitor;
                        di.WorkArea = mi.work;
                        di.Availability = mi.flags.ToString();
                        //di.DeviceName = mi.DeviceName;
                        col.Add(di);
                    }
                    else
                        Console.WriteLine("Failed: "+hMonitor.ToString());
                    return true;

                }, IntPtr.Zero);

            return col;
        }//end GetDisplays

    }//end class MonitorHandler
}
