using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class ButtonConfig
	{
		public ButtonConfig()
		{

		}

		public ButtonConfig(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
