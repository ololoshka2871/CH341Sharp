using CH341Sharp;
using System;
using System.Linq;

namespace BlockReader
{
	class BlockReader
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Scanning for CH341 devices...");

			var devices = Enumerator.Enumerate();
			if (devices.Count == 0)
			{
				Console.WriteLine("No CH341 devices found!\nAre you correctly LibUSB backend (https://zadig.akeo.ie/)?");
				Environment.Exit(-1);
			}

			var dev = new CH341(devices.First());
			byte addr = 0x17;

			dev.I2C_Start();
			dev.WriteByteCheckAck((byte)(addr << 1));
			dev.WriteByteCheckAck(0x80);

			dev.I2C_Start();
			dev.WriteByteCheckAck((byte)((addr << 1) | 1));
			dev.ReadByteAck();
			dev.ReadByteAck();
			dev.ReadByteNak();
			dev.I2C_Stop();
		}
	}
}
