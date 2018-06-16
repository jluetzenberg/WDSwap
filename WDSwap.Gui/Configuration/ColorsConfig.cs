using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class ColorsConfig
	{
		public string Background { get; set; }

		public string Foreground { get; set; }

		public string Border { get; set; }

		public void SelfCheck()
		{
			ColorTranslator.FromHtml(Background);
			ColorTranslator.FromHtml(Foreground);
			ColorTranslator.FromHtml(Border);
		}
	}
}
