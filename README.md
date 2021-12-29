BOSSFALL v1.2.1 Changelog

Using DaggerfallUnity 0.13.4

CHARACTER CREATION:

BOSSFALL essentially requires a custom super-powered character, so I rebalanced custom character creation

Moved default custom class HP/Level to 20 rather than 8, did not increase default skill advancement difficulty

Most advantages are cheaper, most disadvantages drop difficulty dagger much more

Reducing HP/Level below 20 drops difficulty dagger much more per point

COMBAT MECHANICS:

Restored enemy strafe timer to vanilla functionality (not working right previously due to my error)

DUNGEONS:

Streamlined random selection of enemies, dungeons may load a bit faster

Sabertooth Tigers/Grizzly Bears removed from Mine dungeon types, more Giant Bats and Rats

ITEMS:

I noticed I reduced Weapon generation in General Stores. I forgot to add that to v1.2 Changelog


BOSSFALL: UNLEVELED v1.2 Changelog

Using DaggerfallUnity 0.12.3

BANKING:

You can only have one bank loan at a time, different regions will not loan you more

Defaulting on any loan will permanently ban player from getting another loan from any region

Bank loans charge 20% interest rather than 10%

BOSS INFO:

Assassins will switch to having boss stats once player is Level 7 or higher (2 levels higher than v1.1)

Assassin weapon poison bypasses Poison Immunity once player is Level 7 or higher (Immunity bypass didn't work in v1.1)

BOSS STAT CHANGES:

Vampires, Vampire Ancients, and Alternate Dragonlings will do much less than stated dmg if player is a Vampire

"avg dmg/atk" accounts for multiple instances of damage per attack animation (from Assassins, Orc Warlords, and Daedra Lords)

"avg dmg/atk" is average dmg monster will inflict in one attack animation

Large Assassin damage nerf now that they bypass Poison Immunity

Large Daedra Lord damage nerf to better match average damage of other bosses

Boss damage scales smoother, likely will have no game impact but I like consistency

Vampire
55-85 dmg
70 avg dmg/atk (-3 nerf)

Lich
60-90 dmg
75 avg dmg/atk

Assassin
39-67 dmg
106 avg dmg/atk (-40 nerf)
-8 Armor (It was -16 in v1.1 due to my error)

Alternate Dragonling
95-125 dmg
110 avg dmg/atk (+5 buff)

Orc Warlord
42-73 dmg
115 avg dmg/atk (+5 buff)

Daedra Lord
30-60 dmg
120 avg dmg/atk (-40 nerf)

Vampire Ancient
110-140 dmg
125 avg dmg/atk (-3 nerf)

Ancient Lich
115-145 dmg
130 avg dmg/atk (-5 nerf)

COMBAT:

Gauntlet/boot material affects player's Hand-to-Hand to-hit roll, scales just like vanilla weapon materials

Gauntlet material affects player's punch to-hit, boot material affects player's kick to-hit

Leather/Chain/Steel/Silver do not alter player's to-hit

Successful Hand-to-Hand attacks damage player's glove/boot durability based on if attack is punch/kick

Silver gauntlets/boots will damage Ghosts/Wraiths/Werewolves/etc. if player is using Hand-to-Hand

If you're wearing silver gauntlets you must use punch attacks to damage Ghosts/Wraiths/etc.

If you're wearing silver boots you must use kick attacks to damage Ghosts/Wraiths/etc.

COMBAT MECHANICS:

I don't want non-sentient or stupid enemies to retreat, so these enemies always charge:

Rat
Giant Bat
Grizzly Bear
Sabertooth Tiger
Spider
Slaughterfish
Skeletal Warrior
Giant
Zombie
Ghost
Mummy
Giant Scorpion
Gargoyle
Wraith
Barbarian

Archers/Rangers never move in to attack if they can see player, always retreat, player must rush them

Archer/Ranger movespeed nerfed to reduce annoyance factor when rushing them

None of these changes apply if Enhanced Combat AI is disabled

DISEASE/POISON:

Bossfall v1.1 incorrectly stated a rare spider/scorpion poison bypassed Poison Immunity when it did not

This has been corrected, spider/scorpion has 0.1%/hit of inflicting Drothweed that bypasses Poison Immunity

Spider/Giant Scorpion poison chance nerfed to 10% per hit, I fixed buggy Spider/Scorpion poison formula

Non-boss poisoned weapons will not bypass Resistance or Immunity

Moved non-boss poisoned weapon chance back to vanilla 5%

DUNGEON ENCOUNTER TABLES:

Centaurs aren't in any dungeons, Imps are never found outside dungeons (I forgot to add this to v1.1 changelog)

Every encounter table has been rewritten, no longer using any code from Jay_H's "Unleveled Mobs"

Human Stronghold - Sorcerers removed, replaced with Mages

Prison - Mages removed, replaced with Sorcerers

Desecrated Temple - Barbarians/Harpies added, less Daedra Seducers/Giant Bats

Natural Cave - Sabertooth Tigers removed, more Rats

Crypt - Sabertooth Tigers/Grizzly Bears/Spiders removed, more Skeletons/Zombies/Mummies

Orc Stronghold - more Orc Sergeants/Orc Shamans, less Orcs

Spider Nest - Ghosts added, less Mummies

Barbarian Stronghold - Harpies added, slightly less Barbarians

Cemetery - Added Ghosts (they're rare), less Giant Bats

Underwater - Added Ghosts & Wraiths (they're very rare), less Lamias and Ice Atronachs, more Skeletons

Default building - every human enemy, vermin, Assassin boss

Guildhall - every human enemy, vermin, Alternate Dragonling boss

Temple - every human enemy, vermin, Orc Warlord boss

Palace, House 1 - every human enemy, vermin, Daedra Lord boss

House 2 - every human enemy, vermin, Vampire boss

House 3 - every human enemy, vermin, Ancient Lich boss

ENEMIES:

Human enemies are unleveled once player hits Level 7, 2 levels higher than v1.1 (this includes Guards. HALT!)

Human enemies switch to Hand-to-Hand if that's more damaging than their weapon

Thus, human enemies do much more damage at higher levels (they switch to H2H around enemy level 5)

I thought this H2H check was already in v1.1 but I read the formula wrong

In v1.1 I applied human enemy armor incorrectly, armor did not scale with their level

This has been corrected, human enemies have 4 armor @ Level 1 (slightly better than Imp armor), scales up to -4 armor @ Level 20 (slightly worse than Daedra Seducer armor)

Bards pacified by Etiquette, Barbarians/Rangers by Streetwise

Using Hand-to-Hand against Fire Atronachs/Fire Daedra damages player (if in wereform player won't take dmg)

If using Enhanced Combat AI enemy move speed will not be reduced while enemy is melee attacking

ENEMY STAT CHANGES:

No longer using any stats from Ralzar's "Meaner Monsters"

Rat
7 Armor (+1 buff)

Spriggan
6 Armor (+1 buff)

Giant Bat
5 Armor (-1 nerf)

Grizzly Bear
6 Armor (+1 buff)

Sabertooth Tiger
4 Armor (+1 buff)

Orc
Level 5 (-1 nerf)

Nymph
4 Armor (+1 buff)

Orc Sergeant
25 Max Damage (-5 nerf)
Level 11 (-2 nerf)
2 Armor (-1 nerf)

Zombie
5 Armor (+1 buff)

Mummy
2 Armor (+1 buff)

Orc Shaman
55-165 HP (+12-36 buff)
Level 16 (+3 buff)
-2 Armor (+2 buff)

Gargoyle
-1 Armor (+1 buff)

Frost Daedra
0 Armor (-1 nerf)

Daedroth
198 Max HP (-2 nerf)

Dragonling
30 Max Damage (+3 buff)
40-120 HP (+15-45 buff)

Iron Atronach
198 Max HP (-2 nerf)

Lamia
0 Armor (-2 nerf)

GAMEPLAY:

Building identification in "Info" mode range doubled

Removed the check I put in for switching to Hand-to-Hand if player can't be harmed by enemy's weapon, that check already exists in vanilla DFU code

Shortened most HUD messages when attacking enemies that are immune to your weapon for brevity's sake

After a successful Pickpocket skill check on a human enemy or townsperson, 10% of the time you'll pilfer up to 100 gold pieces, 90% of the time you'll find nothing

Thus, Pickpocketing is 3.3 times more profitable (I never liked how pointless it was in vanilla)

Regardless of whether you find any gold, a successful Pickpocket check will count toward your Thieves Guild initiation letter

ITEMS:

Material generation chances fixed, Daedric possible from any enemy, store, or loot pile, not just high level enemies (major oops)

Rebalanced weapon durabilities, all weapon types use same scale (If you're using RPR:I that'll overwrite a few weapon durabilities)

Daggers have greater durability, bows slightly more, Long Blades/Axes/Blunt Weapons much less

No longer using any item stats from Hazelnut & Ralzar's "Roleplay and Realism: Items"

Changed a few miscellaneous item stats/weights

LOOT:

Gauntlets are now generated as regular enemy equipment (if using RPR:I this change will be hard to notice)

Changed high tier material chances - non-boss human enemies and Daedra will drop somewhat less good loot

High level bosses (anything other than Liches & Vampires) will drop significantly more Daedric

If using RPR:I Cemetery loot piles won't contain armor/weapons (Cemeteries were too profitable w/RPR:I)

If not using RPR:I Cemetery loot piles are now more profitable

LYCANTHROPY:

Transforming into werewolf/wereboar no longer restores player to maximum health

If in wereform equipped gloves/boots aren't damaged and don't give +to-hit bonuses to Hand-to-Hand attacks

If in wereform player can damage enemies that are normally immune to Hand-to-Hand

MAGIC ITEMS:

Holy Water, Holy Daggers, and Holy Tomes cast Dispel Undead/Undead/Daedra on use, have 1/3/3 charges

Holy items generate as normal in high quality Pawn Shops and as random loot from enemies, loot piles and house containers

They will not dispel Undead/Daedra 100% of the time, success rate depends on player level (just like regular Dispel spells)

Most useless magic item effects replaced, some names changed

"Item of Teleportation" has Recall as Cast When Used power

Pacify Undead/Humanoid/Animal & Charm added as Cast When Used powers (in case player can't outrun a boss)

Dispel Magic is now a Potion, recipes spawn normally (so player can get rid of Continuous Damage effects)

Potions of Restore Power restore 50 Magicka, doesn't scale w/player level

Regular magic items found as loot or in stores will never be Holy Water, Holy Daggers, or Holy Tomes

NON-DUNGEON ENCOUNTER TABLES (completely reworked for v1.2):

Mountain Woods use Mountain spawn tables in town at night and in wilderness during the day, use new, unique encounter table in wilderness at night

Swamps use Rainforest encounter tables

Desert, in location, night - all human enemies, rare Werewolf/Wereboar/Gargoyle, Vampire boss

Desert, not in location, day - Giant Scorpions, Dragonlings, Nymphs, Daedra Seducer, Fire Atronachs, Rangers, Gargoyles, Fire Daedra, Alternate Dragonling boss

Desert, not in location, night - Rangers, Gargoyles, Mummy/Skeletons/Ghost/Wraith, Lich & Ancient Lich bosses

Mountain/Mountain Woods, in location, night - all human enemies, rare Werewolf/Wereboar/Grizzly Bear, Assassin boss

Mountain/Mountain Woods, not in location, day - all human enemies, orcs & sergeants, Grizzly Bear/Centaur/Giant/Harpy/Spriggan, Lich boss

Mountain, not in location, night - Rangers/Barbarians, Werewolf/Wereboar, Frost Daedra/Ice Atronachs, undead, Grizzly Bear/Gargoyle/Spriggan, Vampire Ancient boss

Mountain Woods, not in location, night - same enemies as Mountains not in location @ night, but less Frost Daedra/Ice 

Atronachs/Gargoyles/lycanthropes, more Spriggans/Rangers/Barbarians, Ancient Lich boss

Rainforest/Swamp, in location, night - all human enemies, rare Werewolf/Wereboar/Spider/Daedroth, Alternate Dragonling boss

Rainforest/Swamp, not in location, day - all humans & orcs, Spider/Sabertooth Tiger/Spriggan/Nymph/Daedroth/Daedra Seducer, Orc Warlord boss

Rainforest/Swamp, not in location, night - all humans & orcs, undead, Spider/Sabertooth Tiger/Giant Bat/Spriggan/Daedroth/lycanthropes, Daedra Lord boss

Subtropical, in location, night - all human enemies, rare Werewolf/Wereboar/Sabertooth Tiger, Assassin boss

Subtropical, not in location, day - all humans & orcs, Spider/Sabertooth Tiger/Spriggan/Rat/Centaur/Nymph/Daedroth/Daedra Seducer, Daedra Lord boss

Subtropical, not in location, night - all humans & orcs, Spider/Rat/Giant Bat/Sabertooth Tiger/Spriggan/Daedroth/lycanthropes, undead, Vampire Ancient boss

Woodlands, in location, night - all human enemies, rare Werewolf/Wereboar/Spriggan, Vampire boss

Woodlands, not in location, day - all human enemies, orcs & sergeants, Grizzly Bear/Spider/Centaur/Giant/Spriggan/Rat, Alternate Dragonling boss

Woodlands, not in location, night - all human enemies, orcs & sergeants, Grizzly Bear/Spider/Spriggan/Rat/Giant Bat/lycanthropes, undead, Orc Warlord boss

Haunted Woodlands, in location, night - all human enemies, Ghosts & Wraiths, rare Werewolf/Wereboar/Flesh Atronach, Vampire & Vampire Ancient bosses

Haunted Woodlands, not in location, day - undead, Spriggans, lycanthropes, Flesh Atronachs, Lich & Ancient Lich bosses

Haunted Woodlands, not in location, night - undead, lycanthropes, Flesh Atronachs, Vampire, Lich, Vampire Ancient & Ancient Lich bosses

SHOPS:

Reduced overall item generation chances in shops, shop quality now has huge impact on what's available

Fancy clothing and other expensive-looking items are only in high quality shops

Cuirasses, gauntlets, tower shields, and best-in-slot weapons are not in low quality shops

If using RPR:I you will find Wakazashis, Katanas & Dai-Katanas in low quality shops

Shops of average quality or better that are not libraries or bookstores will stock far fewer books

Shops below average quality (sturdy shelves, rusty relics) that are not libraries or bookstores will never stock books

General stores stock less armor, Booksellers stock lots more books

SKILLS:

In v1.1 I went overboard on skill advancement difficulty

Medical/Stealth/Running/Orcish/Harpy/Giantish/Daedric easier to level

All weapons/spells/Dodging/Critical Strike easier to level

Backstabbing much easier to level

Etiquette/Streetwise are exercised with human enemies even if you don't pacify them

Reduced language skill tallies from successful pacification from 3 to 1

Running will correctly level up at higher Running skill levels

SPELLS:

Regenerate effect removed from SpellMaker

Fire Daedra spell kit now includes God's Fire

Bossfall: Unleveled v1.1 Changelog

Using DaggerfallUnity 0.12.3

TLDR: Monsters, loot, human enemies unleveled. High level enemies much tougher. Some enemies move VERY fast. Some monsters immune to 
some weapon types/materials. Weapons/armor less durable. Monster variety increased. Bosses rare, difficult, rewarding. 
Weapon/spell skills harder to level, training cap is 95. Exploits/OP mechanics removed/nerfed. Guards tougher. HALT!


BOSS INFO:

Bosses have 1% chance of spawning, on average one boss every 4 block dungeon

Assassin, Alternate Dragonling, Vampire, Vampire Ancient move speed buffed

Due to limited selection boss doesn't always fit environment/dungeon type

Every boss sees Invisible

Mana for 30 spells at Levels 21-25, infinite mana at 26-30

All bosses drop loot, often loaded with Daedric

Vampire/Lich soul gems cost 750,000 gold, all other bosses 1,500,000

Vampire/Lich soul gems worth 7,500 Enchantment Points, all other bosses 15,000 (except Assassins, you can't bind human souls)

Assassins removed from aboveground (non-dungeon) vanilla quests intended to be easy


BOSS STATS:

Vampire
46-100 dmg
80-240 HP
-6 armor
Level 21-25
Spellcaster
Silver x2 dmg

Lich
50-100 dmg
80-240 HP
-7 armor
Level 21-25
Spellcaster
Silver x2 dmg

Assassin
39-126 dmg
100-300 HP
-8 armor
Level 21-30
Poisoned weapon that bypasses immunity (if using RPR:I Assassins have vanilla 60% chance of poisoned weapon)

Alternate Dragonling
76-135 dmg
130-390 HP
-9 armor
Level 21-30

Orc Warlord
40-70 dmg
150-450 HP
-10 armor
Level 21-30

Daedra Lord
50-70 dmg
170-510 HP
-11 armor
Level 26-30
Spellcaster

Vampire Ancient
101-155 dmg
180-540 HP
-12 armor
Level 26-30
Silver x2 dmg

Ancient Lich
100-170 dmg
200-600 HP
-13 armor
Level 26-30
Spellcaster
Silver x2 dmg


CHARACTER CREATION:

Resistances/Immunities more expensive

Spell Absorption too expensive to use

Everything else cheaper/same cost

Custom classes start w/Steel Longsword

Spellswords/custom wizard classes start w/Balyna's Balm

You can drop attributes to 1

Miscellaneous skills start at 1 to 4


COMBAT MECHANICS:

Enemies fire bows/spells more often & while closer & from farther away, back up & turn faster, retreat & strafe less frequently, strafe faster

Enemy accuracy buffed

Vanilla weapon material requirements to hit certain monsters removed

Some enemies immune to certain weapon types/materials & attacking them may damage your weapon

Enemy attack speed unleveled. Equal to vanilla attack speed at player level 10 with greater random variation

Body parts hit with equal likelihood

Weaponless attacks damage armor/shields of targets

All Steel weapons have minimum damage of 1

Enemies who use weapons switch to Hand-To-Hand if their weapon won't damage player in wereform

Attack fatigue cost tripled

Knockbacks nerfed to reduce stunlocks

Enemy Strength added to their Hand-to-Hand attack damage, formula same as player's


CRIME & PUNISHMENT:

Legal reputation loss for most crimes changed. Loan default more severe (-30), murder tanks rep (-50)

All fines doubled, banishment less likely

Murder carries severe penalties (minimum 12 years & 120K fine)

Dark Brotherhood chance of averting murder/assault conviction increased (if you're a guildmember)

Hostile guards use Knight animations, move faster, shoot arrows, hit harder, and see Invisible. HALT!

Guards wield weapons of any material (not true if using RPR:I). HALT!

If player is level 1-4, Guards will be up to 2 levels below or 12 levels above player. HALT! 

Once player is at least level 5, Guards are usually level 9-22 and can be up to 30. HALT!


DISEASE/POISON:

Spiders/Giant Scorpions don't paralyze, they have 22% chance/hit to inflict resistable poison, 2% chance/hit to inflict Drothweed that bypasses immunity

Enemies with poisoned weapons are 5x rarer but always bypass immunity (if using RPR:I enemies have vanilla 5% chance for poisoned weapon)

Drugs can be weapon poisons (If using RPR:I, drugs will not be weapon poisons)

Rats only give you the Plague

Vampires/Vampire Ancients can give you any disease

Skeletal Warriors have 2% chance of inflicting disease

Giant Bats have 5% chance of disease (up from 2%)

Mummies have 2% chance of disease (down from 5%)

Zombies have 5% chance of disease (up from 2%)


DUNGEON SPAWN TABLES:

Crypt - medium difficulty, undead, Wraith ranged threat, Vampire Ancient boss

Orc Stronghold - medium, Orcs, Orc Sergeants, vermin, Orc Shaman ranged threat, Orc Warlord boss

Human Stronghold - hard, lawful humans, vermin, Healer/Sorcerer ranged threat, Assassin boss

Prison - hard, criminals, vermin, Mage/Spellsword ranged threat, Assassin boss

Desecrated Temple - hard, Daedra Seducers, Imps, vermin, criminals, low level undead, Mage/Sorcerer ranged threat, Ancient Lich boss

Mine - medium, vermin, animals, Iron Atronachs/Gargoyles, Lich ranged threat and boss

Natural Cave - easy, vermin, animals, Spriggans, Rangers, Nymphs, Vampire ranged threat and boss

Coven - very hard, undead, every Daedra and every spellcasting human as ranged threats, Daedra Lord boss

Vampire Haunt - impossible, Ghosts/Wraiths as angry spirits of the dead (vampires gotta eat), lots of Vampires as ranged threats, Vampire Ancient boss

Laboratory - hard, Imps, Gargoyles, all Atronachs, all spellcasters as ranged threats, Orc Warlord boss

Harpy Nest - easy, vermin, lots of Harpies, Lich ranged threat and boss

Ruined Castle - hard, vermin, every human enemy, all spellcasters as ranged threat, Assassin boss

Spider Nest - easy (if you're immune to Poison), low level undead, lots of spiders, Ancient Lich ranged threat and boss

Giant Stronghold - hard, vermin, lots of Giants, Vampire ranged threat and boss

Dragon's Den - medium, lots of Dragonlings and their arch-nemesis Knights, Mage ranged threat, Alternate Dragonling boss

Barbarian Stronghold - medium, vermin, lots of Barbarians, Healer ranged threat, Assassin boss

Volcanic Cave - very hard, Fire Atronachs, Dragonlings, Daedra Seducers, Fire Daedra/Daedroth ranged threats, Alternate Dragonling/Daedra Lord bosses

Scorpion Nest - medium (if you're immune to Poison), vermin, lots of Giant Scorpions, Lich ranged threat and boss

Cemetery - very easy, criminals, vermin, low level undead, Vampire ranged threat and boss


ENEMIES: 

Most undead move as slow as molasses. Flying enemies move faster. Underwater enemies slower. Thief types fast. Spellcasting humans slower. Spiders, Giant Bats, Alternate Dragonlings, Assassins, Werewolves, Vampires, Vampire Ancients extremely fast

If there's ever a mod that adds new enemies, they will use vanilla movespeed formula

Human class enemy hand-to-hand/weapon damage scales with their level. Minimum dmg not changed, max dmg has (enemy level * 2) added to dmg range roll

Human class enemy armor scales with their level (Assassins excluded. Their armor doesn't change once player is at least level 5). Armor values are decreased by 5/level

Enemy skill levels have higher cap (180 rather than vanilla 100), scale up faster (+7 skill/level rather than vanilla +5/level)

All non-boss enemies have minimum damage of 1 (not true for human class enemies using Hand-To-Hand or for enemies w/Bonus to Hit: Humanoids or a Weapon Expertise)

Orcs/Centaurs aren't loot fountains

Bards/Sorcerers pacified by Streetwise

Zombies see Invisible


ENEMY LEVELS:

Human enemies unleveled (only kicks in once player is at least level 5)

3% Levels 1-5

17% Levels 6-8

60% Levels 9-12

17% Levels 13-15

3% Levels 16-20

Non-human monster levels randomly vary up and down a bit

If player is Level 1-4, all non-Guard (HALT!) human class enemies will be within 2 levels of the player

If there's ever a mod that adds new monsters or class enemies, they will be unleveled


ENEMY STATS:

Rat
1-3 dmg
6-18 HP
8 armor
Level 1-3
5% chance to inflict Plague

Imp
1-10 dmg
8-24 HP
5 armor
Level 1-4
Spellcaster

Spriggan
1-12 dmg
14-42 HP
7 armor
Level 1-5
Axe only

Giant Bat
1-7 dmg
8-24 HP
4 armor
Level 1-5
5% chance to inflict disease

Grizzly Bear
1-18 dmg
33-99 HP
7 armor
Level 2-6

Sabertooth Tiger
1-21 dmg
25-75 HP
5 armor
Level 2-6

Spider
1-8 dmg
14-42 HP
5 armor
Level 2-6
22% chance/hit to inflict resistable poison, 2% chance/hit to inflict Drothweed that bypasses immunity

Orc
1-12 dmg
24-72 HP
7 armor
Level 4-8

Centaur
1-15 dmg
20-60 HP
6 armor
Level 3-7

Werewolf
1-24 dmg
33-99 HP
0 armor
Level 10-14
Silver only

Nymph
1-10 dmg
15-45 HP
5 armor
Level 4-8

Slaughterfish
1-12 dmg
20-60 HP
5 armor
Level 5-9

Orc Sergeant
1-30 dmg
50-150 HP
1 armor
Level 11-15

Harpy
1-23 dmg
25-75 HP
5 armor
Level 6-10

Wereboar
1-48 dmg
44-132 HP
2 armor
Level 10-14
Silver only

Skeletal Warrior
1-19 dmg
17-51 HP
4 armor
Level 6-10
2% chance to inflict disease
Blunt Weapon, Hand-to-Hand, or Axe only

Giant
1-30 dmg
70-210 HP
4 armor
Level 10-14

Zombie
1-15 dmg
33-99 HP
6 armor
Level 5-9
5% chance to inflict disease
Immune to Archery/Hand-to-Hand/Short Blade
Axe x2 dmg

Ghost
1-30 dmg
5-15 HP
-4 armor
Level 9-13
Silver only

Mummy
1-25 dmg
45-135 HP
3 armor
Level 8-12
2% chance to inflict disease
Immune to Archery

Giant Scorpion
1-30 dmg
33-99 HP
1 armor
Level 10-14
Immune to Archery/Hand-to-Hand
22% chance/hit to inflict resistable poison, 2% chance/hit to inflict Drothweed that bypasses immunity

Orc Shaman
1-35 dmg
43-129 HP
0 armor
Level 11-15
Spellcaster

Gargoyle
1-50 dmg
50-150 HP
0 armor
Level 12-16
Blunt Weapon only

Wraith
1-45 dmg
10-30 HP
-8 armor
Level 13-17
Spellcaster
Silver only

Frost Daedra
1-100 dmg
35-105 HP
-1 armor
Level 15-19
Ice Bolt, Ice Storm, Frostbite

Fire Daedra
1-50 dmg
60-180 HP
-3 armor
Level 15-19
Fireball, Fire Storm

Daedroth
1-50 dmg
66-200 HP
-4 armor
Level 16-20
Spellcaster

Daedra Seducer
1-90 dmg
75-225 HP
-5 armor
Level 17-20
Spellcaster

Dragonling
1-27 dmg
25-75 HP
3 armor
Level 8-12

Fire Atronach
15-33 dmg
40-120 HP
3 armor
Level 14-18
Silver only

Iron Atronach
15-43 dmg
66-200 HP
2 armor
Level 14-18
Blunt Weapon only

Flesh Atronach
15-28 dmg
60-180 HP
4 armor
Level 14-18
Immune to Archery/Hand-to-Hand/Short Blade
Axe x2 dmg

Ice Atronach
15-38 dmg
50-150 HP
3 armor
Level 14-18
Blunt Weapon or Axe only

Dreugh
7-20 dmg
22-66 HP
5 armor
Level 6-10

Lamia
15-33 dmg
51-153 HP
-2 armor
Level 14-18


GAMEPLAY:

Training nerfed. At difficulty dagger of 1.0, my goal is 1 training/level-up until skill level 20, 2 trainings/level-up until 35, etc.

Training cap is 95

Quest rewards unleveled, you'll usually get 10-200 gold (unless it's a faction quest, then rewards are unchanged from vanilla), you can rarely get a level 20 amount

Falling damage doubled, starts at lower heights. Falling 2 blocks is 25 dmg. Damage then increases 10/meter fallen

Climbing nerfed, success depends almost entirely on your skill level

Rappel mode gives no Climbing skill checks

50 Luck no climbing skill check bonus, 1 Luck 5% penalty, 100 Luck 5% bonus

Strength weapon damage bonus halved


ITEMS:

General stores buy/sell armor

Weapon/armor durability nerfed. Durability decreases as material tier increases

Shield armor bonus doubled

Most item weights changed, gems are worth more (weapon/armor weights/costs unchanged)

Item rarities changed, best-in-slot and best-in-weapon-skill equipment rarer

Enchantment capacities of some weapons changed

Some weapon durabilities changed

Arrows actually weigh something


LOOT:

Loot unleveled, Daedric available at level 1

Rarity of all materials above Steel increased

Enemy equipment scales to their level. High level enemy = high chance of great loot

Reduced plate generation chance to 10%

Luck does not influence loot drops, chances are static

Gold piles unleveled, level 20 gold piles possible but rare

Arrows spawn w/maximum 30 per stack


MAGIC ITEMS/ENCHANTMENTS:

Found magic item durability decreased, some names changed (artifacts unchanged)

Found magic items never have "Absorbs Spells" power (artifacts still can, artifact powers unchanged)

Found magic items can have "Spell Resistance" and "Shalidor's Mirror" as Cast When Used power

Empty Soul Gems cost 50,000 gold, filled Soul Gem prices adjusted accordingly

"Repairs Objects" Enchantment removed from Item Maker

"Absorbs Spells" Enchantment removed from Item Maker

"Enhances Skill" Enchantment 10x more expensive (does not change value of "Item of Venom Spitting")

"Increased Weight Allowance" Enchantment 10x more expensive

"Featherweight" Enchantment 10x more expensive

"Strengthens Armor" Enchantment 21.5x more expensive, buffs armor by -7 rather than -5


MONSTER SPAWNING:

Spawns are random, pick from curated list of 100, list changes for each dungeon type/environment

High level enemies rare

Werewolves/Wereboars removed from dungeons

Heat-loving/tolerant monsters in desert/rainforest/subtropical wilderness, cold-loving/tolerant monsters in mountain wilderness

Orcs and humans spawn anywhere, but rarely in wilderness at night and never in daytime deserts or mountain nights

Rangers/Barbarians are outdoorsy types, in no dungeons except Natural Caves and Barbarian Strongholds, they spawn in mountain nights and desert days

Spawn frequency reduced


SKILLS:

All monster language skills much easier to level. Slightly less easy for monsters that can appear frequently (Orcs, Daedra, Giants, Harpies)

Jumping/Climbing much tougher. Running incredibly hard

Medical tougher

All Spells dramatically harder. Thaumaturgy/Restoration not more difficult than the rest

Mercantile much harder

All fighting skills (Dodging, Backstabbing, Critical Strike, all Weapons) far more difficult. Archery no longer easier than the rest

Stealth way harder


SPELLS:

Enemies cast 2 spells at levels 1-7

Enemies cast 3 spells at levels 8-12

Enemies cast 4 spells at levels 13-15

Enemies cast 5 spells at levels 16-17

Enemies cast 6 spells at levels 18-19

Enemies cast 8 spells at level 20

Bosses cast 30 spells at levels 21-25

Bosses cast infinite spells at levels 26-30

Enemy spell variety increased, every spellcaster uses same kit (except Frost & Fire Daedra). Includes Sphere of Negation

Didn't add Hand of Decay as monsters spam you with it in melee range

Enemy spell costs no longer vary based on player spell skill level

Continuous Damage: Health removed from Spellmaker. Wildfire still available for purchase, does impact dmg but nothing over time

Enemies cast Caster Only spells at range, they cast Shalidor's Mirror, Spell Resistance, Heal

Ghosts/Vampire Ancients cast no spells

If you & a monster are reflecting spells, spell will bounce between you until someone's reflection fails a save
