using System;
using System.Drawing;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using WDSwap.lib.Com;
using WDSwap.lib.ProcessManagement;

namespace WDSwap.lib
{
	public class DesktopVisualManager
	{

		private static ConcurrentDictionary<int, ConcurrentDictionary<IntPtr, WindowInfo>> _Processes;
		static DesktopVisualManager()
		{
			_Processes = new ConcurrentDictionary<int, ConcurrentDictionary<IntPtr, WindowInfo>>();
		}

		public static IDictionary<int, Image> LoadAllDesktopImages(Rectangle windowRectangle,
			Color borderColor,
			Color bodyColor,
			Color backgroundColor,
			int height, 
			int width)
		{
			ReloadAllActiveProcesses();

			var result = new ConcurrentDictionary<int, Image>();

			height = Scale(height, 0.85);
			width = Scale(width, 0.85);


			Parallel.ForEach(_Processes, (kvp) =>
			{
				result[kvp.Key] = DrawImage(kvp.Value.Values, windowRectangle, borderColor, bodyColor, backgroundColor, height, width);
			});
			//foreach (var kvp in _Processes)
			//{
			//	result[kvp.Key] = DrawImage(kvp.Value.Values, windowRectangle, borderColor, bodyColor, height, width);
			//}

			return result;
		}

		public static Image LoadDesktopImage(Rectangle windowRectangle,
			int desktopId,
			Color borderColor,
			Color bodyColor,
			Color backgroundColor,
			int height,
			int width)
		{
			ReloadDesktopActiveProcesses(desktopId);

			height = Scale(height, 0.85);
			width = Scale(width, 0.85);

			return DrawImage(_Processes[desktopId].Values, windowRectangle, borderColor, bodyColor, backgroundColor, height, width);
		}


		private static void ReloadDesktopActiveProcesses(int desktopId)
		{
			if (_Processes.ContainsKey(desktopId))
				_Processes[desktopId].Clear();
			LoadProcesses(desktopId);
		}

		private static void ReloadAllActiveProcesses()
		{
			//var sw = Stopwatch.StartNew();

			_Processes.Clear();

			LoadProcesses();

			//sw.Stop();
		}

		private static Image DrawImage(ICollection<WindowInfo> processes, 
			Rectangle windowRectangle,
			Color borderColor,
			Color bodyColor,
			Color backgroundColor,
			int height, 
			int width)
		{
			Image image = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
			Graphics drawing = Graphics.FromImage(image);
			foreach (var process in processes.OrderByDescending(o => o.ZOrder))
			{
				DrawWindowGraphic(drawing, process, windowRectangle, borderColor, bodyColor, height, width);
			}

			//var numberRect = new Rectangle(0, 0, Scale(width, 0.1), Scale(height, 0.1));
			//var background = new SolidBrush(backgroundColor);
			//drawing.FillRectangle(background, numberRect);

			return image;
		}

		private static void DrawWindowGraphic(Graphics drawing,
			WindowInfo drawable,
			Rectangle windowRectangle,
			Color borderColor,
			Color bodyColor,
			int height, 
			int width)
		{
			int newLeft = Scale(windowRectangle.Width, width, drawable.Rectangle.Left);
			int newTop = Scale(windowRectangle.Height, height, drawable.Rectangle.Top);

			int newWidth = Scale(windowRectangle.Width, width, drawable.Rectangle.Right - drawable.Rectangle.Left);
			int newHeight = Scale(windowRectangle.Height, height, drawable.Rectangle.Bottom - drawable.Rectangle.Top);

			Rectangle windowRectBorder = new Rectangle(newLeft, newTop, newWidth, newHeight);
			Rectangle windowFillRect = new Rectangle(newLeft + 1, newTop + 1, newWidth - 2, newHeight - 2);
			
			var iconRectangle = new Rectangle(windowRectBorder.Left, windowRectBorder.Top, drawable.Icon.Width, drawable.Icon.Height);
			
			iconRectangle = ScaleRectangle(iconRectangle, windowRectBorder.Width, windowRectBorder.Height);
			iconRectangle = ScaleRectangle(iconRectangle, 0.75);
			iconRectangle = new Rectangle(newLeft + 3, newTop + 3, iconRectangle.Width, iconRectangle.Height);

			Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
			Brush borderBrush = new SolidBrush(bodyColor);

			drawing.DrawRectangle(borderPen, windowRectBorder);
			drawing.FillRectangle(borderBrush, windowFillRect);

			drawing.DrawIcon(drawable.Icon, iconRectangle);

			
		}

		private static void LoadProcesses(int? desktopId = null)
		{
			var processlist = Process.GetProcesses()
				.Where(p => !String.IsNullOrEmpty(p.MainWindowTitle))
				.ToList();

			var items = RuningWindows.GetOpenedWindows(desktopId);

			foreach (var grouping in items.GroupBy(g => g.DekstopId))
			{
				if (grouping.Key < 0)
					continue;
				if (_Processes.ContainsKey(grouping.Key) == false)
					_Processes.TryAdd(grouping.Key, new ConcurrentDictionary<IntPtr, WindowInfo>());
				foreach (var item in grouping)
				{
					_Processes[grouping.Key][item.Handle] = item;
				}
			}
		}

		#region scaling

		public static Rectangle ScaleRectangle(Rectangle rect, int maxWidth, int maxHeight)
		{
			double heightPercentage1 = (double)rect.Height / (double)maxHeight;
			double widthPercentage1 = (double)rect.Width / (double)maxWidth;
			double heightPercentage2 = (double)maxHeight / (double)rect.Height;
			double widthPercentage2 = (double)maxWidth / (double)rect.Width;
			var percentageToScale = (new Double[] { heightPercentage1, widthPercentage1, heightPercentage2, widthPercentage2 }).Min();
			return ScaleRectangle(rect, percentageToScale);
		}

		public static Rectangle ScaleRectangle(Rectangle rect, double percentage)
		{
			int newWidth = Convert.ToInt32(Math.Round(rect.Width * percentage, 0));
			int newHeight = Convert.ToInt32(Math.Round(rect.Height * percentage, 0));

			int newLeft = rect.Left + (int)(0.5 * (rect.Width - newWidth));
			int newTop = rect.Top + (int)(0.5 * (rect.Height - newHeight));


			return new Rectangle(newLeft, newTop, newWidth, newHeight);
		}

		public static int Scale(int sourceSize, int targetSize, int valueToScale)
		{
			decimal percentageOfSource = ((decimal)valueToScale) / ((decimal)sourceSize);
			decimal valueOfTarget = percentageOfSource * (decimal)targetSize;
			return Convert.ToInt32(Math.Round(valueOfTarget, 0));
		}

		public static int Scale(int sourceSize, double scalePercentage)
		{
			double result = ((double)sourceSize) * scalePercentage;
			int intResult = Convert.ToInt32(Math.Round(result, 0));

			if (intResult == sourceSize)
			{
				if (scalePercentage > 1)
					intResult++;
				if (scalePercentage < 1)
					intResult--;
			}
			if (intResult == 0)
				result++;
			return intResult;
		}

		#endregion
	}
}
