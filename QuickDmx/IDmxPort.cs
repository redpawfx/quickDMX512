using System;

namespace Proxymity.QuickDmx
{
	public interface IDmxPort : IDisposable
	{
		/// <summary>
		/// Get the unique USB/port address
		/// </summary>
		/// <remarks>
		/// This represents the "path" of the physical port.
		/// Could be used to uniquely identify a device on a system.
		/// </remarks>
		string PhysicalPortId { get; }

		void Open();

		void Close();

		/// <summary>
		/// Send universe (channels data)
		/// </summary>
		void Send(byte[] universeData);
	}
}