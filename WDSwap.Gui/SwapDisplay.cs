using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDSwap;
using System.Runtime.InteropServices;
using System.IO;
using WDSwap.Gui.Configuration;
using WDSwap.lib;
using System.Threading;
using System.Collections.Concurrent;
using WDSwap.lib.Logging;

namespace WDSwap.Gui
{

	public partial class SwapDisplay : Form
	{
		private WDSwapConfiguration _config;
		private IWDSwapManager _Manager;
		private IDictionary<int, Button> _Buttons;
		private int _currentFocus;
		private int _currentCount;
		private object _refreshButtonLock = new object();
		private System.Threading.Timer _buttonRefreshTimer;
		private const int GripperButtonIndex = 99;
		private IList<Hotkey> _Hotkeys;
		private Dictionary<int, IntPtr> _ActiveWindowsByDesktopIndex;
		private NotifyIcon _NotifyIcon;
		private static ILogger _Logger = new Logger();

		#region interop for click+drag

		const int WS_EX_TOOLWINDOW = 0x00000080;

		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HTCAPTION = 0x2;

		[DllImport("User32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		#endregion


		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams param = base.CreateParams;
				param.ExStyle |= WS_EX_TOOLWINDOW; //this effectively pins the application to all screens
				return param;
			}
		}

		public SwapDisplay()
		{
			InitializeComponent();
			_Manager = new WdSwapManager();
			_Buttons = new Dictionary<int, Button>();
			_ActiveWindowsByDesktopIndex = new Dictionary<int, IntPtr>();
			_Hotkeys = new List<Hotkey>();
			//this.AutoSize = false;
		}

		public void Initialize()
		{
			//DesktopVisualManager.ReloadAllActiveProcesses();



			LoadConfiguration();



			CreateNotifyIcon();

			InitializeUi();
		}

		private void LoadConfiguration()
		{
			_config = ConfigurationLoader.LoadConfiguration();
		}

		private void InitializeUi()
		{
			_currentCount = _Manager.Count();
			_currentFocus = _Manager.GetCurrent();
			ActivateHotkeys();

			IDictionary<int, Image> images = new Dictionary<int, Image>();

			if (_config.Buttons.ButtonType == ButtonTypeEnum.DisplayGraphic)
			{
				var w = SystemInformation.VirtualScreen.Width;
				var h = SystemInformation.VirtualScreen.Height;
				var fakeRect = new Rectangle(0, 0, w, h);
				double scale = ((double)_config.Application.ApplicationHeight / (double)h);
				double imageScale = scale * 0.9;
				//var imageRect = DesktopVisualManager.ScaleRectangle(fakeRect, imageScale);
				var buttonRect = DesktopVisualManager.ScaleRectangle(fakeRect, imageScale);
				_config.Buttons.MinimumButtonWidth = buttonRect.Width;

				//images = DesktopVisualManager.LoadAllDesktopImages(fakeRect,
				//	ColorTranslator.FromHtml(_config.InactiveDesktopColors.Border),
				//	ColorTranslator.FromHtml(_config.InactiveDesktopColors.Background),
				//	ColorTranslator.FromHtml(_config.ActiveDesktopColors.Background),
				//	imageRect.Height,
				//	imageRect.Width);
			}


			for (var i = 1; i <= _config.Application.MaxDesktops; i++)
			{
				//DrawButton(i, _config.Buttons.ButtonType, images);
				DrawButton(i, _config.Buttons.ButtonType);
			}

			if (_config.Buttons.ButtonType == ButtonTypeEnum.DisplayGraphic)
				QueueIconRefresh();

			DrawGripperButton();

			AdjustApplicationSizeAndColor();
			//PatchButtonSizes();

			StartRefreshTimer();
			SetTopMost();


			SetStartLocation();

			//Process process = Process.GetCurrentProcess();
			//_manager.PinWindow(process.Handle);
		}

		private void SetStartLocation()
		{
			this.Location = new Point(0, 0);

			Screen rightmost = Screen.AllScreens[0];

			this.Left = rightmost.WorkingArea.Right - this.Width;
			this.Top = rightmost.WorkingArea.Bottom - this.Height;
		}

		public void AdjustApplicationSizeAndColor()
		{
			var totalButtonWidth = _Buttons.Select(s => s.Value.Width).Sum();

			SuspendLayout();

			//this.Width = totalButtonWidth;
			//this.Height = _config.Application.ApplicationHeight;
			this.Size = new System.Drawing.Size(totalButtonWidth, _config.Application.ApplicationHeight);
			this.BackColor = ColorTranslator.FromHtml(_config.ActiveDesktopColors.Background);

			ResumeLayout();
			//PatchButtonSizes();
		}

		private void PatchButtonSizes()
		{
			if (this.Height > _config.Application.ApplicationHeight)
			{
				foreach (var btn in _Buttons.Values)
				{
					btn.Height = this.Height;
				}
			}

			var totalButtonWidth = _Buttons.Select(s => s.Value.Width).Sum();
			if (this.Width > totalButtonWidth)
			{
				_Buttons[GripperButtonIndex].Width = this.Width - (totalButtonWidth - _Buttons[GripperButtonIndex].Width);
			}
		}


		private void SetTopMost()
		{
			this.TopMost = true;
		}

		private void SetNotTopMost()
		{
			this.TopMost = false;
		}


		private void CreateNotifyIcon()
		{
			NotifyIcon notifyIcon = new NotifyIcon();
			var menu = GetGripperMenu();
			notifyIcon.ContextMenu = menu;
			notifyIcon.Visible = true;
			notifyIcon.Icon = this.Icon;
			notifyIcon.Text = "WDSwap";

			//notifyIcon.BalloonTipTitle = "WDSwap Running";
			//notifyIcon.BalloonTipText = "The WDSwap application is now running.";
			//notifyIcon.ShowBalloonTip(5000);

			_NotifyIcon = notifyIcon;
		}


		private void DeactivateHotkeys()
		{
			foreach (var hk in _Hotkeys)
			{
				hk.Unregister();
			}
			_Hotkeys.Clear();
		}

		private void ActivateHotkeys()
		{
			if (_config.HotKeys.BindHotkeys == false)
				return;


			for (var i = 1; i <= _config.Application.MaxDesktops; i++)
			{
				var hkChangeDesktop = new Hotkey();
				//the key number 49 represents the '1' key.
				hkChangeDesktop.KeyCode = (Keys)(i + 48);

				switch (_config.HotKeys.ModKey)
				{
					case ModKeyEnum.Ctrl:
						hkChangeDesktop.Control = true;
						break;
					//case ModKeyEnum.Win:
					//	hkChangeDesktop.Windows = true;
					//	break;
					case ModKeyEnum.Alt:
					default:
						hkChangeDesktop.Alt = true;
						break;

				}

				var value = i;
				hkChangeDesktop.Pressed += delegate { ButtonCallback(value); };
				hkChangeDesktop.Register(this);
				_Hotkeys.Add(hkChangeDesktop);
			}
			//Hotkey hk = new Hotkey();
			//hk.KeyCode = Keys.1;
			//hk.Windows = true;
			//hk.Pressed += delegate { Console.WriteLine(“Windows + 1 pressed!”); };

			//if (!hk.GetCanRegister(myForm))
			//{ Console.WriteLine(“Whoops, looks like attempts to register will fail or throw an exception, show an error / visual user feedback”); }
			//else
			//{ hk.Register(myForm); }

			//// .. later, at some point
			//if (hk.Registered)
			//{ hk.Unregister(); }
		}

		
		#region refresh timer

		private void StartRefreshTimer()
		{
			_buttonRefreshTimer = new System.Threading.Timer(RefreshTimerCallback, null, 5000, System.Threading.Timeout.Infinite);
		}
	
		private void RefreshTimerCallback(object state)
		{
			try
			{
				RefreshButtons();
			}
			catch (Exception ex)
			{
				_Logger.Error($"Ann error occurred while refreshing the button status: {ex.Message}", ex.ToString());
			}
			finally
			{
				if (_buttonRefreshTimer != null)
				{
					_buttonRefreshTimer.Change(5000, System.Threading.Timeout.Infinite);
				}
			}
		}
		
		private void RefreshButtons()
		{
			lock (_refreshButtonLock)
			{
				var newCount = _Manager.Count();
				var newFocus = _Manager.GetCurrent();

				if (newFocus != _currentFocus)
					ChangeFocus(_currentFocus, newFocus);

				if (newCount != _currentCount)
					ChangeCount(_currentCount, newCount);

				_currentFocus = newFocus;
				_currentCount = newCount;
			}
		}
		
		private void ChangeFocus(int oldFocus, int newFocus)
		{
			var fcolors = _config.FocusedDesktopColors;
			var acolors = _config.ActiveDesktopColors;

			_Buttons[newFocus].BackColor = ColorTranslator.FromHtml(fcolors.Background);
			_Buttons[newFocus].ForeColor = ColorTranslator.FromHtml(fcolors.Foreground);
			_Buttons[newFocus].FlatAppearance.BorderColor = ColorTranslator.FromHtml(fcolors.Border);

			_Buttons[oldFocus].BackColor = ColorTranslator.FromHtml(acolors.Background);
			_Buttons[oldFocus].ForeColor = ColorTranslator.FromHtml(acolors.Foreground);
			_Buttons[oldFocus].FlatAppearance.BorderColor = ColorTranslator.FromHtml(acolors.Border);
		}
		
		private void ChangeCount(int oldCount, int newCount)
		{
			int min = Math.Min(oldCount, newCount);
			int max = Math.Max(oldCount, newCount);
			max = Math.Max(max, _Buttons.Count - 2);

			var colors = oldCount < newCount ? _config.ActiveDesktopColors : _config.InactiveDesktopColors;

			var foreColor = ColorTranslator.FromHtml(colors.Foreground);
			var backColor = ColorTranslator.FromHtml(colors.Background);
			var borderColor = ColorTranslator.FromHtml(colors.Border);

			for (var i = min; i <= max; i++)
			{
				_Buttons[i].BackColor = backColor;
				_Buttons[i].ForeColor = foreColor;
				_Buttons[i].FlatAppearance.BorderColor = borderColor;
			}
		}
		
		private void DisposeTimer()
		{
			if (_buttonRefreshTimer == null)
				return;
			_buttonRefreshTimer.Dispose();
			_buttonRefreshTimer = null;
		}

		#endregion


		#region button setup

		private void DrawButton(int buttonIndex, ButtonTypeEnum buttonType)
		{
			ColorsConfig buttonColors = GetButtonColors(buttonIndex);

			var button = CreateDesktopButton(buttonColors, buttonIndex, buttonType);

			PlaceButton(button);
			_Buttons[buttonIndex] = button;
		}

		private void PlaceButton(Button btn)
		{
			var currentWidth = _Buttons.Select(s => s.Value.Width).Sum();
			btn.Location = new Point(currentWidth, 0);
			this.Controls.Add(btn);
		}

		private Button CreateDesktopButton(ColorsConfig colors, int index, ButtonTypeEnum buttonType)
		{
			ButtonConfig btnConf = _config.GetButtonConfigByIndex(index);

			Button button = new Button();
			button.FlatStyle = FlatStyle.Flat;


			button.ForeColor = ColorTranslator.FromHtml(colors.Foreground);
			button.BackColor = ColorTranslator.FromHtml(colors.Background);

			button.Click += (s, e) => ButtonCallback(index);

			button.Height = _config.Application.ApplicationHeight;
			button.Width = _config.Buttons.MinimumButtonWidth;
			button.Margin = new Padding(_config.Buttons.ButtonMarginSize);


			if (buttonType == ButtonTypeEnum.DisplayGraphic)
			{
				//button.TextAlign = ContentAlignment.TopLeft;
				//button.Text = index.ToString();
				button.Font = new Font(button.Font.FontFamily, (float)(button.Font.SizeInPoints * 0.75));
				button.BackgroundImageLayout = ImageLayout.Center;
			}
			else
			{
				button.FlatAppearance.BorderSize = _config.Buttons.BorderWidth;
				button.FlatAppearance.BorderColor = ColorTranslator.FromHtml(colors.Border);

				button.Text = btnConf.Name;

				var textwidth = TextRenderer.MeasureText(btnConf.Name, button.Font);
				if (button.Width < (textwidth.Width + (2 * _config.Buttons.ButtonMarginSize)))
				{
					button.Width = textwidth.Width + (2 * _config.Buttons.ButtonMarginSize);
				}
			}

			return button;
		}
		
		private ColorsConfig GetButtonColors(int buttonIndex)
		{
			if (buttonIndex == _currentFocus)
				return _config.FocusedDesktopColors;
			if (buttonIndex <= _currentCount)
				return _config.ActiveDesktopColors;
			return _config.InactiveDesktopColors;
		}
		
		private void DrawGripperButton()
		{
			var colors = _config.InactiveDesktopColors;
			Button button = new Button();
			button.FlatStyle = FlatStyle.Flat;
			button.FlatAppearance.BorderSize = _config.Buttons.BorderWidth;
			button.ForeColor = ColorTranslator.FromHtml(colors.Foreground);
			button.BackColor = ColorTranslator.FromHtml(colors.Background);
			button.FlatAppearance.BorderColor = ColorTranslator.FromHtml(colors.Border);
			button.Text = "";
			//button.Click += (s, e) => ButtonCallback(index);
			button.Width = _config.Buttons.GripperButtonWidth;
			button.Height = _config.Application.ApplicationHeight;
			button.MouseDown += GripperMouseDown;

			var menu = GetGripperMenu();
			button.ContextMenu = menu;

			PlaceButton(button);
			_Buttons[GripperButtonIndex] = button;
		}
		
		private ContextMenu GetGripperMenu()
		{
			var contextMenu = new ContextMenu();

			contextMenu.MenuItems.Add("Configure", (s, e) => LaunchConfigEditor());
			contextMenu.MenuItems.Add("Refresh", (s, e) => Refresh());
			contextMenu.MenuItems.Add("Restore Default Configuration", (s, e) => RestoreDefaultConfiguration());
			contextMenu.MenuItems.Add("Quit", (s, e) => this.Close());

			return contextMenu;
		}

		#endregion


		#region button functions


		private void ButtonCallback(int index)
		{
			if (_config.Buttons.ButtonType == ButtonTypeEnum.DisplayGraphic)
			{
				QueueIconRefresh(_currentFocus);
			}
			var activeWindow = _Manager.GetActiveWindow();
			_ActiveWindowsByDesktopIndex[_currentFocus] = activeWindow;

			_Manager.ChangeDesktop(index);
			RefreshButtons();

			IntPtr newActiveWindow;
			if (_ActiveWindowsByDesktopIndex.TryGetValue(index, out newActiveWindow))
				_Manager.SetActiveWindow(newActiveWindow);
		}

		private void GripperMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
			}
		}

		#endregion

		#region context menu actions

		private void LaunchConfigEditor()
		{
			var p = new Process();
			p.StartInfo.FileName = _config.Application.DefaultConfigurationEditor;
			p.StartInfo.Arguments = ConfigurationLoader.ConfigFilePath;
			p.Start();
		}

		private void Refresh()
		{
			DisposeTimer();
			_Buttons.Clear();
			Controls.Clear();
			DeactivateHotkeys();
			LoadConfiguration();

			InitializeUi();
		}

		private void RestoreDefaultConfiguration()
		{
			var areYouSure = MessageBox.Show("Are you sure? This will erase any custom configurations you have set",
									 "Confirm Configuration Reset",
									 MessageBoxButtons.YesNo);

			if (areYouSure == DialogResult.Yes)
			{
				ConfigurationLoader.RestoreDefault();
				Refresh();
			}
		}


		#endregion

		private static object _IconRefreshLock = new object();
		private static bool _IsRefreshingIcons = false;
		private async void QueueIconRefresh()
		{
			if (_IsRefreshingIcons)
				return;
			lock (_IconRefreshLock)
			{
				_IsRefreshingIcons = true;
			}
			Task.Run(() =>
			{
				var w = SystemInformation.VirtualScreen.Width;
				var h = SystemInformation.VirtualScreen.Height;
				var fakeRect = new Rectangle(0, 0, w, h);
				double scale = ((double)_config.Application.ApplicationHeight / (double)h);
				double imageScale = scale * 0.9;
				var imageRect = DesktopVisualManager.ScaleRectangle(fakeRect, imageScale);

				var images = DesktopVisualManager.LoadAllDesktopImages(fakeRect,
					ColorTranslator.FromHtml(_config.InactiveDesktopColors.Border),
					ColorTranslator.FromHtml(_config.InactiveDesktopColors.Background),
					ColorTranslator.FromHtml(_config.ActiveDesktopColors.Background),
					imageRect.Height,
					imageRect.Width);

				foreach (var btnKvp in _Buttons)
				{
					Image newIcon;
					if (images.TryGetValue(btnKvp.Key - 1, out newIcon))
						btnKvp.Value.BackgroundImage = newIcon;
				}

				lock (_IconRefreshLock)
				{
					_IsRefreshingIcons = false;
				}
			});
		}


		private void QueueIconRefresh(int desktopIndex)
		{
			Task.Run(() =>
			{
				var w = SystemInformation.VirtualScreen.Width;
				var h = SystemInformation.VirtualScreen.Height;
				var fakeRect = new Rectangle(0, 0, w, h);
				double scale = ((double)_config.Application.ApplicationHeight / (double)h);
				double imageScale = scale * 0.9;
				var imageRect = DesktopVisualManager.ScaleRectangle(fakeRect, imageScale);

				var image = DesktopVisualManager.LoadDesktopImage(fakeRect,
					desktopIndex - 1,
					ColorTranslator.FromHtml(_config.InactiveDesktopColors.Border),
					ColorTranslator.FromHtml(_config.InactiveDesktopColors.Background),
					ColorTranslator.FromHtml(_config.ActiveDesktopColors.Background),
					imageRect.Height,
					imageRect.Width);

				_Buttons[desktopIndex].BackgroundImage = image;
			});
		}



		private void SwapDisplay_FormClosing(object sender, FormClosingEventArgs e)
		{
			DisposeTimer();
			_NotifyIcon.Visible = false;
		}
	}
}
