Place replacement BIOG*.txt files in this folder.

The name format is BIOG[class]T[index].txt
Class is a value between 0 and 17, inclusively. They correspond to the same list the player chooses when starting a new character.
Index is any number. The classic biographies use 0, and mods are free to use any other number they want.

For example, BIOG10T2.txt would be a valid alternative biography for a Thief, or Thief-adjacent custom class.

To allow mods to have a custom backstory to go with the biography, BIOG files may optionally start with the following line:

#12345

This will make the game use the specified string id (here, 12345) for the History text found in the character sheet. By default, the string id is 4116 + class. Classic strings stop at 9999, so mods can use a custom ITextProvider to have custom strings above this value.

Note that BIOG text is not currently included in core localization string tables.
It's necessary to copy correct language version of files into BIOGs folder.
English BIOG files are used by default, DE and FR versions are also provided for convenience.

The T0 templates are not exactly the original Arena2 BIOG files. Some bugfixes and minor balance changes have been made.

Credit for fixed files to Frank 'Deepfighter' Schwalb. Please see "readme-deepfighter.txt" for more information.