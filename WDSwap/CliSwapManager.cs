using CLAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDSwap.lib;

namespace WDSwap
{
	public class CliSwapManager : IWDSwapManager
	{
		private IWDSwapManager _internalManager;
		public CliSwapManager()
		{
			_internalManager = new WdSwapManager();
		}

		[Verb(Aliases = "cd")]
		public void ChangeDesktop(int id)
		{
			_internalManager.ChangeDesktop(id);
		}

		//[Verb(Aliases = "mv")]
		public void MoveWindowToDesktop(int id)
		{
			_internalManager.MoveWindowToDesktop(id);
		}

		[Verb(Aliases = "ct")]
		public int Count()
		{
			var result = _internalManager.Count();

			return result;
		}

		[Verb(Aliases = "c")]
		public int GetCurrent()
		{
			var result = _internalManager.GetCurrent();

			return result;
		}

		//[Verb(Aliases = "p")]
		public void PinWindow(IntPtr intPtr)
		{
			_internalManager.PinWindow(intPtr);
		}

		//[Verb(Aliases = "pn")]
		public void PinWindow(string processName)
		{
			_internalManager.PinWindow(processName);
		}

		//[Verb(Aliases = "getactive")]
		public IntPtr GetActiveWindow()
		{
			return _internalManager.GetActiveWindow();
		}

		public bool SetActiveWindow(IntPtr hWnd)
		{
			return _internalManager.SetActiveWindow(hWnd);
		}
	}
}
