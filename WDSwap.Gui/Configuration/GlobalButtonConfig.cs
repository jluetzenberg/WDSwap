using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class GlobalButtonConfig
	{
		public GlobalButtonConfig()
		{
			BorderWidth = 1;
			MinimumButtonWidth = 50;
			GripperButtonWidth = 10;
			ButtonMarginSize = 5;
		}

		[IniComment("This property controls the width of the border surrounding each button",
			"It can be assigned any number, but it is recommended not to make it too large")]
		public int BorderWidth { get; set; }

		[IniComment("This property controls the minimum width of the buttons",
			"The buttons will automatically widen if the text entered for their name is too long, but this property prevents them from shrinking too small")]
		public int MinimumButtonWidth { get; set; }

		[IniComment("The Gripper button is the last button on the right of all of the desktop buttons",
			"You can click-and-drag the gripper button to move the application, and right click on it to load the context menu",
			"The gripper button uses the Inactive color styles defined below",
			"This property controls the width of the button")]
		public int GripperButtonWidth { get; set; }

		[IniComment("This setting controls how much horizontal space is between the button borders and the button text")]
		public int ButtonMarginSize { get; set; }

		[IniComment("Multi-choice setting controlling how the buttons should be displayed",
			"Options: DisplayName, DisplayGraphic")]
		public ButtonTypeEnum ButtonType { get; set; }

		//[IniComment("Show desktop index in upper right corder of button")]
		//public bool Show

	}

	public enum ButtonTypeEnum
	{
		None = 0,
		DisplayName = 1,
		DisplayGraphic = 2
	}
}
