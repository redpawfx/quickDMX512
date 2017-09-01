# quickDMX512
Clone of carldebilly https://quickdmx.codeplex.com  A simple C# DMX 512 Framework 

QuickDmx - A simple DMX 512 framework
QuickDmx is a simple framework you can use to extend and quickly create scalable applications who act as Dmx 512 controllers.

DMX 512 is a protocol mainly used in showbusiness who usually allow a single console (the controller) to control lightning of the show.

Over time, the DMX 512 protocol is now able to control more than light intensity. It can control leds with colors, scan robots movements and a lot of accessories like smoke machines and curtains.

The first version of QuickDmx is supporting only one type of Dmx Port, the OpenDmx USB port, who is based on the FTDI USB-to-serial chip.

QuickDMX is designed to work with .NET 4.0 and 4.5. Can be integrated in a Winforms, WPF or even an ASP.NET application. Currently it's not possible to use it from Windows Store App, since they can't access USB or serial ports.

To use it, simply reference the DLL in your project and type those lines of code :

	var port = FtdiUsbDmxPort.GetPorts().FirstOrDefault();
	if (port != null)
	{
		using (var dmxController = port.CreateController())
		{
			// Send some levels to channels 1 to 3
			dmxController[1] = 255;
			dmxController[2] = 180;
			dmxController[3] = 180;
			Thread.Sleep(TimeSpan.FromSeconds(15));
		}
	}


QuickDmx Features :

    Can control any number of Dmx ports, with the ability to match the ports according to their USB Path
    Fully Thread-safe
    Will adapt the Dmx Universe length according to data on it, to ensure the highest DMX refresh rate
    Can report the current refresh rate to app
    State of the art .NET code
    No unsafe code
    Will ensure every channels set to zero before exiting the application


You can use QuickDmx to build those kind of softwares :

    Build any custom automations (for a custom show, by example)
    Build a generic DMX console application
    Build a bridge to control DMX from another source/protocol (TCP/IP, by example)


Important :
DMX 512 is an old protocol. Very efficient, but old too. That means it's slow. You should try to always allocate lower addresses first and keep addresses of higher addresses for resources you're not using very often. If the refresh rate is too slow, you should consider to add another DMX 512 port.

Last edited Jan 27, 2013 at 2:09 PM by carldebilly, version 8
