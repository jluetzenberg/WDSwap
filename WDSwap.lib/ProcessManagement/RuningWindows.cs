using System;
using System.Runtime.InteropServices;
using System.Management;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDSwap.lib.Com;

namespace WDSwap.lib.ProcessManagement
{
	public static class RuningWindows
	{

		#region public methods

		public static IList<WindowInfo> GetOpenedWindows(int? forDesktopId = null)
		{
			IntPtr shellWindow = Win32.GetShellWindow();
			IList<WindowInfo> windows = new List<WindowInfo>();
			int zOrder = 0;

			

			Win32.EnumWindows(new Win32.EnumWindowsProc(delegate (IntPtr hWnd, int lParam)
			{

				if (hWnd == shellWindow)
					return true;
				if (Win32.IsWindowVisible(hWnd) == false)
					return true;

				var info = GetFromHandle(hWnd, forDesktopId);
				if (info == null)
					return true;

				info.ZOrder = zOrder++;
				windows.Add(info);

				return true;
			}), 0);
			return windows;
		}



		#endregion

		#region private methods

		private static string GetProcessPath(IntPtr hwnd)
		{
			
			uint uintProcessId;
			var processId = Win32.GetWindowThreadProcessId(hwnd, out uintProcessId);
			var wmiQueryString = $"SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = {uintProcessId}";

			using (var searcher = new ManagementObjectSearcher(wmiQueryString))
			{
				using (var results = searcher.Get())
				{
					var result = results.Cast<ManagementObject>()
						.Select(s => (string)s["ExecutablePath"])
						.FirstOrDefault();
					return result;
				}
			}
		}

		private static WindowInfo GetFromHandle(IntPtr handle, int? targetDesktopId = null)
		{

			string path = GetProcessPath(handle);
			if (string.IsNullOrEmpty(path))
				return null;

			int length = Win32.GetWindowTextLength(handle);
			if (length == 0)
				return null;

			int processDesktopId = -1;
			try
			{
				var virtualDesktop = VirtualDesktop.FromWindow(handle);
				if (virtualDesktop != null)
					processDesktopId = VirtualDesktop.GetIndex(virtualDesktop);
			}
			catch (Exception ex)
			{
				//Errors are expected
				//there are some system processes that aren't running on a 'Desktop'
			}

			if (processDesktopId < 0 || (targetDesktopId.HasValue && processDesktopId != targetDesktopId.Value))
				return null;

			Icon icon = Icon.ExtractAssociatedIcon(path);
			//Icon icon = GetAppIcon(handle);

			StringBuilder builder = new StringBuilder(length);
			Win32.GetWindowText(handle, builder, length + 1);

			Rect rectangle = new Rect();
			Win32.GetWindowRect(handle, ref rectangle);

			var windowActive = new WindowInfo();
			windowActive.DekstopId = processDesktopId;
			windowActive.Handle = handle;
			windowActive.File = new FileInfo(path);
			windowActive.Title = builder.ToString();
			windowActive.Rectangle = rectangle;
			windowActive.Icon = icon;

			return windowActive;
		}

		#endregion
	}
}
