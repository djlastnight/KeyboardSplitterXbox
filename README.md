# Support me on Patreon
<a href="https://www.patreon.com/djlastnight" style="font-size:50px">
  <img src="https://c5.patreon.com/external/logo/rebrandLogoIconMark@2x.png"
       height="40"
       style="vertical-align:top" />
  Click here to become a patron and get your reward!
    <img src="https://c5.patreon.com/external/logo/rebrandLogoIconMark@2x.png"
       height="40"
       style="vertical-align:top" />
</a>                             

# djlastnight's Gaming Keyboard Splitter
[![Github All Releases](https://img.shields.io/github/downloads/djlastnight/KeyboardSplitterXbox/total.svg?style=plastic)](https://github.com/djlastnight/KeyboardSplitterXbox)  
By default Windows OS does not distinguish between the
connected keyboards. They act as the same device.

The current solution creates up to 4 virtual xbox 360 controllers
and feeds them via one or more keyboards (up to 10).
The goal is to play any game that supports xbox controllers
with different keyboards instead of just one. Any application,
which works with such controllers should be supported too.

# Video
[![video](https://img.youtube.com/vi/06ZZp-u01kE/0.jpg)](https://www.youtube.com/watch?v=06ZZp-u01kE)

# Main Features:
- keyboards input monitor
- virtual xbox 360 controllers tester
- customizable mapping presets
- managing xbox custom functions
- keyboard detector
- key detector
- realtime usb detection
- keyboard input blocker
- remote blocking/unblocking the keyboards input

# Prerequisites
DirectX 9.0c June https://download.microsoft.com/download/8/4/A/84A35BF1-DAFE-4AE8-82AF-AD2AE20B6B14/directx_Jun2010_redist.exe  

Vcredist 2013 x86 https://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe  

At least 1 connected keyboard  

If you use Windows XP, Vista or Seven, you also need the
Microsoft's Xbox Accessories Driver found at https://www.microsoft.com/accessories/bg-bg/d/xbox-360-controller-for-windows  

# Installation:
Run the application, it will ask you to install the built-in drivers.
Do it and reboot your PC.
Please read the FAQ section located in application's Help menu.

# Download
https://github.com/djlastnight/KeyboardSplitterXbox/releases

# User Interface
The User Interface is very intuitive and does not require
technical skills.

![alt tag](https://raw.githubusercontent.com/djlastnight/KeyboardSplitterXbox/master/splitter_UI_help.png)

All keyboard mappings to xbox functions such as buttons,
axes, d-pad directions and triggers are fully customizable.
This is possible via preconfigured presets.
The user could manage (add/edit/delete) different presets
for different games/applications/players. The presets are kept in
presets.xml, which the application reads on startup and writes on exit.
Keyboard Splitter comes with two hardcoded presets called 'default' and 'empty'.

# How it works

![alt tag](https://raw.githubusercontent.com/djlastnight/KeyboardSplitterXbox/master/how_it_works_diagram.png)

# Internal Build Details
The main project is called Keyboard Splitter.
All other projects are build into KeyboardSplitter\Lib folder.
The main project loads both managed and unmanaged assemblies.
The managed ones are directly loaded into memory.
The unmanaged ones are first extracted to user's temp folder
and then loaded, using LoadLibrary method from kernel32.dll via PInvoke.
This produces a single fully portable executable file (*.exe).

# Drivers
Keyboard Splitter required drivers (interception and xbox bus)
are embedded into the exe file and the user will be prompted to
install them on first run. The user must install Xbox Accessories
Driver if he/she uses Windows XP, Vista or Seven.

# Credits (original author's projects)

https://github.com/oblitum/Interception

https://github.com/jasonpang/Interceptor

https://github.com/nefarius/ScpVBus

https://github.com/shauleiz/vXboxInterface

djlastnight, 2017
