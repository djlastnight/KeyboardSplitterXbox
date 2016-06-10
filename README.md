# KeyboardSplitterXbox

By default Windows OS does not distinguish between the
connected keyboards. They act as the same device.

The current solution creates up to 4 virtual xbox 360 controllers
and feeds them via one or more keyboards (up to 10).
The goal is to play any game that supports xbox controllers
with different keyboards instead of just one. Any application,
which works with such controllers should be supported too.

The main project is called Keyboard Splitter.
All other projects are build into KeyboardSplitter\Lib folder.
The main project loads both managed and unmanaged assemblies.
The managed ones are directly loaded into memory.
The unmanaged ones are first extracted to user's temp folder
and then loaded, using LoadLibrary method from user32.dll via PInvoke.

This produces a single fully portable executable file (*.exe).

Keyboard Splitter required drivers (interception and xbox bus)
are embedded into the exe file and the user will be prompted to
install them on first run. The user must install Xbox Accessories
Driver if he/she uses Windows XP, Vista or Seven.

The User Interface is very intuitive and does not require
technical skills.

![alt tag](https://raw.githubusercontent.com/djlastnight/KeyboardSplitterXbox/master/splitter_UI_help.png)

All keyboard mappings to xbox functions such as buttons,
axes, d-pad directions and triggers are fully customizable.
This is possible via preconfigured presets.
The user could manage (add/edit/delete) different presets
for different games/applications/players. The presets are kept in
presets.xml, which the application reads on startup writes on exit.
It also comes with two hardcoded presets called 'default' and 'empty'.

#Main Features:
- keyboards input monitor
- virtual xbox 360 controllers tester
- customizable mapping presets
- managing xbox custom functions
- keyboard detector
- key detector
- realtime usb detection
- keyboard input blocker
- remote blocking/unblocking the keyboards input

#Installation:
Run the application, it will ask you to install the built-in drivers.
Do it and reboot your PC.
If you use Windows XP, Vista or Sever, you also need the
Microsoft's Xbox Accessories Driver found at https://www.microsoft.com/hardware/en-us/d/xbox-360-controller-for-windows

djlastnight, 2016
