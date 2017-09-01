using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Proxymity.QuickDmx
{
	/// <summary>
	/// Controller who use the FTDI OpenDmx Controller
	/// </summary>
	public class DmxUniverseController : IDmxUniverseController
	{
		private readonly IDmxPort _port;
		private readonly byte[] _buffer = new byte[513];
		private readonly Thread _thread;

		private bool _isDisposed;
		private int _nbChannelsUsed;
		private int _nbResendBeforeCheck;

		private readonly ManualResetEvent _completed = new ManualResetEvent(false);
		private readonly int _nbMinResendToChannel;
		private readonly bool _disposePortWithController;
		private bool _isRunning;

		public byte this[ushort channel]
		{
			get
			{
				if (channel < 1 || channel > 512)
				{
					throw new ArgumentOutOfRangeException("channel", "Must be a value from 1 to 512");
				}
				return _buffer[channel];
			}
			set
			{
				if (channel < 1 || channel > 512)
				{
					throw new ArgumentOutOfRangeException("channel", "Must be a value from 1 to 512");
				}
				_buffer[channel] = value;

				// Check if we must expand the universe to send this channel data
				if (channel > _nbChannelsUsed)
				{
					_nbChannelsUsed = channel;
				}
				// If we set the last channel to zero, it means we must
				// queue a check for universe length.
				// We're not doing it right now, to ensure devices will
				// receive the "zero" values few times before stopping
				// to send data for them.
				if (channel == _nbChannelsUsed && value == 0)
				{
					Interlocked.Exchange(ref _nbResendBeforeCheck, _nbMinResendToChannel);
				}
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
		}

		public event EventHandler<DmxUniverserControllerRateEventArgs> NewRefreshRate;

		/// <summary>
		/// Wrap a port with a controller
		/// </summary>
		/// <param name="port">The port to wrap with this controller</param>
		/// <param name="nbMinResendToChannel">Minimum number of times a zero should be sent to a channel before stopping sending this channel, when the value is zero</param>
		/// <param name="disposePortWithController">If the port should be disposed by the controller</param>
		public DmxUniverseController(IDmxPort port, int nbMinResendToChannel = 6, bool disposePortWithController = false)
		{
			_port = port;
			_nbMinResendToChannel = nbMinResendToChannel;
			_disposePortWithController = disposePortWithController;

			_thread =
				new Thread(ThreadMethod)
				{
					Name = string.Format("{0} - Port '{1}'", GetType().Name, _port.PhysicalPortId),
					IsBackground = true,
					Priority = ThreadPriority.Highest
				};
			_thread.Start();
		}

		private void ThreadMethod()
		{
			try
			{
				var stopWatch = new Stopwatch();

				_isRunning = true;
				_port.Open();
				DateTimeOffset nextRateFresh = DateTimeOffset.Now.AddSeconds(2);
				long nbRefresh = 0;
				stopWatch.Start();
				while (!(_isDisposed && _nbChannelsUsed == 0))
				{
					var universe = new byte[_nbChannelsUsed];
					Array.Copy(_buffer, universe, _nbChannelsUsed);
					_port.Send(universe);

					if (_nbResendBeforeCheck > 0)
					{
						var newNbResendBeforeCheck = Interlocked.Decrement(ref _nbResendBeforeCheck);
						if (newNbResendBeforeCheck == 0)
						{
							CheckUniverseLength();
						}
					}

					nbRefresh++;
					if (DateTimeOffset.Now > nextRateFresh)
					{
						stopWatch.Stop();
						var elapsedTicks = stopWatch.ElapsedTicks;

						ThreadPool.QueueUserWorkItem(
							_ =>
								{
									var newRate = (double)TimeSpan.TicksPerSecond * nbRefresh / elapsedTicks;
									if (NewRefreshRate != null)
									{
										var evt = new DmxUniverserControllerRateEventArgs(newRate);
										try
										{
											NewRefreshRate(this, evt);
										}
										catch
										{
											// catch all to prevent crashing the application
											// if there's a problem in the event handler.
										}
									}
								});

						nextRateFresh = DateTimeOffset.Now.AddSeconds(2.5);
						stopWatch.Restart();
						nbRefresh = 0;
					}
					Thread.Sleep(25);
				}
				_port.Close();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{
				_isRunning = false;
				_completed.Set();
			}
		}

		private void CheckUniverseLength()
		{
			// This method will check if we're sending too much 
			var lastUsedChannel = 0;

			for (var i = _nbChannelsUsed - 1; i > 0; i--)
			{
				if (_buffer[i] == 0)
				{
					continue;
				}

				lastUsedChannel = i;
				break;
			}

			int oldValue = Interlocked.Exchange(ref _nbChannelsUsed, lastUsedChannel);

			// Check for a concurrency error.
			if (oldValue > lastUsedChannel)
			{
				// In this case, we must redo the check.
				CheckUniverseLength();
			}
		}

		public void Dispose()
		{
			_isDisposed = true;

			for (ushort i = 1; i <= _nbChannelsUsed; i++)
			{
				this[i] = 0;
			}

			_completed.WaitOne();

			if (_disposePortWithController)
			{
				_port.Dispose();
			}
		}
	}
}