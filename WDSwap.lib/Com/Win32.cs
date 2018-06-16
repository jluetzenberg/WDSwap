using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.lib.Com
{
	public class Win32
	{
		public const int GCL_HICONSM = -34;
		public const int GCL_HICON = -14;

		public const int ICON_SMALL = 0;
		public const int ICON_BIG = 1;
		public const int ICON_SMALL2 = 2;

		public const int WM_GETICON = 0x7F;

		public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size > 4)
				return GetClassLongPtr64(hWnd, nIndex);
			else
				return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
		}

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		// When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string strClassName, string strWindowName);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

		[DllImport("USER32.DLL")]
		public static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr lParam);


		[DllImport("kernel32.dll")]
		public static extern int GetProcessId(IntPtr handle);

		[DllImport("USER32.DLL")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("USER32.DLL")]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("USER32.DLL")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("USER32.DLL")]
		public static extern IntPtr GetShellWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		//WARN: Only for "Any CPU":
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

		
		public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
	}
}
