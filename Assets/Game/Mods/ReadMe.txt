This folder is the place for development files of Daggerfall Unity mods. Its content is not tracked by git for the core project.

Each mod should be in a subfolder with an appropriate name, i.e. Mods/Example.
The following is an optimal organization of files structure. This is not mandatory but is a good start if you're making your first mod.

~/<Name>.dfmod.json
The manifest file for the mod, created with Daggerfall Tools > Mod Builder.

~/modsettings.json
~/modpresets.json
Settings and settings presets, created with Daggerfall Tools > Mod Settings.

~/ReadMe.txt
A readme file that should be shipped with the mod.

~/Assets
A folder containing text or binary assets like textures, materials, models and prefabs.
It should follow the same structure of loose files inside StreamingAssets (i.e. Assets/IconPacks/), although this is often not mandatory.

~/Scripts
A folder containing C# scripts.

~/Scripts/Editor
A folder containing C# scripts for the Unity Editor.

~/Shaders
A folder containing Unity shaders.

~/Out~
A possible target directory for the Mod Builder, which is not imported by Unity so no unnecessary .meta files are created.
If you make a repository for your mod, you should add this folder to .gitignore file.

For more information on development of mods for Daggerfal Unity see https://www.dfworkshop.net/projects/daggerfall-unity/modding/.