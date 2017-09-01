using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxymity.QuickDmx.FtdiUsb;

namespace Proxymity.QuickDmx.Tests
{
	[TestClass]
	public class IntegrationTest
	{
		[TestMethod]
		public void SanityTest()
		{
			var port = FtdiUsbDmxPort.GetPorts().FirstOrDefault();
			if (port != null)
			{
				using (var controller = port.CreateController())
				{
					controller.NewRefreshRate +=
						(snd, evt) => Console.WriteLine("New Refresh Rate: " + evt.UniversesPerSecond + " universes/sec");
					Console.WriteLine(1);
					controller[1] = 255;
					controller[2] = 180;
					controller[30] = 180;
					Thread.Sleep(TimeSpan.FromSeconds(0.1));
					Assert.IsTrue(controller.IsRunning, "Controller not running! (1)");

					Console.WriteLine(2);
					Thread.Sleep(TimeSpan.FromSeconds(5));
					Assert.IsTrue(controller.IsRunning, "Controller not running! (2)");

					Console.WriteLine(3);
					controller[60] = 1;
					Thread.Sleep(TimeSpan.FromSeconds(4));
					Assert.IsTrue(controller.IsRunning, "Controller not running! (3)");

					Console.WriteLine(4);
					controller[90] = 3;
					Thread.Sleep(TimeSpan.FromSeconds(5));
					Assert.IsTrue(controller.IsRunning, "Controller not running! (4)");

					Console.WriteLine(5);
				}
			}
		}
	}
}
