+------------------------------------------------------------------------------------------------------+
| WDSwap                                                                                               |
+------------------------------------------------------------------------------------------------------+
|                                                                                                      |
| The Windows Desktop Swapper is a utility to provide easier access to the Windows Virtual Desktop     |
| infrastructure. WDSwap is designed to be configuration-driven, allowing you to customize the look    |
| and behavior of the app to your specific use case.                                                   |
|                                                                                                      |
+------------------------------------------------------------------------------------------------------+
| Virtual Desktops                                                                                     |
+------------------------------------------------------------------------------------------------------+
|                                                                                                      |
| Windows 10 introduced the Virtual Desktop concept to windows. A Virtual Desktop is simply a different|
| grouping of running applications. You can swap between Virtual Desktops and have a different set of  |
| applications running on each one. This can be very useful for someone who is commonly multi-tasking  |
| but does not have access to a second monitor, or to someone who likes to keep each set of 'tasks'    |
| separate. For instance, you may want to have you Email and IM software running on one Virtual        |
| Desktop, your web browsing on another, and the documents you're working on running on a third.       |
|                                                                                                      |
| By default, Windows allows you to access the Virtual Desktops using the 'Task View' button commonly  |
| pinned to your task bar. This button opens up a complex view of your running apps and allows you to  |
| swap between different Virtual Desktops. Using this 'Task View', however, can be slow (as it has to  |
| open a new view with a lot of information), and you can only see what Desktops you have when you     |
| enter the view.                                                                                      |
|                                                                                                      |
+------------------------------------------------------------------------------------------------------+
| Using the app                                                                                        |
+------------------------------------------------------------------------------------------------------+
|                                                                                                      |
| The application will automatically start itself in the lower-right hand portion of your screen. It   |
| will contain a series of numbered Desktop Swapping buttons, representing the Virtual Desktops that   |
| the tool will give you access to. By default, it has 5 desktops, each simply numbered 1-5.           |
|                                                                                                      |
| There are two ways to swap between desktops:                                                         |
|     1. Click on the appropriate button                                                               |
|     2. Use the hotkey                                                                                |
|         - typically this is the Alt key plus a number key corresponding to your target desktop       |
|         - EG: Alt+1    <- this would swap to Virtual Desktop 1                                       |
|               Alt+3    <- this would swap to Virtual Desktop 3                                       |
| When you swap between desktops you will notice any opened applications will disappear, and the button|
| for the desktop you switch to will change color. The desktop you are currently using will typically  |
| appear lighter than the other desktops. Anny application you launch while viewing a given desktop    |
| will be tied to that desktop.                                                                        |
|                                                                                                      |
| In addition to the Desktop Swapping buttons, to the far-right of the application is a smaller        |
| 'gripper' button. This button has two functions:                                                     |
|     1. When you press and hold left click, it will drag the application across the screen            |
|     2. When you right click on it, it will present you with a context menu.                          |
| The context menu will allow you to exit the application, open the configuration file, re-load the    |
| configuration file, or restore it to defaults.                                                       |
|                                                                                                      |
+------------------------------------------------------------------------------------------------------+
| Configuration                                                                                        |
+------------------------------------------------------------------------------------------------------+
|                                                                                                      |
| The application itself is designed to be configurable to your preferences wherever possible. This    |
| section attempts to give you an introduction to the configuration, but the config file itself has    |
| comments to explain what the settings mean and may be more up-to-date than this entry.               |
|                                                                                                      |
| The configuration file uses the common *.ini format, and can be found in your My Documents folder in |
| the wdswap folder.                                                                                   |
|                                                                                                      |
| The ini format separates sections using bracketed section names, such as "[Application]". Following  |
| the section name will be a series of setting names being assigned values. EG: "MaxDesktops = 5".     |
| These settings can all be changed, and when you select the "Refresh" option from the context menu it |
| will trigger the tool to reload the configuration with the changes you have made. Below are some of  |
| the more useful settings you can change:                                                             |
|                                                                                                      |
| The Application section:                                                                             |
|     This section deals with general settings for the application                                     |
|                                                                                                      |
|     * MaxDesktops                                                                                    |
|         - this setting controls how many Desktop Swapping buttons will be displayed                  |
|         - the value can be any number between 1 and 10                                               |
|     * Application Height                                                                             |
|         - Measured in pixels, this controls how tall the application will be                         |
|         - a height of 50 will provide an easy-to-read tool, while 30 will be more out of the way     |
|         - while there are no restrictions on the values, making it too small will make it hard to    |
|             use the tool                                                                             |
|     * DefaultConfigurationEditor                                                                     |
|         - this is what application is launched when you select 'Configure' from the context menu     |
|         - This defaults to Notepad, but can be changed to your text editor of choice                 |
|                                                                                                      |
| The HotKeys section:                                                                                 |
|     This section controls the hotkeys used by the application                                        |
|                                                                                                      |
|     * BindHotKeys                                                                                    |
|         - This controls whether or not the hotkeys for desktop swapping will be available            |
|     * ModKey                                                                                         |
|         - This allows you to choose either the Control key or the Alt key for desktop swapping       |
|         - The hotkeys will be a combination of the ModKey and the number of the desktop you want     |
|             to swap to                                                                               |
|         - EG: Alt+3 to swap to Virtual Desktop 3                                                     |
|                                                                                                      |
| The Buttons section:                                                                                 |
|     This section handles settings that affect all of the Desktop Swapping buttons                    |
|                                                                                                      |
|     * BorderWidth                                                                                    |
|         - This controls the width of the border that surround each button                            |
|     * MinimumButtonWidth                                                                             |
|         - This controls how small a button can be, regardless of how much text there is              |
|         - When you assign a name to a button, the button will grow to fit the text. This setting     |
|             will control how small the button is if the name is too short, such as the number 1      |
|     * GripperButtonWidth                                                                             |
|         - Controls how wide the gripper button is. If this is too small it may be difficult to       |
|             control the application                                                                  |
|     * ButtonMarginSize                                                                               |
|         - controls the horizontal distance between the button's text and the edge of the button      |
|     * ButtonType                                                                                     |
|         - This setting allows you to swap between text names on your buttons and a graphical display |
|         - The default option 'DisplayName' will display the names in the following section on each   |
|           of the buttons                                                                             |
|         - The 'DisplayGraphic' option will detect running processes and draw a graphical             |
|           representation of each Virtual Desktop on the buttons                                      |
|               - It is worth noting that the DisplayGraphic option requires more system resources than|
|                 the DisplayName, and requires some performance tuning, as the graphic refresh on     |
|                 switch tends to lag behind the actual switching of desktops.                         |                                                                                     |
|                                                                                                      |
| The Button{One, Two, Three...} sections:                                                             |
|     These sections are to control the individual buttons for each Virtual Desktop                    |
|                                                                                                      |
|     * Name                                                                                           |
|         - the text to display for the given Virtual Desktop Swapper button                           |
|         - this can simply be the number, or can be a string of text.                                 |
|             EG: if you plan to keep an email application running on desktop number 3, consider       |
|                 changing it's 'Name' setting to 'Email'                                              |
|                                                                                                      |
| The {Focused, Active, Inactive}DesktopColors sections:                                               |
|     These sections control the colors of the buttons.                                                |
|     The 'Focused' desktop is the one you are currently viewing                                       |
|     To understand 'Active' vs 'Inactive' requires some description. When you first start windows     |
|     there is only one Virtual Desktop running, which is the main desktop, desktop 1. If you launch   |
|     WDSwap in this state, Desktop 1 will be considered 'Focused', while all other desktops will be   |
|     considered 'Inactive'. If you then navigate to Virtual Desktop 4, you will see that desktops 2   |
|     and 3 will become 'Active', and desktop 4 will become 'Focused. This is because in order to give |
|     you access to a given Windows Virtual Desktop, the tool must first create all of the Virtual     |
|     Desktops before your target. The tool handles all of this for you, and shows Active vs Inactive  |
|     to give you an idea of what the tool is doing for you.                                           |
|                                                                                                      |
|     * Background                                                                                     |
|         - The hexadecimal RGB color to assign as the background color for the button                 |
|     * Foreground                                                                                     |
|         - The hexadecimal RGB color to assign as the foreground color for the button                 |
|         - This will affect the text                                                                  |
|     * Border                                                                                         |
|         - The hexadecimal RGB color to assign as the border color for the button                     |
|                                                                                                      |
+------------------------------------------------------------------------------------------------------+