This folder contains books in localized .txt format.
Classic books have been exported here so English text can be improved in base game.

Books placed in this folder must have the following format:

* Must be a UTF-8 .txt file.
* Filename must end in -LOC (e.g. "BOK00042-LOC.txt").
* File must begin with a header block for book data followed by book contents.

<<<<EXAMPLE FILE FOLLOWS>>>>

Title: The Madness of Pelagius
Author: Tsathenes
IsNaughty: False
Price: 580
IsUnique: False
WhenVarSet: 
Content:



[/font=2]

[/center]The Madness of

[/center]Pelagius



[/font=4]

[/center]

 The man who would be Emperor of all Tamriel was born Thoriz
...

<<<<END EXAMPLE FILE>>>>

Notes:
* Everthing below the Content: line (including whitespace) is considered readable part of book shown to player.
* When translating book files, do not translate field names (e.g. Title:, Author:) or formatting tags (e.g. [center], [/font=4]).
* Supported formatting tokens are [/left] [/center] [/font=N] (N is font index 1 through 4).