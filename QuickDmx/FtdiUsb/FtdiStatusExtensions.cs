namespace Proxymity.QuickDmx.FtdiUsb
{
	internal static class FtdiStatusExtensions
	{
		internal static void Validate(this FtdiInterop.FtStatus status)
		{
			if (status != FtdiInterop.FtStatus.FT_OK)
			{
				throw new FtdiException(status);
			}
		}
	}
}