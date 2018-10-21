using LibUsbDotNet.Main;
using System;

namespace CH341Sharp
{
	class CH341Exception : Exception
	{
		public CH341Exception() : base() { }
		public CH341Exception(string message) : base(message) { }
	}

	class DeviceNotFoundException : CH341Exception
	{
		public readonly int vid = 0x1a86;
		public readonly int pid = 0x5512;

		public DeviceNotFoundException() : base() { }
		public DeviceNotFoundException(int vid, int pid) : base() {
			this.vid = vid;
			this.pid = pid;
		}

		public override string ToString()
		{
			return $"Can't find CH341 device with VID={vid}, PID={pid}";
		}
	}

	class DeviceConfigurationsIncorrect : CH341Exception
	{
		public DeviceConfigurationsIncorrect(string messgae) : base(messgae) { }
	}

	class WriteException : CH341Exception
	{
		public ErrorCode Code { get; }

		public WriteException(ErrorCode code) : base() { this.Code = code; }

		public override string ToString()
		{
			return string.Format("I2C Write exception with code {0}", Code.ToString("D"));
		}
	}

	class CommandException : WriteException
	{
		public CommandException(ErrorCode code) : base(code) { }

		public override string ToString()
		{
			return string.Format("Write Command exception with code {0}", Code.ToString("D"));
		}
	}

	class ReadException : CH341Exception
	{
		public ErrorCode Code { get; }

		public ReadException(ErrorCode code) : base() { this.Code = code; }

		public override string ToString()
		{
			return string.Format("I2C Read exception with code {0}", Code.ToString("D"));
		}
	}
}
