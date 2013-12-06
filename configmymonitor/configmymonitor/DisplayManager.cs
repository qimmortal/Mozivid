using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace configmymonitor
{
    /// <summary>
    /// Represents a wrapper to the native methods.
    /// </summary>
    static class DisplayManager
    {
        public static void AttachSecondMonitor(DisplayDevice dev)
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();

            //ZeroMemory(&mode, sizeof(DEVMODE));

            
            mode.dmBitsPerPel = 32;
            mode.dmPelsWidth = 1920;
            mode.dmPelsHeight = 1200;
            mode.dmPosition.x = -1920;
            mode.dmPosition.y = 0;
            mode.dmDisplayFrequency = 60;
            mode.dmDisplayOrientation = 0;

            mode.dmFields = (uint)(SafeNativeMethods.DM.PelsWidth | SafeNativeMethods.DM.PelsHeight | SafeNativeMethods.DM.BitsPerPixel | 
                                   SafeNativeMethods.DM.Position | SafeNativeMethods.DM.DisplayFrequency | 
                                   SafeNativeMethods.DM.DisplayFlags | SafeNativeMethods.DM.Orientation);


            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            DisplayChangeResult result = (DisplayChangeResult)SafeNativeMethods.ChangeDisplaySettingsEx(dev.DeviceName, ref mode, IntPtr.Zero, (uint)(CDSFlags.CDS_UPDATEREGISTRY | CDSFlags.CDS_NORESET), IntPtr.Zero);

            if (result == 0)
            {
                Console.WriteLine("Success");
                //ChangeDisplaySettingsEx(NULL, NULL, NULL, 0, NULL);
            }
            else
            {
                Console.WriteLine(result.ToString());
            }
        }
        /// <summary>
        /// Returns a DisplaySettings object encapsulates the current display settings.
        /// </summary>
        public static DisplaySettings GetCurrentSettings()
        {
            DisplayDevice d = new DisplayDevice();

            return CreateDisplaySettingsObject(-1, GetDeviceMode(d, true));
        }

        /// <summary>
        /// Changes the current display settings with the new settings provided. May throw InvalidOperationException if failed. Check the exception message for more details.
        /// </summary>
        /// <param name="set">The new settings.</param>
        /// <remarks>
        /// Internally calls ChangeDisplaySettings() native function.
        /// </remarks>
        public static string SetDisplaySettings(DisplaySettings set, DisplayDevice dev, bool primary)
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();
            
            mode.dmPelsWidth = (uint)set.Width;
            mode.dmPelsHeight = (uint)set.Height;
            mode.dmDisplayOrientation = (uint)set.Orientation;
            mode.dmBitsPerPel = (uint)set.BitCount;
            mode.dmDisplayFrequency = (uint)set.Frequency;

            if (primary)
            {
                mode.dmPosition.x = 0;
                mode.dmPosition.y = 0;
            }
            else
            {
                mode.dmPosition.x = -set.Width -1;
                mode.dmPosition.y = 0;
            }
            
            uint flags;

            if (primary)

                flags = (uint)(CDSFlags.CDS_RESET | CDSFlags.CDS_UPDATEREGISTRY | CDSFlags.CDS_SET_PRIMARY );

            else
                flags = (uint)(CDSFlags.CDS_RESET | CDSFlags.CDS_UPDATEREGISTRY);

            mode.dmFields = (uint)(SafeNativeMethods.DM.Position | SafeNativeMethods.DM.PelsHeight | SafeNativeMethods.DM.PelsWidth|
                                    SafeNativeMethods.DM.DisplayFrequency | SafeNativeMethods.DM.DisplayOrientation| 
                                    SafeNativeMethods.DM.BitsPerPixel);

            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            DisplayChangeResult result = (DisplayChangeResult)
            SafeNativeMethods.ChangeDisplaySettingsEx(dev.DeviceName, ref mode, IntPtr.Zero, flags, IntPtr.Zero);

            string msg = "Successfully changed the settings!";
            switch (result)
            {
                case DisplayChangeResult.BadDualView:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadDualView;
                    break;
                case DisplayChangeResult.BadParam:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadParam;
                    break;
                case DisplayChangeResult.BadFlags:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadFlags;
                    break;
                case DisplayChangeResult.NotUpdated:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_NotUpdated;
                    break;
                case DisplayChangeResult.BadMode:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadMode;
                    break;
                case DisplayChangeResult.Failed:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Failed;
                    break;
                case DisplayChangeResult.Restart:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Restart;
                    break;
            }

            return msg;
        }

        public static string enableDevice(DisplayDevice dev)
        {
            SafeNativeMethods.DEVMODE dm = new SafeNativeMethods.DEVMODE();

            SafeNativeMethods.POINTL enabledPosition = new SafeNativeMethods.POINTL();
            enabledPosition.x = -1280;
            enabledPosition.y = 0;

            dm.dmPosition = enabledPosition;
            dm.dmFields = (uint)SafeNativeMethods.DM.Position;

            dm.dmSize = (ushort)Marshal.SizeOf(dm);

            DisplayChangeResult result = (DisplayChangeResult)
            SafeNativeMethods.ChangeDisplaySettingsEx(dev.DeviceName, ref dm, IntPtr.Zero, (uint)CDSFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

            string msg = "Successfully enabled the device!";
            switch (result)
            {
                case DisplayChangeResult.BadDualView:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadDualView;
                    break;
                case DisplayChangeResult.BadParam:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadParam;
                    break;
                case DisplayChangeResult.BadFlags:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadFlags;
                    break;
                case DisplayChangeResult.NotUpdated:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_NotUpdated;
                    break;
                case DisplayChangeResult.BadMode:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadMode;
                    break;
                case DisplayChangeResult.Failed:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Failed;
                    break;
                case DisplayChangeResult.Restart:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Restart;
                    break;
            }

            return msg;
        }

        public static string disableDevice(DisplayDevice dev)
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();
            /*
            mode.dmPelsWidth = 0;
            mode.dmPelsHeight = 0;
            mode.dmDisplayOrientation = 0;
            

            uint flags;

            
            flags = (uint)(CDSFlags.CDS_RESET | CDSFlags.CDS_UPDATEREGISTRY);

            mode.dmFields = (uint)(SafeNativeMethods.DM.PelsHeight | SafeNativeMethods.DM.PelsWidth);
            */
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            DisplayChangeResult result = (DisplayChangeResult)
            SafeNativeMethods.ChangeDisplaySettingsEx(dev.DeviceName, 0,0, 0, 0);

            string msg = "Successfully disabled the device!";
            switch (result)
            {
                case DisplayChangeResult.BadDualView:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadDualView;
                    break;
                case DisplayChangeResult.BadParam:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadParam;
                    break;
                case DisplayChangeResult.BadFlags:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadFlags;
                    break;
                case DisplayChangeResult.NotUpdated:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_NotUpdated;
                    break;
                case DisplayChangeResult.BadMode:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadMode;
                    break;
                case DisplayChangeResult.Failed:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Failed;
                    break;
                case DisplayChangeResult.Restart:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Restart;
                    break;
            }

            return msg;
        }

        /// <summary>
        /// Enumerates all supported display modes.
        /// </summary>
        /// <remarks>
        /// Internally calls EnumDisplaySettings() native function.
        /// Because of the nature of EnumDisplaySettings() it calls it many times and uses the <code>yield return</code> statement to simulate an enumerator.
        /// </remarks>
        public static IEnumerator<DisplaySettings> GetModesEnumerator(string devName)
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();

            mode.Initialize();
           // mode.dmDeviceName = devName;
            

            int idx = 0;
            

            while (SafeNativeMethods.EnumDisplaySettingsEx(devName, idx, ref mode, 0))
            {
                
                yield return CreateDisplaySettingsObject(idx++, mode);
                
            }
        }
        //my code start
        /// <summary>
        /// Enumerates all display devices.
        /// </summary>
        /// <remarks>
        /// Internally calls EnumDisplaySettings() native function.
        /// Because of the nature of EnumDisplaySettings() it calls it many times and uses the <code>yield return</code> statement to simulate an enumerator.
        /// </remarks>
        public static IEnumerator<DisplayDevice> GetDisplayDevicesEnumerator()
        {
            SafeNativeMethods.DISPLAY_DEVICE dev = new SafeNativeMethods.DISPLAY_DEVICE();

            //dev.Initialize();
            dev.cb = Marshal.SizeOf(dev);

            int idx = 0;

            while (SafeNativeMethods.EnumDisplayDevices(null, (uint)idx, ref dev, 0))
            {
                yield return CreateDisplayDeviceObject(idx++, dev);

                dev.cb = Marshal.SizeOf(dev);
            }
        }

        public struct DisplayDevice
        {
            public int Index { get; set; }
            public int Cb { get; set; }
            public string DeviceName { get; set; }
            public string DeviceString { get; set; }
            public SafeNativeMethods.DisplayDeviceStateFlags StateFlags { get; set; }
            public string DeviceID { get; set; }
            public string DeviceKey { get; set; }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}", Index, Cb,
                    DeviceName, DeviceString, StateFlags.ToString(), DeviceID, DeviceKey);
            }
        }

        /// <summary>
        /// A private helper methods used to derive a DisplaySettings object from the DEVMODE structure.
        /// </summary>
        /// <param name="idx">The mode index attached with the settings. Starts form zero. Is -1 for the current settings.</param>
        /// <param name="mode">The current DEVMODE object represents the display information to derive the DisplaySettings object from.</param>
        private static DisplayDevice CreateDisplayDeviceObject(int idx, SafeNativeMethods.DISPLAY_DEVICE dev)
        {
            return new DisplayDevice()
            {
                Index = idx,
                Cb = dev.cb,
                DeviceName = dev.DeviceName,
                DeviceString = dev.DeviceString,
                StateFlags = dev.StateFlags,
                DeviceID = dev.DeviceID,
                DeviceKey = dev.DeviceKey
            };
        }
        

        //--------------------------------------------------

        /// <summary>
        /// Rotates the screen from its current location by 90 degrees either clockwise or anti-clockwise.
        /// </summary>
        /// <param name="clockwise">Set to true to rotate the screen 90 degrees clockwise from its current location, or false to rotate it anti-clockwise.</param>
        public static void RotateScreen(bool clockwise, DisplayDevice dev)
        {
            DisplaySettings set = DisplayManager.GetCurrentSettings();

            int tmp = set.Height;
            set.Height = set.Width;
            set.Width = tmp;

            if (clockwise)
                set.Orientation++;
            else
                set.Orientation--;

            if (set.Orientation < Orientation.Default)
                set.Orientation = Orientation.Clockwise270;
            else if (set.Orientation > Orientation.Clockwise270)
                set.Orientation = Orientation.Default;

            SetDisplaySettings(set, dev, true);
        }


        /// <summary>
        /// A private helper methods used to derive a DisplaySettings object from the DEVMODE structure.
        /// </summary>
        /// <param name="idx">The mode index attached with the settings. Starts form zero. Is -1 for the current settings.</param>
        /// <param name="mode">The current DEVMODE object represents the display information to derive the DisplaySettings object from.</param>
        private static DisplaySettings CreateDisplaySettingsObject(int idx, SafeNativeMethods.DEVMODE mode)
        {
            return new DisplaySettings()
            {
                Index = idx,
                Width = (int)mode.dmPelsWidth,
                Height = (int)mode.dmPelsHeight,
                Orientation = (Orientation)mode.dmDisplayOrientation,
                BitCount = (int)mode.dmBitsPerPel,
                Frequency = (int)mode.dmDisplayFrequency,
                DeviceName = mode.dmDeviceName
            };
        }

        /// <summary>
        /// A private helper method used to retrieve current display settings as a DEVMODE object.
        /// </summary>
        /// <remarks>
        /// Internally calls EnumDisplaySettings() native function with the value ENUM_CURRENT_SETTINGS (-1) to retrieve the current settings.
        /// </remarks>
        private static SafeNativeMethods.DEVMODE GetDeviceMode(DisplayDevice dev, bool def)
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();

            mode.Initialize();

            if (def)
            {
                if (SafeNativeMethods.EnumDisplaySettings(null, SafeNativeMethods.ENUM_CURRENT_SETTINGS, ref mode))
                    return mode;
                else
                    //throw new InvalidOperationException(GetLastError());
                    throw new InvalidOperationException("Hello");
            }
            else
            {

                //if (SafeNativeMethods.EnumDisplaySettings(dev.DeviceName, 0, ref mode))
                  if(SafeNativeMethods.EnumDisplaySettingsEx(dev.DeviceName, -2, ref mode, (uint)CDSFlags.CDS_RESET) )
                    return mode;
                else
                    //throw new InvalidOperationException(GetLastError());
                    throw new InvalidOperationException("Hello1");
            }
        }

        private static string GetLastError()
        {
            //int err = Marshal.GetLastWin32Error();
            int err = Marshal.GetLastWin32Error();

            string msg;

            if (SafeNativeMethods.FormatMessage(SafeNativeMethods.FORMAT_MESSAGE_FLAGS, SafeNativeMethods.FORMAT_MESSAGE_FROM_HMODULE, (uint)err, 0, out msg, 0, 0) == 0)
                return Properties.Resources.InvalidOperation_FatalError;
            else
                return msg;
        }
    }

    enum Orientation
    {
        Default = 0,
        Clockwise90 = 1,
        Clockwise180 = 2,
        Clockwise270 = 3
    }

    struct DisplaySettings
    {
        public int Index { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Orientation Orientation { get; set; }
        public int BitCount { get; set; }
        public int Frequency { get; set; }
        public string DeviceName { get; set; }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                "{0}. {1} by {2}, {3}, {4} Bit, {5} Hertz, {6}", Index,
                Width, Height, (int)Orientation, BitCount, Frequency, DeviceName);
        }
    }
    enum CDSFlags
    {
        CDS_RESET = 0x40000000,
        CDS_UPDATEREGISTRY = 0x00000001,
        CDS_SET_PRIMARY = 0x00000010,
        CDS_NORESET = 0x10000000
    };

    /*
    enum DisplayChangeFlags
    {
        CDS_UPDATEREGISTRY = 0x1,
        CDS_TEST = 0x2,
        CDS_FULLSCREEN = 0x4,
        CDS_GLOBAL = 0x8,
        CDS_SET_PRIMARY = 0x10,
        CDS_RESET = 0x40000000,
        CDS_SETRECT = 0x20000000,
        CDS_NORESET = 0x10000000
    }*/

    enum DisplayChangeResult
    {
        /// <summary>
        /// he settings change was unsuccessful because system is DualView capable.
        /// </summary>
        BadDualView = -6,
        /// <summary>
        /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
        /// </summary>
        BadParam = -5,
        /// <summary>
        /// An invalid set of flags was passed in.
        /// </summary>
        BadFlags = -4,
        /// <summary>
        /// Windows NT/2000/XP: Unable to write settings to the registry.
        /// </summary>
        NotUpdated = -3,
        /// <summary>
        /// The graphics mode is not supported.
        /// </summary>
        BadMode = -2,
        /// <summary>
        /// The display driver failed the specified graphics mode.
        /// </summary>
        Failed = -1,
        /// <summary>
        /// The settings change was successful.
        /// </summary>
        Successful = 0,
        /// <summary>
        /// The computer must be restarted in order for the graphics mode to work.
        /// </summary>
        Restart = 1
    }
}
