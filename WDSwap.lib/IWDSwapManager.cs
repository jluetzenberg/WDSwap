using System;

namespace WDSwap.lib
{
	public interface IWDSwapManager
	{
		/// <summary>
		/// Changes to the Virtual Desktop with the given Id
		/// <para>If the desktop with that Id does not exist, it will be created</para>
		/// </summary>
		/// <param name="id">The id of the target desktop</param>
		void ChangeDesktop(int id);

		/// <summary>
		/// Returns the current count of Virtual Desktops
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>
		/// Returns the Id of the currently focussed desktop
		/// </summary>
		/// <returns></returns>
		int GetCurrent();

		/// <summary>
		/// Moves the window to the target desktop id
		/// <para>This method has been shown to cause errors</para>
		/// </summary>
		/// <param name="id"></param>
		[Obsolete("This method has not yet been shown to work")]
		void MoveWindowToDesktop(int id);

		/// <summary>
		/// Pins the window defined by its handle to all desktops
		/// <para>This method has been shown to cause errors</para>
		/// </summary>
		/// <param name="intPtr"></param>
		[Obsolete("This method has not yet been shown to work")]
		void PinWindow(IntPtr intPtr);

		/// <summary>
		/// Pins the window defined by its process name to all desktops
		/// <para>This method has been shown to cause errors</para>
		/// </summary>
		/// <param name="processName"></param>
		[Obsolete("This method has not yet been shown to work")]
		void PinWindow(string processName);

		/// <summary>
		/// Returns  the handle for the currently in-focus window
		/// </summary>
		/// <returns></returns>
		IntPtr GetActiveWindow();

		/// <summary>
		/// Gives focus to the window specified by its handle
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		bool SetActiveWindow(IntPtr hWnd);
	}
}

