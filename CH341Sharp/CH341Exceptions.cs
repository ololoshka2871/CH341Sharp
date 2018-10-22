using LibUsbDotNet.Main;
using System;

namespace CH341Sharp
{
	public class CH341Exception : Exception
	{
		#region Constructors

		public CH341Exception() : base()
		{
		}

		public CH341Exception(string message) : base(message)
		{
		}

		#endregion Constructors
	}

	public class CommandException : WriteException
	{
		#region Constructors

		public CommandException(ErrorCode code) : base(code)
		{
		}

		#endregion Constructors

		#region Methods

		public override string ToString()
		{
			return string.Format("Write Command exception with code {0}", Code.ToString("D"));
		}

		#endregion Methods
	}

	public class DeviceConfigurationsIncorrect : CH341Exception
	{
		#region Constructors

		public DeviceConfigurationsIncorrect(string messgae) : base(messgae)
		{
		}

		#endregion Constructors
	}

	public class DeviceNotFoundException : CH341Exception
	{
		#region Fields

		public readonly int pid = 0x5512;
		public readonly int vid = 0x1a86;

		#endregion Fields

		#region Constructors

		public DeviceNotFoundException() : base()
		{
		}

		public DeviceNotFoundException(int vid, int pid) : base()
		{
			this.vid = vid;
			this.pid = pid;
		}

		#endregion Constructors

		#region Methods

		public override string ToString()
		{
			return $"Can't find CH341 device with VID={vid}, PID={pid}";
		}

		#endregion Methods
	}

	public class I2CAddressException : CH341Exception
	{
		#region Fields

		public readonly int i2c_address;

		#endregion Fields

		#region Constructors

		public I2CAddressException(int i2c_address) : base()
		{
			this.i2c_address = i2c_address;
		}

		#endregion Constructors

		#region Methods

		public override string ToString()
		{
			return $"Requested operation with incorrect I2C address {i2c_address}";
		}

		#endregion Methods
	}

	public class ReadException : CH341Exception
	{
		#region Constructors

		public ReadException(ErrorCode code) : base()
		{
			this.Code = code;
		}

		#endregion Constructors

		#region Properties

		public ErrorCode Code { get; }

		#endregion Properties

		#region Methods

		public override string ToString()
		{
			return string.Format("I2C Read exception with code {0}", Code.ToString("D"));
		}

		#endregion Methods
	}

	public class WriteException : CH341Exception
	{
		#region Constructors

		public WriteException(ErrorCode code) : base()
		{
			this.Code = code;
		}

		#endregion Constructors

		#region Properties

		public ErrorCode Code { get; }

		#endregion Properties

		#region Methods

		public override string ToString()
		{
			return string.Format("I2C Write exception with code {0}", Code.ToString("D"));
		}

		#endregion Methods
	}
}