COMP 376 Assignment 1
-----------------------------------------------------

Reading The Code:
-----------------------------------------------------
All of the code is contained in the Scripts folder in the Assets of the game. It contains a boat script which is tied to the boat object of the game. This script tracks the current gold count, the enemy spawning, the nitro cylinder generation in variant mode, and many other things since it is a persistent object and so a good candidate for doing these checks. There is also a high score loader script which loads the high scores and displays them on the menu of the game. There is also a nitro cylinder script that is tied to nitro cylinder objects and causes them to move down when they are spawned and also disappear when they go out of the frame. It also includes the collider code for the objects. Next is a scene switcher script that contains a function to switch scenes. This is used by the menu to load the appropriate game modes. Finally we have the submarine script which contains collision code for enemies, movement of the submarine as well as collecting gold and activating nitro boost in variant mode.

All of the code is heavily commented and easy to understand.

Compiling and Playing:
----------------------------------------------------
By dragging the Assets and Project Settings Folders into a new folder and creating a new Unity Project, the game can be played within Unity by pressing the play button once loaded in Unity. The package also contains a Build folder that contains a build of the game that can be played on a Windows PC.

Controls:
----------------------------------------------------
w - Move submarine up
s - Move submarine down
a - Move submarine left
d - Move submarine right
e - Activate nitro cylinders (Variant Mode Only)

Colors:
-----------------------------------------------------
Red - The submarine will be immune for two seconds
Green - Nitro boost is available (Variant Mode Only)