
# WDSwap
A Virtual Desktop Swapper for Windows

<h3 align="center"><img src="https://i.imgur.com/yD1fBrI.png"></h3>

# About
The Windows Desktop Swapper came about because I had a love for the utility of Virtual Desktops in Linux, and was depressed with the Windows implementation. The Task View is over-complicated for what I am looking for: a simple way to swap between Desktop M and Desktop N.

This tool is aimed at giving you easy access to your Virtual Desktops, and provides customization in the form of a config.ini file that will be generated on first-run.

# Using WDSwap
WDSwap automatically starts itself in the lower right hand portion of your main monitor. It will have dedicated buttons for each of the desktops you can swap to. Clicking one of these buttons will change the active desktop. You can also change the active desktop by using hotkeys. The hotkeys consist of a mod key and the desktop's index, EG: `Alt+3`. The mod key can be changed between Alt and Ctrl. 

In addition to the Desktop Swapper buttons, there is one additional 'gripper' button to the far right of the others. This button is used to control the application. Using `left click + drag` you can move the application somewhere else on your screen. Right clicking on the gripper button will bring up a context menu, letting you do things like edit the config file, re-load the config file, or quit the application.
<img src="https://i.imgur.com/pNYmQ9Z.png" alt="Example of Graphic Buttons" align="right" width="400px">
WDSwap does not appear in the taskbar as a typical application. Instead it lives in the Notification section of your taskbar. You can right click on the icon in your notification area to access the same controls available from the gripper button.

# Requirements
  - Windows 10    |    _tested on builds 1709 and 1803_
  - .NET Framework 4.6.1

# Graphic Buttons
<img src="https://i.imgur.com/ZQ0Q9yb.png" alt="Example of Graphic Buttons" align="right" width="400px">
A feature that is still in development is the Graphic buttons feature (to enable this feature, in the config section 'Buttons', change the 'ButtonType' setting to 'DisplayGraphic'). When this feature is enabled, instead of displaying the names you have configured on the buttons the application will get a lit of running processes and generate an image that represents your virtual desktops. This is a convinient feature if the role of your virtual desktops change frequently but you'd like to have some idea of what is running on them.

This feature is still under development and requires some performance tuning. When using this feature WDSwap will require more system resources than it does when simply displaying the names.

# Configuration
The default configuration file is automatically generated in the user's Documents folder, in a new 'wdswap' subdirectory. editing the config.ini file there will change the behavior of the application. The config file itself is littered with comments to guide you through the editing process. Here are a few things that you can do:

  - Change the max number of Virtual Desktops, up to a max of 10
  - Change the display name of each Virtual Desktop button individually
  - Change the size of the application
  - Change the colors of the buttons
  - Disable hotkey support