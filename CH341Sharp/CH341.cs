using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;

namespace CH341Sharp
{
	public class CH341 : IDisposable
	{
		#region Fields

		public static readonly int DEFAULT_PID = 0x5512;
		public static readonly int DEFAULT_VID = 0x1a86;
		public static readonly int DefaultI2CTimeout = 10;
		public readonly ReadEndpointID EP_IN = ReadEndpointID.Ep02;
		public readonly WriteEndpointID EP_OUT = WriteEndpointID.Ep02;
		private UsbDevice CH341device;
		private UsbEndpointReader reader;
		private UsbEndpointWriter writer;

		#endregion Fields

		#region Constructors

		public CH341(UsbRegistry registry)
		{
			if (!registry.IsAlive || !registry.Open(out CH341device))
			{
				throw new DeviceNotFoundException(registry.Vid, registry.Pid);
			}
			Constructors_common();
		}

		public CH341() : this(DEFAULT_VID, DEFAULT_PID)
		{
		}

		public CH341(int vid, int pid)
		{
			CH341device = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(vid, pid));
			if (CH341device == null)
			{
				throw new DeviceNotFoundException(vid, pid);
			}
			Constructors_common();
		}

		#endregion Constructors

		#region Properties

		public int I2CTimeout { get; set; } = DefaultI2CTimeout;

		#endregion Properties

		#region Methods

		public void Dispose()
		{
			if (CH341device != null)
			{
				if (CH341device.IsOpen)
				{
					// If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
					// it exposes an IUsbDevice interface. If not (WinUSB) the
					// 'wholeUsbDevice' variable will be null indicating this is
					// an interface of a device; it does not require or support
					// configuration and interface selection.
					IUsbDevice wholeUsbDevice = CH341device as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						// Release interface #0.
						wholeUsbDevice.ReleaseInterface(0);
					}

					CH341device.Close();
				}
				CH341device = null;
			}
		}

		/// <summary>
		/// Use the single byte write style to get an ack bit from writing to an address with no commands.
		/// </summary>
		/// <returns></returns>
		public bool I2C_Detect(byte i2c_addr)
		{
			Write(I2CcommandBuilder.DetectCommand(i2c_addr));
			return IsAck();
		}

		/// <summary>
		/// Just a start bit...
		/// </summary>
		public void I2C_Start()
		{
			Write(I2CcommandBuilder.StartCommand);
		}

		/// <summary>
		///  This doesn't seem to be very reliable :(
		/// </summary>
		public void I2C_Stop()
		{
			Write(I2CcommandBuilder.StopCommand);
		}

		/// <summary>
		/// Requests a read of up to 32 bytes
		/// </summary>
		/// <param name="length">Size to read up to 32</param>
		/// <returns>array of data</returns>
		public byte[] ReadBlock(int length)
		{
			Write(I2CcommandBuilder.ReadCommand(length));

			byte[] rval = new byte[length];
			var err = reader.Read(rval, 0, rval.Length, I2CTimeout, out int transferLength);
			if (err != ErrorCode.Ok || transferLength != length)
			{
				throw new ReadException(err);
			}
			return rval;
		}

		/// <summary>
		/// Set the i2c speed desired
		/// </summary>
		/// <param name="speed_khz">speed: in khz, will round down to 20, 100, 400, 750
		/// 750 is closer to 1000 for bytes, but slower around acks and each byte start.
		/// </param>
		public void SetSpeed(uint speed_khz = 100)
		{
			var command = I2CcommandBuilder.SetSpeedCommand(speed_khz);
			Write(command);
		}

		public override string ToString()
		{
			var devreg = CH341device.UsbRegistryInfo;
			return $"{devreg.FullName} [{devreg.DeviceProperties["EnumeratorName"]}:{devreg.DeviceProperties["BusNumber"]}/{devreg.DeviceProperties["Address"]}]";
		}

		/// <summary>
		/// Write a byte and return the ack bit
		/// </summary>
		/// <param name="bb">byte to write</param>
		/// <returns>true for ack, false for nak</returns>
		public bool WriteByteCheckAck(byte bb)
		{
			Write(I2CcommandBuilder.WriteByteCommand(bb));
			return IsAck();
		}

		private static void SetConfiguration(IUsbDevice wholeUsbDevice)
		{
			if (!ReferenceEquals(wholeUsbDevice, null))
			{
				// This is a "whole" USB device. Before it can be used,
				// the desired configuration and interface must be selected.

				// Select config #1
				wholeUsbDevice.SetConfiguration(1);

				// Claim interface #0.
				wholeUsbDevice.ClaimInterface(0);
			}
		}

		private void Constructors_common()
		{
			if (CH341device.Configs.Count != 1)
			{
				throw new DeviceConfigurationsIncorrect("CH341 mast have only one configuration");
			}
			SetConfiguration(CH341device as IUsbDevice);

			writer = CH341device.OpenEndpointWriter(EP_OUT);
			reader = CH341device.OpenEndpointReader(EP_IN);
		}

		private bool IsAck()
		{
			byte[] rval = new byte[(int)I2CCommands.MAX];
			var err = reader.Read(rval, 0, rval.Length, I2CTimeout, out int transferLength);
			if (err != ErrorCode.Ok || transferLength != 1)
			{
				throw new ReadException(err);
			}
			return (rval[0] & 0x80) == 0;
		}

		private void Write(byte[] data)
		{
			var err = writer.Write(data, I2CTimeout, out int transferLength);
			if (err != ErrorCode.Ok || transferLength != data.Length)
			{
				throw new WriteException(err);
			}
		}

		#endregion Methods
	}
}