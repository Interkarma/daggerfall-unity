Add localized quest files to this folder. See StreamingAssets/Quests for master files.
Quest text found in these files will override quest text in master quest scripts.
Quest DisplayName: value in these file will overrride quest DisplayName in master quest scripts.

Files placed in this folder must have the following format:

* Must be a UTF-8 .txt file.
* Filename must end in -LOC (e.g. "M0B00Y16-LOC.txt").
* File must begin with a header block for DisplayName: followed by QRC: then Message: contents.
* Only QRC part of quest localization file will be read here. No quest logic (QBN part) will be used and should be discarded.
* Localization parser will discard anything after QBN: in source file.

<<<<EXAMPLE FILE FOLLOWS>>>>

Quest: M0B00Y16
DisplayName: Giant Killing
QRC:

QuestorOffer:  [1000]
<ce>               Howdy, %pcf.  If you got a few days free,
<ce>            a job just came in for some giant-killin', and
<ce>                    I thought of you.  Interested?

RefuseQuest:  [1001]
<ce>                  Too busy, eh?  When I was your age,
<ce>                  I was never too busy to off a giant
<ce>                 or two.  Kids these days, I tell ya.

AcceptQuest:  [1002]
<ce>                Right.  Seems there's this giant, moved
<ce>                   into ___mondung_ a while back and
<ce>               has been having a fine old time feastin'
<ce>                 on _dummy_'s sheep in the area.  Ate
<ce>              a few of the local villagers too, I guess,
<ce>                but old _dummy_ don't seem so concerned
<ce>             about that.  At any rate, %g wants this giant
<ce>          taken care of within =qtime_ days, and %g's payin'
<ce>                 _gold_ gold for it.  See ya in a bit.

QuestFail:  [1003]
<ce>                  What, the giant died of overeating?
<ce>                Get going, %pcf.  You got a job to do.

QuestComplete:  [1004]
<ce>                  Hail the giant-killer!  I knew you
<ce>                    was the right one for the job.
<ce>                        Here's the _gold_ gold.

RumorsDuringQuest:  [1005]
<ce> A giant in ___mondung_ is terrorizing the locals, but _dummy_ doesn't
<ce>                             seem to care.
                                     <--->
<ce> That giant in ___mondung_ carried off another villager the other day.

RumorsPostfailure:  [1006]
<ce> I'm glad that giant finally moved out of ___mondung_.  I guess it got
<ce>       bored with it's bland diet of villager night after night.

RumorsPostsuccess:  [1007]
<ce>      Thank %god _dummy_ finally sent someone to kill that giant
<ce>    in ___mondung_.  It must have killed one of %g3 precious sheep.

QuestorPostsuccess:  [1008]
<ce>                 It's my old friend Giant-Killer %pcf!

QuestorPostfailure:  [1009]
<ce>                In my day, you could get kicked out of
<ce>               the Fighter's Guild if you couldn't kill
<ce>                a simple giant.  Times change, I guess.

Message:  1020
%qdt:
 _questgiver_ of the Fighter's Guild
 of ___questgiver_ has hired me to
 kill a giant in ___mondung_.
 I am to report back within =qtime_ days
 for my reward of _gold_ gold.

Message:  1021
%qdt:
 I rescued a merchant the giant was
 apparently saving for a snack.
 _villager_ asked me to take %g2
 to _store_ in __store_.

Message:  1030
<ce>                  The giant roars in pain.  You hear
<ce>                  the sound of heavy footsteps coming
<ce>                    towards you.  Perhaps the giant
<ce>               invited a few friends over for dinner...

Message:  1031
<ce>  You have slain the giant you came for.

Message:  1040
<ce>                    Thank %god you're here!  I was
<ce>                     on my way to market yesterday
<ce>                  when the giant carried me off along
<ce>                    with four other merchants I was
<ce>                travelling with.  It already ate them,
<ce>                  I guess it was saving me for later.
<ce>                Will you please help me get out of this
<ce>               place?  I'll never make it out on my own.

Message:  1041
<ce>             You're a true hero.  Let me introduce myself:
<ce>                      _villager_ of ___villager_.
<ce>                     I was on my way to _store_ in
<ce>                    __store_.  If you could drop me
<ce>              off there, I would be forever in your debt.

Message:  1042
<ce>                  I appreciate your help, but I think
<ce>                   I can find my own way from here.

Message:  1043
<ce>                       You are truly heartless.

Message:  1044
<ce>                   I've enjoyed your company, %pcf.
<ce>               You saved my life, and I won't forget it.

Message:  1080
<ce>                   Hey, here's a map to ___newdung_
<ce>                   one of the other chaps found the
<ce>                  other day.  If I was a couple years
<ce>                   younger, we could tear through it
<ce>                   together, just like the old days.

<<<<END EXAMPLE FILE>>>>

Notes:
* When translating quest files, do not translate field names (e.g. DisplayName:, QRC:, Message:, RumorsDuringQuest:, etc.) or formatting text (e.g. <ce>, __var_, etc.).
* Master quest scripts can be updated over time. If this involves text or Message: ID, you might need to update your translated text.