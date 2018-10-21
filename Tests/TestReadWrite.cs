using CH341Sharp;
using LibUsbDotNet.Main;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
	[TestFixture]
	public class TestReadWrite
	{
		#region Fields

		private readonly UsbRegistry CH341registry;

		#endregion Fields

		#region Constructors

		public TestReadWrite()
		{
			CH341registry = Enumerator.Enumerate().First();
		}

		#endregion Constructors

		#region Methods

		[Test]
		public void Scantest()
		{
			var ch341 = new CH341(CH341registry);

			for (byte i = 0; i < 250; ++i)
			{
				ch341.I2C_Detect(i);
			}
		}

		#endregion Methods
	}
}