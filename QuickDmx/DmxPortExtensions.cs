namespace Proxymity.QuickDmx
{
	public static class DmxPortExtensions
	{
		public static IDmxUniverseController CreateController(this IDmxPort forPort)
		{
			return new DmxUniverseController(forPort, disposePortWithController: true);
		}
	}
}