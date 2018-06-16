using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class HotKeysConfig
	{
		public HotKeysConfig()
		{
			BindHotkeys = true;
			ModKey = ModKeyEnum.Alt;
		}


		[IniComment("when set to true the application will bind hotkeys to desktop-swap actions",
			"The hotkeys are always in the form: <mod>+<number>, where the <mod> is defined below and the ,number. is the number of the desktop you want to swap to")]
		public bool BindHotkeys { get; set; }

		[IniComment("Assigns the <mod> key to be used in the hotkeys",
			"Options: Alt, Ctrl")]
		public ModKeyEnum ModKey { get; set; }

	}

	public enum ModKeyEnum
	{
		None = 0,
		Alt = 1,
		Ctrl = 2
		//Win = 3
	}
}
