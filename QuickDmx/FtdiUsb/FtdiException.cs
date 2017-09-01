using System;

namespace Proxymity.QuickDmx.FtdiUsb
{
	public class FtdiException : Exception
	{
		internal FtdiInterop.FtStatus Status;

		internal FtdiException(FtdiInterop.FtStatus status) : base(status.ToString())
		{
			Status = status;
		}

		internal FtdiException(FtdiInterop.FtStatus status, Exception ex) : base(status.ToString(), ex)
		{
			Status = status;
		}
	}
}