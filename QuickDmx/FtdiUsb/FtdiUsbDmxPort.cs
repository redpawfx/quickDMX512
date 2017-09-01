using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Proxymity.QuickDmx.FtdiUsb
{
	public class FtdiUsbDmxPort : IDmxPort
	{
		private readonly FtdiInterop.FtDeviceListInfoNode _portInfo;
		private readonly int _portNo;
		private IntPtr _handle;
		private bool _isOpened;
		private bool _isDisposed;

		/// <summary>
		/// Get the unique USB/port address
		/// </summary>
		/// <remarks>
		/// This represents the "path" of the physical port.
		/// Could be used to uniquely identify a device on a system.
		/// </remarks>
		public string PhysicalPortId
		{
			get
			{
				return _portInfo.LocId.ToString("X", CultureInfo.InvariantCulture);
			}
		}

		private FtdiUsbDmxPort(int portNo, FtdiInterop.FtDeviceListInfoNode portInfo)
		{
			_portNo = portNo;
			_portInfo = portInfo;
		}

		public static FtdiUsbDmxPort[] GetPorts()
		{
			int nbDevices;

			FtdiInterop.CreateDeviceInfoList(out nbDevices).Validate();
			if (nbDevices == 0)
			{
				return new FtdiUsbDmxPort[0];
			}

			var devices = new FtdiInterop.FtDeviceListInfoNode[nbDevices];
			FtdiInterop.FT_GetDeviceInfoList(devices, ref nbDevices).Validate();

			return devices.Select((x, i)=>new FtdiUsbDmxPort(i, x)).ToArray();
		}

		public void Open()
		{
			CheckDisposed();
			if (_handle != IntPtr.Zero)
			{
				throw new InvalidOperationException("Port already opened");
			}

			FtdiInterop.Open(_portNo, out _handle).Validate();
			FtdiInterop.ResetDevice(_handle).Validate();
			FtdiInterop.SetDataCharacteristics(_handle, 8, 2, 0).Validate();
			FtdiInterop.SetFlowControl(_handle, 0, 0, 0).Validate();
			FtdiInterop.ClrRts(_handle).Validate();
			FtdiInterop.Purge(_handle, 1).Validate();
			FtdiInterop.Purge(_handle, 2).Validate();

			_isOpened = true;
		}

		public void Close()
		{
			CheckDisposed();

			FtdiInterop.Close(_handle).Validate();

			_handle = IntPtr.Zero;
		}

		public void Send(byte[] universeData)
		{
			CheckPortOpened();

			// Prepare the buffer to send (MAB + universe)
			var bytesToSend = universeData.Length + 1;
			var toSend = new byte[bytesToSend];
			Array.Copy(universeData, 0, toSend, 1, universeData.Length);

			// Purge any RX packet (useless)
			FtdiInterop.Purge(_handle, 1).Validate();

			// Send the BREAK signal
			FtdiInterop.SetBreakOn(_handle).Validate();
			FtdiInterop.SetBreakOff(_handle).Validate();

			// Send the MARK-AFTER-BREAK (first value with zeros) and the Universe itself
			IntPtr ptr = Marshal.AllocHGlobal(bytesToSend);
			Marshal.Copy(toSend, 0, ptr, bytesToSend);
			int written;
			FtdiInterop.Write(_handle, ptr, (uint)bytesToSend, out written).Validate();
			Marshal.Release(ptr);
		}

		public void Dispose()
		{
			CheckDisposed();
			if (_isOpened)
			{
				FtdiInterop.Close(_handle);
			}
			_isDisposed = true;
		}

		private void CheckDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("Already disposed");
			}
		}

		private void CheckPortOpened()
		{
			if (!_isOpened)
			{
				throw new InvalidOperationException("Port not opened");
			}
		}
	}
}