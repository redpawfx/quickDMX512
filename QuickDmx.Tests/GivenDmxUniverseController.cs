using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proxymity.QuickDmx.Tests
{
	[TestClass]
	public class GivenDmxUniverseController
	{
		[TestMethod]
		public void WhenNotChannelSet_ThenUniverseAreSentEmpty()
		{
			var dummyPort = new DummyPort();
			using (var sut = dummyPort.CreateController())
			{
				Thread.Sleep(250);
			}
			Assert.IsTrue(dummyPort.Received.Any(), "No output data");
			Assert.IsTrue(dummyPort.Received.All(x=>x.Length == 0), "All universes should be empty");
		}

		[TestMethod]
		public void WhenOneChannelIsSet_ThenUniverseAreSentWithRightLength()
		{
			var dummyPort = new DummyPort();
			using (var sut = dummyPort.CreateController())
			{
				sut[10] = 255;
				Thread.Sleep(250);
			}
			Assert.IsTrue(dummyPort.Received.Any(), "No output data");
			Assert.IsTrue(dummyPort.Received.Any(x => x.Length != 0), "At least one universe should be not empty");
			Assert.IsTrue(dummyPort.Received.Last().Length == 10, "The last universe sent should be 10 length");
		}

		[TestMethod]
		public void WhenOneChannelIsSetAndBackToZero_ThenUniverseWillReturnToEmpty()
		{
			var dummyPort = new DummyPort();
			using (var sut = new DmxUniverseController(dummyPort, 2))
			{
				Thread.Sleep(10);
				sut[10] = 255;
				Thread.Sleep(100);
				sut[10] = 0;
				Thread.Sleep(175);
			}
			Assert.IsTrue(dummyPort.Received.Any(), "No output data");
			Assert.IsTrue(dummyPort.Received.Any(x => x.Length == 10), "At least one universe should 10 length");
			Assert.IsTrue(dummyPort.Received.Last().Length == 0, "Last universe should be empty");
		}

		internal class DummyPort : IDmxPort
		{
			internal List<byte[]> Received = new List<byte[]>();

			public string PhysicalPortId
			{
				get;
				set;
			}

			public void Open()
			{
			}

			public void Close()
			{
			}

			public void Send(byte[] universeData)
			{
				Received.Add(universeData);
			}

			public void Dispose()
			{
			}
		}
	}
}
