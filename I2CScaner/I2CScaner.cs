using CH341Sharp;
using System;

namespace I2CScaner
{
	internal class Program
	{
		#region Methods

		private static void Main(string[] args)
		{
			Console.WriteLine("Scanning for CH341 devices...");

			var devices = Enumerator.Enumerate();
			if (devices.Count == 0)
			{
				Console.WriteLine("No CH341 devices found!\nAre you correctly LibUSB backend (https://zadig.akeo.ie/)?");
				Environment.Exit(-1);
			}

			foreach (var devreg in devices)
			{
				var ch341 = new CH341(devreg);

				Console.WriteLine($"Using device {ch341.ToString()}...");
				int foud_count = 0;
				for (int i = CH341.I2C_AddressMin; i <= CH341.I2C_AddressMax; ++i)
				{
					if (ch341.I2C_Detect(i))
					{
						++foud_count;
						Console.WriteLine($"Found i2c slave device at address 0x{i:X}");
					}
				}
				Console.WriteLine($"Scan complete, {foud_count} found.");
			}
		}

		#endregion Methods
	}
}