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
* **At least 1 connected keyboard**  

* **DirectX 9.0c June**    
https://download.microsoft.com/download/8/4/A/84A35BF1-DAFE-4AE8-82AF-AD2AE20B6B14/directx_Jun2010_redist.exe  

* **Vcredist 2013 x86**    
https://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe  

* **Microsoft's Xbox Accessories Driver** (64 bit)    
https://github.com/djlastnight/KeyboardSplitterXbox/blob/master/Xbox360Accessories_x64_1.2.exe?raw=true  

# Scp virtual bus version mismatch  
If you get 'Slot is invalidated' error, you probably you have different SCP Bus installed.  
It must be 22.52.24.182. In case it differs (no matter newer or older):  
Go to device manager and under system devices find it and uninstall it.  
![image](https://user-images.githubusercontent.com/19281127/115582624-d1ffd100-a2d1-11eb-8e8a-c7fc69ebfdc4.png)

Reboot, run KS - it should offer you to install drivers, do it and reboot again.  
If this does not fix your problem open a new issue or find existing one.  

# Installation:
Run the application, it will ask you to install the built-in drivers.
Do it and reboot your PC.
Please read the FAQ section located in application's Help menu.

# Keyboard Ghosting
Keyboard splitter can not really help in case you own a cheap keyboard, so you have 2 options:  
1. Buy an anti-ghosting keyboard  
or  
2. Change the preset bindings. Try which keys might be pressed simultaneously here: https://drakeirving.github.io/MultiKeyDisplay/  
Please do not report issues like *%some key%* + *%other key%* does not register ingame!  
**Choose your preset bindings wisely to avoid ghosting!**  
The default preset has known ghosting - you can't use both sticks (LS and RS) at their lower left positions (x min + y min) simultaneously.  
This is because on cheap keyboard you can not use LeftArrow + DownArrow + NumPad4 + NumPad2 simultaneously. Try yours from the link above.  

Please do use the built-in xbox 360 controller tester (from app file menu 'Controllers' -> 'Test Xinput Controllers') to ensure the preset works as you expect, before running the game.   
If you encounter strange controller behavior in-game, please do an anti-ghosting check, described above, before commiting an issue.  

# Download
https://github.com/djlastnight/KeyboardSplitterXbox/releases

# Graphical User Interface (GUI)  
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

# Command Line Interface (CLI)    
Since version 2.3 you can start KeyboardSplitter.exe from terminal by passing a game to autostart.  
This means you must already have a game, added to your games list.  
In case you do not have a game defined, start the app as usual (using the GUI), go to Games -> Edit -> Add Game.  
  
Once you have 1 or more games at your splitter_games.xml, provide the desired game title as command line agrument `game=`  
The app will autohide the GUI, mount the predefined slots, start the emulation and then will autostart the game executable.  
Once you exit the game, keyboard splitter will autoclose.  
  
example start from command line: `KeyboardSplitter.exe game=Fifa` // Fifa is a game Title, defined at splitter_games.xml

# Custom Axes Range (0-100%) 
By default the app will use the Axis Min and Max value.  
You can achieve custom percentage of an axis by manually editing a preset xml file.  
Open splitter_presets.xml file and manually add the following line  
`<axis id="1" value="-16384">None</axis>`  
In case you do not have such file simply open the app, create a new preset, exit the app and save the presets when asked.  

Open the app, choose the preset you just modified and your custom axis will appear at the preset UI as follows  
![image](https://github.com/djlastnight/KeyboardSplitterXbox/assets/19281127/3d092000-811b-4662-9cf2-1ea58b88d4be)  

You have your custom axis created. You can use it as usual.  

Here are the axis ids to use:  
Left Stick X = 1  
Left Stick Y = 2  
Right Stick X = 4  
Right Stick Y = 8  

Values must be in range -32768 to 32767  
You can add as many custom axes as you wish. The example above is an example of -50% Axis X  

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
install them on first run. The user must install Xbox Accessories Driver separately.      
You might want to try out my [Interception GUI Uninstaller](https://github.com/djlastnight/KeyboardSplitterXbox/blob/master/InterceptionUninstall/interception-gui-uninstaller.zip?raw=true) in case you recently updated your W10.  

# Credits (original author's projects)

https://github.com/oblitum/Interception

https://github.com/jasonpang/Interceptor

https://github.com/nefarius/ScpVBus

https://github.com/shauleiz/vXboxInterface

djlastnight, 2023
