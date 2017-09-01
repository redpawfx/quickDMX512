using System;

namespace Proxymity.QuickDmx
{
	public interface IDmxUniverseController : IDisposable
	{
		/// <summary>
		/// Control the value of each of the 512 channels.
		/// </summary>
		byte this[ushort channel] { get; set; }

		bool IsRunning { get; }

		event EventHandler<DmxUniverserControllerRateEventArgs> NewRefreshRate;
	}

	public class DmxUniverserControllerRateEventArgs : EventArgs
	{
		private readonly double _universesPerSecond;

		public DmxUniverserControllerRateEventArgs(double universesPerSecond)
		{
			_universesPerSecond = universesPerSecond;
		}

		public double UniversesPerSecond
		{
			get { return _universesPerSecond; }
		}
	}
}
