using System.Linq;

namespace CH341Sharp
{
	/// <summary>
	/// This is just usb standard stuff...
	/// </summary>
	public enum CtrlCommands : byte
	{
		WRITE_TYPE = 0x40,
		READ_TYPE = 0xc0
	}

	/// <summary>
	/// These are all from ch341dll.h, mostly untested
	///
	/// After STA, you can insert MS|millis, and US|usecs to insert a delay
	/// (you can insert multiple)
	/// MS|0 = 250ms wait,
	/// US|0 = ~260usecs?
	/// US|10 is ~10usecs,
	/// be careful, US|20 = MS|4!  US|40 = ? (switched back to 20khz mode)
	/// </summary>
	public enum I2CCommands : byte
	{
		STA = 0x74,
		STO = 0x75,
		OUT = 0x80,
		IN = 0xc0,
		IN_ACK = IN | 1,
		IN_NAK = IN | 0,
		MAX = 32, // min (0x3f, 32) ?! (wrong place for this)
		SET = 0x60, // bit 7 apparently SPI bit order, bit 2 spi single vs spi double
		US = 0x40, // vendor code uses a few of these in 20khz mode?
		MS = 0x50,
		DLY = 0x0f,
		END = 0x00 // Finish commands with this. is this really necessary?
	}

	public enum VendorCommands : byte
	{
		READ_REG = 0x95,
		WRITE_REG = 0x9a,
		SERIAL = 0xa1,
		PRINT = 0xa3,
		MODEM = 0xa4,
		MEMW = 0xa6, // aka mCH341_PARA_CMD_W0
		MEMR = 0xac, // aka mCH341_PARA_CMD_R0
		SPI = 0xa8,
		SIO = 0xa9,
		I2C = 0xaa,
		UIO = 0xab,
		I2C_STATUS = 0x52,
		I2C_COMMAND = 0x53,
		VERSION = 0x5f // at least in serial mode?
	}

	public static class I2CcommandBuilder
	{
		#region Fields

		internal static readonly byte[] StartCommand = new byte[] { (byte)VendorCommands.I2C, (byte)I2CCommands.STA, (byte)I2CCommands.END };
		internal static readonly byte[] StopCommand = new byte[] { (byte)VendorCommands.I2C, (byte)I2CCommands.STO, (byte)I2CCommands.END };

		#endregion Fields

		#region Methods

		public static byte[] SetSpeedCommand(uint speed_khz)
		{
			byte sbit = 1;
			if (speed_khz < 100)
			{
				sbit = 0;
			}
			else if (speed_khz < 400)
			{
				sbit = 1;
			}
			else if (speed_khz < 750)
			{
				sbit = 2;
			}
			else
			{
				sbit = 3;
			}
			return new byte[] { (byte)VendorCommands.I2C, (byte)((byte)I2CCommands.SET | sbit), (byte)I2CCommands.END };
		}

		internal static byte[] DetectCommand(byte i2c_addr)
		{
			return new byte[] { (byte)VendorCommands.I2C, (byte)I2CCommands.STA, (byte)I2CCommands.OUT, (byte)(i2c_addr << 1), (byte)I2CCommands.STO, (byte)I2CCommands.END };
		}

		internal static byte[] ReadCommand(int length)
		{
			return new byte[] { (byte)VendorCommands.I2C,
				(byte)((byte)I2CCommands.IN | (length > 1 ? length : 0)), (byte)I2CCommands.END };
		}

		internal static byte[] WriteByteCommand(byte bb)
		{
			return new byte[] { (byte)VendorCommands.I2C, (byte)I2CCommands.OUT, bb, (byte)I2CCommands.END };
		}

		#endregion Methods
	}
}