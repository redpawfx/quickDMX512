using System;
using System.Runtime.InteropServices;

namespace Proxymity.QuickDmx.FtdiUsb
{
	internal static class FtdiInterop
	{
		[DllImport("FTD2XX.dll", EntryPoint = "FT_Open")]
		internal static extern FtStatus Open(int uiPort, out IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_Close")]
		internal static extern FtStatus Close(IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_Read")]
		internal static extern FtStatus Read(IntPtr ftHandle, IntPtr lpBuffer, int dwBytesToRead, out int lpdwBytesReturned);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_Write")]
		internal static extern FtStatus Write(IntPtr ftHandle, IntPtr lpBuffer, uint dwBytesToWrite, out int lpdwBytesWritten);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_SetDataCharacteristics")]
		internal static extern FtStatus SetDataCharacteristics(IntPtr ftHandle, byte uWordLength, byte uStopBits, byte uParity);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_SetFlowControl")]
		internal static extern FtStatus SetFlowControl(IntPtr ftHandle, ushort usFlowControl, byte uXon, byte uXoff);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_GetModemStatus")]
		internal static extern FtStatus GetModemStatus(IntPtr ftHandle, ref int lpdwModemStatus);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_Purge")]
		internal static extern FtStatus Purge(IntPtr ftHandle, int dwMask);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_ClrRts")]
		internal static extern FtStatus ClrRts(IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_SetBreakOn")]
		internal static extern FtStatus SetBreakOn(IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_SetBreakOff")]
		internal static extern FtStatus SetBreakOff(IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_GetStatus")]
		internal static extern FtStatus GetStatus(IntPtr ftHandle, out int lpdwAmountInRxQueue, out int lpdwAmountInTxQueue, out int lpdwEventStatus);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_ResetDevice")]
		internal static extern FtStatus ResetDevice(IntPtr ftHandle);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_SetDivisor")]
		internal static extern FtStatus SetDivisor(IntPtr ftHandle, char usDivisor);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_CreateDeviceInfoList")]
		internal static extern FtStatus CreateDeviceInfoList(out int nlpdwNumDevs);

		[DllImport("FTD2XX.dll", EntryPoint = "FT_GetDeviceInfoList")]
		internal static extern FtStatus FT_GetDeviceInfoList(
			[In] [Out] FtDeviceListInfoNode[] devices,
			ref int nlpdwNumDevs);

		[Flags]
		internal enum FtDeviceFlags : uint
		{
			None=0,
			Opened = 1
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		internal struct FtDeviceListInfoNode
		{
			internal uint Flags;
			internal uint Type;
			internal uint Id;
			internal uint LocId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			internal string SerialNumber;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			internal string Description;
			internal IntPtr FtHandle;
		}

		internal enum FtStatus : uint
		{
			// ReSharper disable InconsistentNaming
			FT_OK = 0,
			FT_INVALID_HANDLE,
			FT_DEVICE_NOT_FOUND,
			FT_DEVICE_NOT_OPENED,
			FT_IO_ERROR,
			FT_INSUFFICIENT_RESOURCES,
			FT_INVALID_PARAMETER,
			FT_INVALID_BAUD_RATE,
			FT_DEVICE_NOT_OPENED_FOR_ERASE,
			FT_DEVICE_NOT_OPENED_FOR_WRITE,
			FT_FAILED_TO_WRITE_DEVICE,
			FT_EEPROM_READ_FAILED,
			FT_EEPROM_WRITE_FAILED,
			FT_EEPROM_ERASE_FAILED,
			FT_EEPROM_NOT_PRESENT,
			FT_EEPROM_NOT_PROGRAMMED,
			FT_INVALID_ARGS,
			FT_OTHER_ERROR
			// ReSharper restore InconsistentNaming
		};
	}
}