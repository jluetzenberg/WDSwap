using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using WDSwap.lib;

namespace WDSwap.lib
{
	public class WdSwapManager : IWDSwapManager
	{

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);


		#region interface implementation

		public void ChangeDesktop(int id)
		{
			EnsureDesktopExists(id);
			VirtualDesktop.FromIndex(id - 1).MakeVisible();
		}

		public int Count()
		{
			return VirtualDesktop.GetCount();
		}

		public int GetCurrent()
		{
			return VirtualDesktop.GetIndex(VirtualDesktop.GetCurrent()) + 1;
		}

		public void MoveWindowToDesktop(int id)
		{
			EnsureDesktopExists(id);

			IntPtr hwnd = GetForegroundWindow();
			uint pid;
			GetWindowThreadProcessId(hwnd, out pid);
			Process process = Process.GetProcessById((int)pid);
			IntPtr iParam = process.MainWindowHandle;
			VirtualDesktop.FromIndex(id - 1).MoveWindow(iParam);

			ChangeDesktop(id);
		}

		public IntPtr GetActiveWindow()
		{
			return GetForegroundWindow();
		}

		public bool SetActiveWindow(IntPtr hWnd)
		{
			return SetForegroundWindow(hWnd);
		}

		public void PinWindow(IntPtr intPtr)
		{
			VirtualDesktop.PinWindow(intPtr);
		}

		public void PinWindow(string processName)
		{
			var pid = VirtualDesktop.GetWindowHandle(processName);
			VirtualDesktop.PinApplication((IntPtr)pid);
		}

		#endregion


		private bool DoesDesktopExist(int desktopId)
		{
			//check for presence of the desktop
			var currentCount = VirtualDesktop.GetCount();
			Console.WriteLine($"Current Desktop Count: {currentCount}. Target id: {desktopId}");
			return currentCount >= desktopId;
		}

		private ISet<int> CreateDesktop(int desktopId)
		{
			//If you can create a specific number, do it
			//Else, create all desktops up to the number
			//return list of created desktops
			var result = new HashSet<int>();

			var currentCount = VirtualDesktop.GetCount();
			Console.WriteLine($"Current Desktop Count: {currentCount}. Target id: {desktopId}");
			if (currentCount < (desktopId - 1))
			{
				for (var i = currentCount; i < (desktopId - 1); i++)
				{
					Console.WriteLine($"Creating desktop {i + 1}...");
					VirtualDesktop.GetIndex(VirtualDesktop.Create());
					result.Add(i);
				}
			}
			Console.WriteLine($"Creating desktop {desktopId}...");
			VirtualDesktop.GetIndex(VirtualDesktop.Create());
			result.Add(desktopId);

			return result;
		}

		private void EnsureDesktopExists(int id)
		{
			if (DoesDesktopExist(id) == false)
			{
				Console.WriteLine($"Desktop does not exist. Creating...");
				var createdDesktops = CreateDesktop(id);
				if (createdDesktops.Contains(id) == false)
				{
					throw new Exception("There was an error creating the specified desktop");
				}
			}
		}
	}
}
