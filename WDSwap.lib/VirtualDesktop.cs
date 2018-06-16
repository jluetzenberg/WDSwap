//trimmed-down version of the included VirtualDesktop C# file by Markus Scholtes

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using WDSwap.lib.Com;

namespace WDSwap.lib
{

	public class VirtualDesktop
	{
		// Get process id to window handle
		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


		private IVirtualDesktop _VirtualDesktop;

		private VirtualDesktop(IVirtualDesktop desktop)
		{
			this._VirtualDesktop = desktop;
		}


		public override int GetHashCode()
		{
			return _VirtualDesktop.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var desk = obj as VirtualDesktop;
			return desk != null && object.ReferenceEquals(this._VirtualDesktop, desk._VirtualDesktop);
		}

		/// <summary>
		/// Returns the number of desktops
		/// </summary>
		public static int GetCount()
		{
			return DesktopManager.VirtualDesktopManagerInternal.GetCount();
		}

		/// <summary>
		/// Returns current desktop
		/// </summary>
		/// <returns></returns>
		public static VirtualDesktop GetCurrent()
		{
			return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop());
		}

		/// <summary>
		/// Create desktop object from a VirtualDesktop at index 0..Count-1
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static VirtualDesktop FromIndex(int index)
		{
			return new VirtualDesktop(DesktopManager.GetDesktop(index));
		}

		/// <summary>
		/// Creates desktop object on which window <hWnd> is displayed
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		public static VirtualDesktop FromWindow(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();
			Guid id = DesktopManager.VirtualDesktopManager.GetWindowDesktopId(hWnd);
			return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.FindDesktop(ref id));
		}

		/// <summary>
		/// Returns index of desktop object or -1 if not found
		/// </summary>
		/// <param name="desktop"></param>
		/// <returns></returns>
		public static int GetIndex(VirtualDesktop desktop)
		{
			return DesktopManager.GetDesktopIndex(desktop._VirtualDesktop);
		}

		/// <summary>
		/// Create a new desktop
		/// </summary>
		/// <returns></returns>
		public static VirtualDesktop Create()
		{
			return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.CreateDesktop());
		}

		/// <summary>
		/// Destroy desktop and switch to <fallback>
		/// </summary>
		/// <param name="fallback"></param>
		public void Remove(VirtualDesktop fallback = null)
		{
			IVirtualDesktop fallbackdesktop;
			if (fallback == null)
			{
				// if no fallback is given use desktop to the left except for desktop 0.
				VirtualDesktop dtToCheck = new VirtualDesktop(DesktopManager.GetDesktop(0));
				if (this.Equals(dtToCheck))
				{
					// desktop 0: set fallback to second desktop (= "right" desktop)
					DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(_VirtualDesktop, 4, out fallbackdesktop); // 4 = RightDirection
				}
				else
				{
					// set fallback to "left" desktop
					DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(_VirtualDesktop, 3, out fallbackdesktop); // 3 = LeftDirection
				}
			}
			else
			{
				// set fallback desktop
				fallbackdesktop = fallback._VirtualDesktop;
			}

			DesktopManager.VirtualDesktopManagerInternal.RemoveDesktop(_VirtualDesktop, fallbackdesktop);
		}

		public bool IsVisible
		{
			get { return object.ReferenceEquals(_VirtualDesktop, DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop()); }
		}

		public void MakeVisible()
		{
			DesktopManager.VirtualDesktopManagerInternal.SwitchDesktop(_VirtualDesktop);
		}


		/// <summary>
		/// Returns desktop at the left of this one, null if none
		/// </summary>
		/// <returns></returns>
		public VirtualDesktop Left()
		{
			IVirtualDesktop desktop;
			int hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(_VirtualDesktop, 3, out desktop); // 3 = LeftDirection
			if (hr == 0)
				return new VirtualDesktop(desktop);

			return null;
		}

		/// <summary>
		/// Returns desktop at the right of this one, null if none
		/// </summary>
		public VirtualDesktop Right()
		{
			IVirtualDesktop desktop;
			int hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(_VirtualDesktop, 4, out desktop); // 4 = RightDirection
			if (hr == 0)
				return new VirtualDesktop(desktop);

			return null;
		}

		/// <summary>
		/// Move window <hWnd> to this desktop
		/// <para>note that this may not work with windows the application does not own</para>
		/// </summary>
		/// <param name="hWnd"></param>
		public void MoveWindow(IntPtr hWnd)
		{
			int processId;
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			GetWindowThreadProcessId(hWnd, out processId);

			if (System.Diagnostics.Process.GetCurrentProcess().Id == processId)
			{
				// window of process
				try // the easy way (if we are owner)
				{
					DesktopManager.VirtualDesktopManager.MoveWindowToDesktop(hWnd, _VirtualDesktop.GetId());
				}
				catch // window of process, but we are not the owner
				{
					IApplicationView view;
					DesktopManager.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
					DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, _VirtualDesktop);
				}
			}
			else
			{
				// window of other process
				IApplicationView view;
				DesktopManager.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
				DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, _VirtualDesktop);
			}
		}

		/// <summary>
		/// Returns true if window <hWnd> is on this desktop
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		public bool HasWindow(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			return _VirtualDesktop.GetId() == DesktopManager.VirtualDesktopManager.GetWindowDesktopId(hWnd);
		}

		/// <summary>
		/// Returns true if window <hWnd> is pinned to all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		public static bool IsWindowPinned(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			return DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(hWnd.GetApplicationView());
		}

		/// <summary>
		/// pin window <hWnd> to all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		[Obsolete("This method has been shown to fail. It is kept here for reference.")]
		public static void PinWindow(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			var view = hWnd.GetApplicationView();
			if (!DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				// pin only if not already pinned
				DesktopManager.VirtualDesktopPinnedApps.PinView(view);
			}
		}

		/// <summary>
		/// pin window <hWnd> to all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		[Obsolete("This method will likely fail, as its sister method fails. It is kept here for reference.")]
		public static void UnpinWindow(IntPtr hWnd)
		{
			// unpin window <hWnd> from all desktops
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			var view = hWnd.GetApplicationView();
			if (DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(view))
			{
				// unpin only if not already unpinned
				DesktopManager.VirtualDesktopPinnedApps.UnpinView(view);
			}
		}

		/// <summary>
		/// Returns true if application for window <hWnd> is pinned to all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		public static bool IsApplicationPinned(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			return DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(DesktopManager.GetAppId(hWnd));
		}

		/// <summary>
		/// pin application for window <hWnd> to all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		/// [Obsolete("This method will likely fail, as its sister method fails. It is kept here for reference.")]
		public static void PinApplication(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			string appId = DesktopManager.GetAppId(hWnd);
			if (!DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				// pin only if not already pinned
				DesktopManager.VirtualDesktopPinnedApps.PinAppID(appId);
			}
		}

		/// <summary>
		/// unpin application for window <hWnd> from all desktops
		/// </summary>
		/// <param name="hWnd"></param>
		/// [Obsolete("This method will likely fail, as its sister method fails. It is kept here for reference.")]
		public static void UnpinApplication(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
				throw new ArgumentNullException();

			var view = hWnd.GetApplicationView();
			string appId = DesktopManager.GetAppId(hWnd);
			if (DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
			{
				// unpin only if already pinned
				DesktopManager.VirtualDesktopPinnedApps.UnpinAppID(appId);
			}
		}

		/// <summary>
		/// retrieve window handle to process name
		/// </summary>
		/// <param name="ProcessName"></param>
		/// <returns></returns>
		public static IntPtr GetWindowHandle(string ProcessName)
		{
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(ProcessName);
			IntPtr wHwnd = IntPtr.Zero;

			if (processes.Length > 0)
			{
				// process found, get window handle
				wHwnd = processes[0].MainWindowHandle;
			}

			return wHwnd;
		}
	}
}