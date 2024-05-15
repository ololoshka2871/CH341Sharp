using CH341Sharp;
using LibUsbDotNet;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
	[TestFixture]
	public class TestEnumerator
	{
		#region Methods

		[Test]
		public void Enumerate()
		{
			var result = Enumerator.Enumerate();
			Assert.NotNull(result);
			Assert.IsTrue(result.Any());
		}

		[Test]
		public void OpenTwice()
		{
			var ch341 = Enumerator.Enumerate().First();

			Assert.True(ch341.Open(out UsbDevice d1));
			Assert.False(ch341.Open(out UsbDevice d2));

			Assert.NotNull(d1);
			Assert.Null(d2);

			d1.Close();
		}

		#endregion Methods
	}
}