using IniParser.Model;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WDSwap.Gui.Configuration
{
	public class WDSwapConfiguration
	{
		public WDSwapConfiguration()
		{
			Application = new ApplicationConfiguration();
			HotKeys = new HotKeysConfig();
			Buttons = new GlobalButtonConfig();
			DesktopOne = new ButtonConfig("1");
			DesktopTwo = new ButtonConfig("2");
			DesktopThree = new ButtonConfig("3");
			DesktopFour = new ButtonConfig("4");
			DesktopFive = new ButtonConfig("5");
			DesktopSix = new ButtonConfig("6");
			DesktopSeven = new ButtonConfig("7");
			DesktopEight = new ButtonConfig("8");
			DesktopNine = new ButtonConfig("9");
			DesktopTen = new ButtonConfig("10");
			FocusedDesktopColors = new ColorsConfig();
			ActiveDesktopColors = new ColorsConfig();
			InactiveDesktopColors = new ColorsConfig();

		}

		[IniComment("This section handles application-level configuration that effects how the tool runs")]
		public ApplicationConfiguration Application { get; set; }

		[IniComment("This section controls the hotkey behavior")]
		public HotKeysConfig HotKeys { get; set; }

		[IniComment("This section handles settings that affect all of the Desktop swapper buttons")]
		public GlobalButtonConfig Buttons { get; set; }

		[IniComment("These sections control the individual settings for the 10 desktop swapper buttons",
			"The 'Name' attribute can be any single-line text value, but should be kept short to keep the buttons from being too long")]
		public ButtonConfig DesktopOne { get; set; }

		public ButtonConfig DesktopTwo { get; set; }

		public ButtonConfig DesktopThree { get; set; }

		public ButtonConfig DesktopFour { get; set; }

		public ButtonConfig DesktopFive { get; set; }

		public ButtonConfig DesktopSix { get; set; }

		public ButtonConfig DesktopSeven { get; set; }

		public ButtonConfig DesktopEight { get; set; }

		public ButtonConfig DesktopNine { get; set; }

		public ButtonConfig DesktopTen { get; set; }

		[IniComment("This section handles the colors of the currently focused Virtual Desktop",
			"All colors are in hexadecimal RGB format")]
		public ColorsConfig FocusedDesktopColors { get; set; }

		[IniComment("This section handles the colors of all 'active' virtual desktops",
			"This tool does not create desktops until you switch to them. A desktop that has not yet been created is considered 'inactive'")]
		public ColorsConfig ActiveDesktopColors { get; set; }

		[IniComment("This section handles the colors of all 'inactive' virtual desktops",
			"This tool does not create desktops until you switch to them. A desktop that has not yet been created is considered 'inactive'",
			"This also controls the colors the colors of the Gripper button")]
		public ColorsConfig InactiveDesktopColors { get; set; }


		public ButtonConfig GetButtonConfigByIndex(int index)
		{
			switch (index)
			{
				case 1:
					return DesktopOne;
				case 2:
					return DesktopTwo;
				case 3:
					return DesktopThree;
				case 4:
					return DesktopFour;
				case 5:
					return DesktopFive;
				case 6:
					return DesktopSix;
				case 7:
					return DesktopSeven;
				case 8:
					return DesktopEight;
				case 9:
					return DesktopNine;
				case 10:
				default:
					return DesktopTen;
			}
		}

		public IniData ToIni()
		{
			var result = new IniData();
			foreach (var catProp in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				var categoryName = catProp.Name;
				var catObject = catProp.GetValue(this);
				var sectionData = new SectionData(categoryName);

				var catCommentAttr = catProp.GetCustomAttribute<IniCommentAttribute>();
				if (catCommentAttr != null)
					sectionData.Comments.AddRange(catCommentAttr.Comments);

				foreach (var prop in catProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					var propName = prop.Name;
					var propValue = prop.GetValue(catObject);

					var commentAttr = prop.GetCustomAttribute<IniCommentAttribute>();
					
					KeyData keyData = new KeyData(propName);
					keyData.Value = propValue.ToString();

					if (commentAttr != null)
						keyData.Comments = commentAttr.Comments;
					
					sectionData.Keys.AddKey(keyData);
				}

				result.Sections.Add(sectionData);
			}
			return result;
		}

		public static WDSwapConfiguration FromIni(IniData iniData)
		{
			var result = new WDSwapConfiguration();
			Type resType = result.GetType();

			foreach (var section in iniData.Sections)
			{
				var configProp = resType.GetProperty(section.SectionName, BindingFlags.Public | BindingFlags.Instance);
				if (configProp == null)
					continue;

				var configPropObj = configProp.GetValue(result);

				foreach (var key in section.Keys)
				{
					var keyProp = configProp.PropertyType.GetProperty(key.KeyName, BindingFlags.Public | BindingFlags.Instance);

					if (keyProp == null)
						continue;

					

					keyProp.SetValue(configPropObj, ChangeType(key.Value, keyProp.PropertyType));
				}
			}

			result.ActiveDesktopColors.SelfCheck();
			result.InactiveDesktopColors.SelfCheck();
			result.FocusedDesktopColors.SelfCheck();

			return result;
		}

		private static object ChangeType(string value, Type targetType)
		{
			if (targetType == typeof(string))
				return value;
			if (targetType == typeof(int))
				return Convert.ToInt32(value);
			if (targetType == typeof(bool))
				return Convert.ToBoolean(value);
			if (targetType == typeof(ModKeyEnum))
				return Enum.Parse(typeof(ModKeyEnum), value);
			if (targetType == typeof(ButtonTypeEnum))
				return Enum.Parse(typeof(ButtonTypeEnum), value);

			return value;
		}
	}
}
