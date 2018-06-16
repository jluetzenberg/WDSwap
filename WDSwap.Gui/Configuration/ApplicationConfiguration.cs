using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class ApplicationConfiguration
	{
		public ApplicationConfiguration()
		{
			MaxDesktops = 5;
			ApplicationHeight = 50;
			DefaultConfigurationEditor = "notepad";
		}

		private int _maxDesktop;

		[IniComment("Controls the number of swappable desktops",
			"This can be any number between 1 and 10, though it is recommended to be at least 2 or else this application has little value")]
		public int MaxDesktops
		{
			get { return _maxDesktop; }
			set
			{
				if (value <= 10)
					_maxDesktop = value;
			}
		}

		[IniComment("This setting controls the height of the application, measured in pixels")]
		public int ApplicationHeight { get; set; }

		[IniComment("This setting controls what application is launched when you select the 'configure' option from the context menu")]
		public string DefaultConfigurationEditor { get; set; }
	}
}
