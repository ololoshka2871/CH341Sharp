using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Collections.Generic;

namespace CH341Sharp
{
	public static class Enumerator
	{
		#region Methods

		public static IList<UsbRegistry> Enumerate()
		{
			return Enumerate(CH341.DEFAULT_VID, CH341.DEFAULT_PID);
		}

		public static IList<UsbRegistry> Enumerate(int vid, int pid)
		{
			List<UsbRegistry> result = new List<UsbRegistry>();
			UsbRegDeviceList allDevices = UsbDevice.AllDevices;

			foreach (UsbRegistry usbRegistry in allDevices)
			{
				if (usbRegistry.Vid == vid && usbRegistry.Pid == pid)
				{
					result.Add(usbRegistry);
				}
			}

			return result;
		}

		#endregion Methods
	}
}