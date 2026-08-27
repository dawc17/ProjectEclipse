using Nekki.SF2.GUI.Common;

namespace Nekki.SF2.GUI
{
	public class DebugUI : ModuleHolder
	{
		public static ConsoleUI CAPIAHDHCOO
		{
			get
			{
				return get_Console();
			}
		}

		public static ModelDebugUI COHMAEHOPLG
		{
			get
			{
				return get_ModelDebug();
			}
		}

		public static ConsoleUI get_Console()
		{
			return UIModule.GetModule<ConsoleUI>();
		}

		public static ModelDebugUI get_ModelDebug()
		{
			return UIModule.GetModule<ModelDebugUI>();
		}
	}
}
