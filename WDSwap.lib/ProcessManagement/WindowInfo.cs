using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDSwap.lib.Com;

namespace WDSwap.lib.ProcessManagement
{
	public class WindowInfo
	{
		public int DekstopId { get; set; }
		public IntPtr Handle { get; set; }
		public FileInfo File { get; set; }
		public string Title { get; set; }
		public Rect Rectangle { get; set; }
		public Icon Icon { get; set; }
		public int ZOrder { get; set; }

		public override string ToString()
		{
			return File.Name + "\t>\t" + Title;
		}
	}
}
