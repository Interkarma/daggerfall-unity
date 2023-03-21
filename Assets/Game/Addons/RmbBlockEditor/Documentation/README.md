# RMB Block Editor

I highly recommend reading this thread by Hazelnut before continuing with the rest of this information. It will help you understand RMB blocks and their structure.

https://forums.dfworkshop.net/viewtopic.php?t=2857

Also, here are the video tutorials that I have made for the tool:

https://youtu.be/dk5z2orOcBQ


## Getting started

To get started with the editor, first you need to dump some RMB blocks from the game. To do that you can use the dumpblock console command. For example:

    dumpblock ARMRAL02.RMB

For more info look at the forum post by Hazelnut, linked above.

To get the editor open, start Unity, go to Daggerfall Tools -> RMB Block Editor. It is designed to be docked on the right side of the screen, where the Inspector window is.

This editor comes with its own menu on the top. To open an RMB block file go to File -> Open and select the file on your computer. If you chose a valid file, the block will get loaded in the scene.


## Block properties editor

You can open this view by selecting the RMB Block object in the Hierarchy, or by clicking Edit -> Block Properties from the menu in the editor. This view is also opened by default after loading an RMB block file. Here you can edit the Position, Index, Name and Type of the block.


## Climate and Season

To change the climate and season that you are viewing the block in, you can go to File->Settings in the menu. You can also change the window style of the buildings here. Changing these options will change the ground textures as well as most of the textures on buildings, models and flats.

## Editing the Ground Textures

There are two ways you can go to the ground editor view.

1. From the menu, go to Edit -> Ground
2. From the Hierarchy, expand the RMB Block object and click on the Ground object.

The ground editor has two modes – selection mode and paint mode. You can change the mode by clicking the button in the upper-left corner of the editor.

In selection mode, you can select a tile on the lower part of the editor. Once the tile is selected, you can preview it in the upper part of the editor. You can use the buttons and the input to change its rotation and texture.

In paint mode you can pick a texture to pain with from the upper left part of the editor. You can see an enlarged preview of the texture on the right. The buttons can be used to rotate the texture. Once the texture and rotation are selected you can paint on the map, which is in the lower part of the editor.


## Editing Ground Scenery

Ground scenery includes various plants, trees and rocks that are saved as part of the GroundData. These objects are limited to a specific palette and can only be placed in the lower left corner of any given ground texture tile.

To open this view, go to the Hierarchy, expand the RMB Block object and click on the Ground Scenery object. A palette of scenery objects will appear in the editor window.

To add a scenery object, select an object from the palette by clicking on it. A white border will appear around it, to indicate that it is selected. After that you have to click the Paint button. Now move your mouse cursor over the terrain in the Scene view and a preview of your selected object will appear under it. You can place the object by clicking. To cancel the selection you can right click.

To remove a scenery object, select it and press the delete key on your keyboard.

To select the object you can either click on it in the Scene, or go to the Hierarchy, RMB Block->Ground Scenery ->Scenery and select it from there.


## Editing the Automap

The Automap is the map of the town that the player can see in-game.

There are two ways you can open this editor view.
1. From the menu, go to Edit -> Automap
2. From the Hierarchy, expand the RMB Block object and click on the Automap object.

In this view there is a palette of different building types that you can choose and paint on the map.

Several different building types have the same colors. To see what each box in the palette represents, hover over it and wait for the tooltip to appear. Tooltips will also appear when hovering over the map, so you can tell what has already been painted.

In this view you can also automatically generate the map, by clicking on the Generate Automap button on top of the view. This is useful when you have added or removed buildings and moved them around.


## Editing Buildings

To edit a building you have to select it first. You can either click on it in the scene, or go to the Hierarchy RMB Block->Buildings and select the one you want.

In this view you can change some of the properties of the building:
- FactionId
- BuildingType
- Quality
- XPos
- ZPos
- ModelYPos
- YRotation

Some explanation is needed for the ModelYPos parameter. In this case Y represents the vertical axis. The building itself does not have a Y axis and is always at ground level. However, the model or models that comprise the building do have a Y axis and can be moved up and down. This is what this parameter represents.

For buildings, rotation is only allowed on the Y axis.

You can also change the position and the rotation of the building by using the controls in the Scene view of unity.

You can export a BuildingReplacementData file, as if using the "dumpbuilding" command, by clicking on the "Export Building" button, at the top of the view.

You can also change the building entirely. To do so, you can click either on the "Import from Catalog", or the "Import From File" buttons.

- If you click on "Import from Catalog", you will be able to choose a building form the catalog. You can  use the search input to filter the catalog. You can also use the ObjectID field to select a specific building that way.

- If you click on "Import From File", you can load a building from a file, by clicking the Import button and selecting a BuildingReplacementData file.

In both cases you will be presented with three checkboxes - "Properties", "Exterior" and "Interior". These let you choose what parts of the building you want to update.
"Properties" refers to the FactionId, BuildingType and Quality fields.

To remove a building, simply select it in the scene and press delete.

*Some explanation of how this building picker is implemented might be useful, so I will include it here. In the game 3D and 2D objects are associated with specific IDs and exist as specific entities in the game code. A building, however, is an object constructed from such 3D and 2D objects and does not have its own ID in the RMB block json file. The buildings that can be picked here are templates that I extracted from RMB blocks that exist in the game. Their IDs are only useful for this editor and have no impact on the resulting json file.*

## Editing Misc 3D Objects

Misc 3D objects are any 3D objects in the block that are not part of a building. These can be fences, signposts, water trough and so on.

To edit a 3D Object you have to select it first. You can either click on it in the scene, or go to the Hierarchy RMB Block-> Misc 3D Objects and select the one you want.

In this view you can change the Position, Rotation and Scale of the object.

Note that for the scale, one field changes it for all axes. I tested having different scales for each axis and it breaks the object in some specific cases.

You can also change the position, rotation and scale of the model by using the controls in the Scene view of unity.

If the model is in a catalog subcategory, you can cycle through the different items in the subcategory, using the arrow buttons in this view. You can read more about catalogs in that section of the document.

You can also change the object by clicking the Modify button.

The modify view lets you choose a model from a catalog. You can use the search input to filter the tree. Bear in mind that not all models that are in the game can be found in the tree. However, you can also enter a specific ID in the Object ID field, even if it is not present in the catalog.

Once you select a model, a preview will appear on the right. You can rotate the preview by dragging with the left mouse button.

Once you are happy with the selection you can click the Modify button and the selected model will replace the old one.

To remove a model, simply select it and press delete.


## Editing Misc Flat Objects

Misc flat objects are any 2D objects in the block that are not part of a building. These can be people, animals, trees, lamp posts and so on.

To edit a Flat Object you have to select it first. You can either click on it in the scene, or go to the Hierarchy RMB Block-> Misc Flat Objects and select the one you want.

In this view you can change the FactionID, Flags and Position of the object.

You can also change the position of the flat by using the controls in the Scene view of unity.

If the flat is in a catalog subcategory, you can cycle through the different items in the subcategory, using the arrow buttons in this view. You can read more about catalogs in that section of the document.

You can also change the object by clicking the Modify button.

The modify view lets you choose a flat from a catalog. You can use the search input to filter the tree. Bear in mind that not all flats that are in the game can be found in the tree. However, you can also enter a specific ID in the Object ID field, even if it is not present in the catalog.

Once you select a flat, a preview will appear on the right. You can rotate the preview by dragging with the left mouse button.

Once you are happy with the selection you can click the Modify button and the selected flat will replace the old one.

To remove a flat, simply select it and press delete.


## Adding New Buildings

To add a new building to the block, go to the menu Add -> Buildings.

In this view you can pick a new building, much like when editing an existing one. See “Editing Buildings”.

Once you pick a building, some additional fields will appear under the picker.

The Paint Options let you “paint” the building into the scene.

Just click on the Paint button and move your mouse in the Scene window of Unity. You will see a placeholder for the building appear under your mouse. When you click, the building will be added to the block.

Some buildings have an origin point that is not at ground level, but higher up. In effect this means that when you try to paint them, they will appear sunken into the ground. To fix this problem use the Snap to Surface checkbox.

By using the Position Offset fields you can further adjust any offset of the building when painting.

The Rotation field lets you set an initial rotation of the building. You can also change the rotation while painting, by dragging the left mouse button in the scene.

The Placement Options let you place the building at the exact coordinates and rotation you want, without painting it on the terrain. Click on the Place in scene button to do so.


## Adding New Models

To add a new model to the block, go to the menu Add -> Modes.

In this view you can pick a new model, much like when editing an existing one. See “Editing Misc 3D Objects”.

Once you pick a model, some additional fields will appear under the picker.

The Paint Options let you “paint” the model into the scene.

Just click on the Paint button and move your mouse in the Scene window of Unity. You will see a placeholder for the model appear under your mouse. When you click, the model will be added to the block.

Some models have an origin point that is not at ground level, but higher up. In effect this means that when you try to paint them, they will appear sunken into the ground. To fix this problem use the Snap to Surface checkbox.

In addition, if you want the model to align to a surface, other than the ground plane, you can use the Align With Surface checkbox.

By using the Position Offset and Rotation Offset fields you can further adjust any offset of the model when painting.

While painting, you can adjust the rotation by dragging the left mouse button in the Scene. You can also adjust the scale of the model, by holding CTRL and dragging the left mouse button.

The Placement Options let you place the model at the exact coordinates and rotations you want, and to specify a scale for it, without painting it on the terrain. Click on the Place in scene button to do so.


## Adding New Flats

To add a new 2D object to the block, go to the menu Add -> Flats.

In this view you can pick a new 2D object, much like when editing an existing one. See “Editing Misc Flat Objects”.

Once you pick an object, some additional fields will appear under the picker.

The Paint Options let you “paint” the flat object into the scene.

Just click on the Paint button and move your mouse in the Scene window of Unity. You will see a placeholder for the flat appear under your mouse. When you click, it will be added to the block.

Some flats have an origin point that is not at ground level, but higher up. In effect this means that when you try to paint them, they will appear sunken into the ground. To fix this problem use the Snap to Surface checkbox.

By using the Position Offset fields you can further adjust any offset of the flat when painting.

The Placement Options let you place the flat at the exact coordinates and rotation you want, without painting it on the terrain. Click on the Place in scene button to do so.


## Saving the block

After you are happy with your changes, you can save the RMB block by going to File -> Save in the menu. By default, the file will have the same name as the Name field in the Block properties view, but you can change it if you want.


## Exporting and Importing Object Groups

Sometimes you might want to reuse some objects that you have arranged in the scene. To do this, you can export them to a json file and import them in a different RMB block later on.

To export an object group, you have to select the objects you want to export. You can do that by holding Shift and left-clicking them in the scene, or in the Hierarchy view of Unity. This includes Buildings, Misc 3D Objects and Misc Flat Objects. Once they are selected, you can export them by going to File->Export Selected Objects. Bear in mind that the resulting json file is not a valid RMB block file. It is an export of objects that is only meant to be used with this editor.

To import an object group, you can go to File -> Import Objects. Here you have to select a json file previously exported as described above. Keep in mind that when the objects are imported into the scene they will be imported at the coordinates and rotations that they were exported from.

When exporting objects, it might be a good idea to move them up, above any buildings and models. This way, when you import them they are less likely to overlap with anything else on the scene.

## Editing the catalogs

Using the default catalogs for models, flats and buildings is good to start with, but you will soon want to add some items that are not present there. A good example is assets that come with mods.

To help with this, a catalog system was developed.

## Editing the Models/Flats catalogs

The catalogs for models and for catalogs work the same, so they will be described together.

To go to the catalog editor go to File->Settings->Catalogs->Models or File->Settings->Catalogs->Flats.

Here you can change what is included in the catalog. Every change is saved on your computer, so the next time you load Unity, the catalogs will be the way you left them.

At the top of this view you can see the following buttons:
- Import Catalog - This action lets you import a catalog that has already been exported. If you click this button, you will be asked if you want to replace or merge the existing catalog with the imported one.
- Export Catalog - This lets you save a JSON file, containing the current configuration of the catalog. This can be shared with others or just kept as a backup.
- Remove All Items From The Catalog - This does what it says. It can be useful to have an empty catalog, for things like adding only specific items to an export.
- Restore The Default Catalog - Does what it says.
- Scan For Custom Assets - This lets you scan your project for mods and imports all the assets from those into your catalog. Again, you will be asked if you want to replace or merge with the existing catalog.

Apart from those actions, you can also select an item. When you do, a box will appear at the bottom. There you will be able to change the following:
- Label - this is the name of the item as it will appear in the catalog.
- Category - this is the main category the item is placed under.
- Subcategory - this is the subcategory the item is placed under. When editing objects placed in the scene, you will be able to cycle through all the items in the category that that item is in.
- Tags - these will help when using the Search field. If you enter some text here, you will be able to find this item by using the same text in the Search field.

Also in this box, you have two actions - Save and Remove.
- The Save action lets you save your changes of the fields in the box.
- The Remove action lets you remove this item from the catalog.

If you want to add an item that is not in the catalog, you can enter its ID in the Object ID field. The item will be selected and the box with the ID, Label and Category will appear. You can then use the Save action to add that item into the catalog.

## Editing the Buildings catalog

The buildings catalogs works similarly to the ones for models and flats, but has some differences.

The actions on top are mostly the same. Here are the differences
- The "Scan For Custom Assets" action is not present here.
- There us an "Add New Building to Catalog" button. Since buildings don't have real IDs in Daggerfall, you can't add new ones by entering an Object ID that is not in the catalog. So to add a new building, use this action.

Like models and flats, you can select items and change their Label, Category, Subcategory and Tags.

You have some additional options here:
- Properties - you can change the FactionId, BuildingType and Quality here.
- Import From File - This action lets you import the building from a File. When you click this button, three checkboxes will appear - Properties, Exterior and Interior. These control what parts of the building data you want to import. When you click on the Import button, you will be able to choose a BuildingReplacement file from your system.

## Working with custom assets from mods

Previously, if you wanted to use assets from mods, it was necessary to first run the DaggerfallUnityStartup scene and get to the main menu. This is no longer the case and you do not need to run the game, to use custom assets.