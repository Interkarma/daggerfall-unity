DFBIOGFIX v1.2 - Daggerfall Biography Fix

	by Frank 'Deepfighter' Schwalb
	- deepfighter@tamriel-almanach.de -

 ~ A fix for the biography files in The Elder Scrolls II: Daggerfall. ~

==============================================================================
                             C O N T E N T S
==============================================================================

    1. Introduction
    2. Changelog
    3. Installation
    4. Thanks and Disclaimer
    5. Changes overview

==============================================================================
  1. Introduction                                                            
==============================================================================

 The idea to fix the errors in the BIOGXXT0 files of Daggerfall came during
 the German translation process. While translating the files I found some
 strange mistakes which ranged from spell errors, to wrong values, to
 different outcome of chosen equipment. Over the years nobody seemed to check
 all the files for errors. There was a small fix for the BIOG00TO.TXT file,
 by Uniblab, but he missed the same mistake in another question of the same
 file. All in all it was incomplete.
 
 That was the main reason to check through all 216 questions and find errors.
 I found more inconsistencies than I fixed, because some are intended and some
 changes would mean some in-depth balance changes. For that reason I made some
 guidelines for this (first) fix (v1.0):
 
  * Change 'just' the obvious mistakes.
  * Try to keep as original as possible.
 
 And I did as you will see in my list of changes below. 
 
 On this basis there are much possibilities. We can fix more or less other
 balance issues and inconsistencies, which would otherwise not be as
 original as possible (as for example the missing "Orcish" skill in the
 "[...]friendlier than most [...]" question or some misleading outcome
 with "Critical Strike" and "Hand-to-Hand" which I left out due to my
 guidelines). Everything is already reported and can be easily changed.
 
 For v1.2 I included further fixes especially, where you choose "Archery"
 as an answer and get no corresponding bow to shoot the associated arrows.
 I also changed a file where you could acquire two (redundant) Katana's
 or where an appropriate weapon has been missing.
 
 Feel free to discuss with me about the changes or just write if you found
 other mistakes in Daggerfall, or my files. Everyone will be credited
 of course. This should be more or less a daggerfall community fix.
 
 Regards
 Frank Schwalb aka Deepfighter
 
==============================================================================
  2. Changelog                                                            
==============================================================================

 (11/10/15) - v1.0 - Release of English and German version
 (23/10/15) - v1.1 - Added French version
 (12/05/20) - v1.2 - Changed some further inconsistencies

==============================================================================
  3. Installation                                                            
==============================================================================

 Just copy the files of your language into the ARENA2 folder. If you are asked
 to replace the old files, do that. If needed, do a backup of your old files
 before replacing them, otherwise the original files should be in the archive
 you downloaded. Just take them and replace them the same way to get the
 original files in your language back.
 
 Daggerfall Unity
 ----------------
 It's planned that these fix will be implemented as a core feature (Github
 Issue #1468). Until this is in place Daggerfall Unity gets the information
 from these files directly from the ARENA2 folder of the associated game
 files, which means that you should patch them as stated above.

==============================================================================
  4. Thanks and Disclaimer
==============================================================================

 * Ted Peterson and Julien LeFay, the true minds behind The Elder Scrolls.
 * Thanks to Gavin Clayton (Interkarma) and the DF Workshop Community.
 * Thanks to Numenorean for doing so much on the German translation, that I've
   got the time to do my side Daggerfall-projects, whenever I have to.
 * Special thanks to Uniblab and PLRDLF for their incredible work on the UESP
   pages for Daggerfall.
 * To everyone else who loves and contributes to Daggerfall (in)directly and
   the big and gorgeous Daggerfall community. We will have a great time to
   work on the game.

 Contact: deepfighter@tamriel-almanach.de

==============================================================================
  5. Overview of Changes
==============================================================================

 A visual overview of all the changes, and a general overview for all the
 questions + a place for adding comments:
 https://docs.google.com/spreadsheets/d/1MyDLmIMQ1G20QWbkGWQg7Pxl07TAJGM_
 VsLN-b025_o/edit

 -----------------
 
# BIOG00T0.TXT {Mages}

* #3 c) The answer "practising acrobatics" is incorrectly associated with the
  skill 'Orcish'.
**Changed it to the appropriate skill 'Jumping'.

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.

* #7 a) The answer "Rabbit" is incorrectly associated with the skill
  'Orcish', even if it's indicate athleticism.
**Changed it to the appropriate skill 'Jumping' as it is in the other
  original files with the same question; it makes more sense.

* #7 c) The answer "Quicksilver" is incorrectly associated with the skill
  'Resoration', even if it's indicate quickness.
**Changed it to the appropriate skill 'Running' as it is in the other
  original files with the same question; it makes more sense.


# BIOG01T0.TXT {Spellsword}

* #3 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.

* #7 f) If you are selecting "Longblade" you get six skill points, but no
  weapon, while for all other answers (a-e) a corresponding weapon is added.
**Changed it to add an extra 'Iron Long Sword'.


# BIOG02T0.TXT {Battlemage}

* #3 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.


# BIOG03T0.TXT {Sorcerer}

* #3 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.


# BIOG04T0.TXT {Healer}

* #3 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.


# BIOG05T0.TXT {Nightblade}

* #3 e) The question "Where is your expertise in combat?" has the answer
  "Archery" which gives you 'Iron Left Pauldron' instead of a bow weapon.
**Changed the outcome to 'Iron Short Bow' and 6x 'Iron Arrow'.

* #7 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.


# BIOG06T0.TXT {Bard}

* #4 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.

* #6 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.


# BIOG07T0.TXT {Burglar}

* #5 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.

* #7 a) The question "Where is your expertise in combat?" has the answer
  "Short-bladed weapons" which gives you 'Iron Gauntlets' instead of a short-
  bladed weapon.
**Changed the outcome to 'Iron Short Sword'.

* #7 b) The question "Where is your expertise in combat?" has the answer
  "Hand-to-hand combat" which gives you 'Steel Cuirass' instead of help for
  Hand-to-hand combat.
**Changed the outcome to 'Iron Gauntlets'.

* #7 e) The question "Where is your expertise in combat?" has the answer
  "Archery" which gives you 'Iron Left Pauldron' instead of a bow weapon.
**Changed the outcome to 'Iron Short Bow' and 6x 'Iron Arrow'.


# BIOG08T0.TXT {Rogue}

* #1 b) The question "You have a certain degree of training with ___." has
  the answer "Hand-to-hand combat" which gives you 'Silver Cuirass' instead
  of help for Hand-to-hand combat.
**Changed the outcome to 'Silver Gauntlets'.

* #1 e) The question "You have a certain degree of training with ___." has
  the answer "Archery" which gives you 'Iron Left Pauldron' instead of
  a corresponding weapon, like the other answers.
**Changed the outcome to 'Iron Short Bow' and 6x 'Iron Arrow'.


# BIOG10T0.TXT {Thief}

* #3 e) The question "Where is your expertise in combat?" has the answer
  "Archery" which gives you 'Iron Left Pauldron' instead of a bow weapon.
**Changed the outcome to 'Iron Short Bow' and 6x 'Iron Arrow'.

* #4 a) For the answer "Over 200 gold pieces" you'll get just 200 gold pieces
  and not like indicated 200+.
**Changed it to 250 gold pieces.

* #8 c) The answer "practising acrobatics" is next to 'Jumping' and 'Dodging'
  inconsistent associated with the skill 'Running'.
**Changed it to the appropriate skill 'Climbing' like in BIOG00T0.TXT.


# BIOG14T0.TXT {Ranger}

* #1 c) The answer "Axe fighting" is associated with 'Iron Gauntlets' instead
  of an appropriate weapon which fits to the skill.
**Changed it to 'Iron Axe'.

* #1 f) The answer "Longblade" is associated with 'Iron Katana'. As you'll
  have the chance to get a Dai-Katana in #5 e), it seems to be redundant
  aquiring two Katana's.
**Changed it to 'Iron Long Sword'.

# BIOG16T0.TXT {Warrior}

* #1 f) The answer "Longblade" is associated with 'Iron Kite Shield' instead
  of an appropriate weapon which fits to the skill.
**Changed it to 'Iron Claymore' like in BIOG15T0.TXT.

* #9 The answers are messed up as the associated values of the different
  skills are strangely [-2]. That means that e.g. the answer g) "the primitive
  Centaurs" is associated with the skill 'Daedric' and answer e) "the infernal
  Daedra" with the skill 'Dragonish' etc.
**Changed every answer to his respective skill; Added to each the appropriate
  value [+2].

______________________________________________________________________________
THE UNDERKING FINDS             DFBIOGFIX © F. Schwalb (Deepfighter) 2015-2020
YOU AND BRINGS YOU              The Elder Scrolls namesake © respective owners
FAR, FAR, FAR AWAY...                            E N D   O F   D O C U M E N T
