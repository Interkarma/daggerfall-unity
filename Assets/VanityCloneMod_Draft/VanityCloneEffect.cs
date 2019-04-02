// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Extremely rough draft of Vanity Clone effect.
    /// Normally this effect just drops a clone of player paperdoll in world.
    /// Requires experimental paperdoll renderer that is not currently part of any live builds.
    /// Includes a fun event if cast at night, outside, under a full moon.
    /// Will rewrite this cleaner and package as a mod sometime around 0.7.x Stable.
    /// </summary>
    public class VanityCloneEffect : IncumbentEffect
    {
        public static readonly string EffectKey = "VanityClone";

        const string eventQuestFilename = "VanityCloneQuest01";
        const string nothingHappens = "Nothing happens.";
        const string cloneHeal = "Your clone heals you!";
        const string cloneCannotHeal = "You are too far away for your clone to heal you!";

        const float minMissileRespawnTime = 0.25f;
        const float maxMissileRespawnTime = 0.25f;
        const int maxMissiles = 10;

        const int coldMissileArchive = 376;
        const int fireMissileArchive = 375;
        const int magicMissileArchive = 379;
        const int poisonMissileArchive = 377;
        const int shockMissileArchive = 378;

        SongFiles eventSong = SongFiles.song_30;
        SongManager songManager = null;
        GameObject cloneObject = null;
        Quest eventQuest;
        
        bool eventRunning = false;
        int eventStage = 0;

        float witnessMe = 0;
        float missileTimer = 0;
        float nextMissileTime = 0;
        GameObject[] formationPool = new GameObject[maxMissiles];
        VanityMissile[] missilePool = new VanityMissile[maxMissiles];

        class VanityMissile
        {
            public int archive;
            public DaggerfallBillboard billboard;
            public float swirlTimer;
            public GameObject target;
            public bool returning;
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.GroupName = "Vanity Clone";
            properties.SpellMakerDescription = GetSpellMakerDescription();
            properties.SpellBookDescription = GetSpellBookDescription();
            properties.SupportDuration = true;
            properties.ShowSpellIcon = false;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(1, 1);
        }

        TextFile.Token[] GetSpellMakerDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                "Vanity Clone",
                "Summons a vanity clone of yourself to admire.",
                "Duration: Rounds your clone remains in world.",
                "Chance: N/A",
                "Magnitude: N/A");
        }

        TextFile.Token[] GetSpellBookDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                "Vanity Clone",
                "Duration: %bdr + %adr per %cld level(s)",
                "Chance: N/A",
                "Magnitude: N/A",
                "Summons a vanity clone of yourself to admire.",
                "-:WARNING:-",
                "Never cast under the light of a full moon.");
        }

        TextFile.Token[] GetEventStartMessage()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                "A full moon shines brightly above.",
                "You feel the hair stand up on the back of your neck.",
                "Something terrible is about to happen.",
                "Do you submit to your fate?");
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is VanityCloneEffect;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Do nothing if event running
            if ((incumbent as VanityCloneEffect).eventRunning)
            {
                DaggerfallUI.AddHUDText(nothingHappens);
                return;
            }

            // TODO: Reposition active clone and stack time
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);

            // Clone is not serialized, just resign on load so player can cast another
            ResignAsIncumbent();
        }

        public override int RoundsRemaining
        {
            // Don't let effect end if event running
            get { return (eventRunning) ? 1 : base.RoundsRemaining; }
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            if (eventRunning)
            {
                if (eventStage == 1)
                {
                    missileTimer += Time.deltaTime;
                    if (missileTimer > nextMissileTime)
                    {
                        missileTimer = 0;
                        nextMissileTime = Random.Range(minMissileRespawnTime, maxMissileRespawnTime);
                        CreateNextMissile();
                    }

                    GameManager.Instance.PlayerEntity.IsImmuneToParalysis = true;
                    GameManager.Instance.PlayerEntity.IsImmuneToDisease = true;

                    AnimateMissiles();
                    AnimateClone();
                    HealPlayer();
                }
                else if (eventStage == 2)
                {
                    witnessMe += 0.05f * Time.deltaTime;
                    AnimateMissiles();
                    AnimateClone();
                    if (witnessMe > 1)
                    {
                        eventStage = 0;
                        eventRunning = false;
                        RoundsRemaining = 1;
                    }
                }
            }
        }

        public override void End()
        {
            base.End();

            Cleanup();
        }

        protected override void BecomeIncumbent()
        {
            base.BecomeIncumbent();

            DaggerfallUI.Instance.PaperDollRenderer.Refresh();

            // Size of billboard with proportional width (based on rough paper doll ratio of 0.6:1)
            // Set to be taller than player capsule or it looks wrong in scene
            float width = 1.35f;
            float height = 2.25f;
            float yoffset = -0.15f;

            // Drop a new vanity clone on top of player position
            cloneObject = new GameObject("VanityClone");
            DaggerfallBillboard billboard = cloneObject.AddComponent<DaggerfallBillboard>();
            billboard.SetMaterial(DaggerfallUI.Instance.PaperDollRenderer.PaperDollTexture, new Vector2(width, height));
            cloneObject.transform.position = GameManager.Instance.PlayerObject.transform.position;

            // Align to ground - billboard is also lowered slightly or it appears to be floating
            GameObjectHelper.AlignBillboardToGround(billboard.gameObject, billboard.Summary.Size, 4);
            cloneObject.transform.position += new Vector3(0, yoffset, 0);

            // Check if player is outside, at night, under a full moon
            if ((DaggerfallUnity.Instance.WorldTime.Now.MassarLunarPhase == LunarPhases.Full ||
                DaggerfallUnity.Instance.WorldTime.Now.SecundaLunarPhase == LunarPhases.Full) &&
                !GameManager.Instance.PlayerEnterExit.IsPlayerInside &&
                DaggerfallUnity.Instance.WorldTime.Now.IsNight)
            {
                eventRunning = true;
            }
        }

        public override void MagicRound()
        {
            base.MagicRound();

            if (cloneObject && eventRunning)
            {
                switch (eventStage)
                {
                    case 0:
                        // Start event
                        eventStage = 1;
                        OfferEvent();
                        cloneObject.transform.position += new Vector3(0, 1, 0);
                        eventStage = 1;
                        break;
                    case 1:
                        // Waves
                        AssignEnemies();
                        if (eventQuest.QuestComplete)
                            eventStage = 2;
                        break;
                }
            }
        }

        void OfferEvent()
        {
            // Show a message box - not giving the player a real choice here ^.^
            DaggerfallMessageBox mb = new DaggerfallMessageBox(DaggerfallUI.Instance.UserInterfaceManager, DaggerfallUI.Instance.UserInterfaceManager.TopWindow);
            mb.SetTextTokens(GetEventStartMessage());
            mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            mb.OnButtonClick += AcceptFate;
            mb.Show();   
        }

        private void AcceptFate(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();

            // Start quest to drive most of event
            TextAsset sourceAsset = Resources.Load<TextAsset>(eventQuestFilename);
            eventQuest = QuestMachine.Instance.ParseQuest(string.Empty, sourceAsset.text.Split('\n'));
            QuestMachine.Instance.InstantiateQuest(eventQuest);

            // Play some serious music
            if (GameManager.Instance.ExteriorParent)
            {
                songManager = GameManager.Instance.ExteriorParent.GetComponentInChildren<SongManager>();
                if (songManager)
                    songManager.SongPlayer.Play(eventSong);
            }
        }

        float cloneSwirlTime = 0;
        void AnimateClone()
        {
            if (!cloneObject)
                return;

            // Swirl while waiting for enemies
            cloneSwirlTime += Time.deltaTime;
            cloneObject.transform.position += new Vector3(
                Mathf.Cos(cloneSwirlTime) * 0.01f,
                Mathf.Sin(cloneSwirlTime) * 0.01f + witnessMe,
                Mathf.Cos(cloneSwirlTime) * 0.01f);
        }

        void HealPlayer()
        {
            const float maxHealDist = 15;

            if (!cloneObject)
                return;

            float distance = Vector3.Distance(cloneObject.transform.position, GameManager.Instance.PlayerObject.transform.position);
            if (distance > maxHealDist)
            {
                DaggerfallUI.SetMidScreenText(cloneCannotHeal, 0.5f);
                return;
            }

            int curHealth = GameManager.Instance.PlayerEntity.CurrentHealth;
            int maxHealth = GameManager.Instance.PlayerEntity.MaxHealth;
            if (curHealth < maxHealth)
            {
                GameManager.Instance.PlayerEntity.CurrentHealth = maxHealth;
                DaggerfallUI.SetMidScreenText(cloneHeal);
                DaggerfallUI.Instance.PlayOneShot(SoundClips.CastSpell1);
            }
        }

        void AssignEnemies()
        {
            const string tagged = "tagged";

            List<PlayerGPS.NearbyObject> mobs = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy);
            for (int i = 0; i < mobs.Count; i++)
            {
                if (mobs[i].gameObject.name == tagged)
                    continue;

                for (int j = 0; j < missilePool.Length; j++)
                {
                    if (!missilePool[j].target)
                    {
                        missilePool[j].target = mobs[i].gameObject;
                        mobs[i].gameObject.name = tagged;
                        break;
                    }
                }
            }
        }

        void AnimateMissiles()
        {
            const float missileSpeed = 10f;

            if (!cloneObject)
                return;

            for (int i = 0; i < maxMissiles; i++)
            {
                GameObject formation = formationPool[i];
                VanityMissile missile = missilePool[i];
                if (missile == null)
                    continue;

                if (missile.target == null)
                {
                    // Swirl while waiting for enemies
                    formation.transform.position += new Vector3(
                        Mathf.Sin(missile.swirlTimer) * 0.05f,
                        Mathf.Cos(missile.swirlTimer) * 0.05f + witnessMe,
                        0);
                    missile.swirlTimer += Time.deltaTime;
                    formation.transform.RotateAround(cloneObject.transform.position, Vector3.up, 160 * Time.deltaTime);
                    missile.billboard.gameObject.transform.position = formation.transform.position;
                }
                else
                {
                    // Bounce billboard very quickly off assigned enemy
                    if (!missile.returning)
                    {
                        // Target mob
                        Vector3 direction = Vector3.Normalize(missile.target.transform.position - missile.billboard.gameObject.transform.position);
                        missile.billboard.gameObject.transform.position += direction * missileSpeed * Time.deltaTime;
                        float distance = Vector3.Distance(missile.target.transform.position, missile.billboard.gameObject.transform.position);
                        if (distance < 0.2f)
                        {
                            DaggerfallEntityBehaviour enemy = missile.target.GetComponent<DaggerfallEntityBehaviour>();
                            if (enemy)
                            {
                                // Instant kill!
                                enemy.Entity.SetHealth(0);
                                DaggerfallUI.Instance.PlayOneShot(GetSpellImpactSound(missile.archive));
                                missile.returning = true;
                            }
                        }
                    }
                    else
                    {
                        // Return to formation
                        Vector3 direction = Vector3.Normalize(formation.transform.position - missile.billboard.gameObject.transform.position);
                        missile.billboard.gameObject.transform.position += direction * missileSpeed * Time.deltaTime;
                        float distance = Vector3.Distance(formation.transform.position, missile.billboard.gameObject.transform.position);
                        if (distance < 0.2f)
                        {
                            missile.returning = false;
                            missile.target = null;
                        }
                    }
                }
            }
        }

        void CreateNextMissile()
        {
            if (!cloneObject)
                return;

            for (int i = 0; i < maxMissiles; i++)
            {
                if (missilePool[i] == null)
                {
                    formationPool[i] = new GameObject();
                    Vector3 pos = cloneObject.transform.position;
                    formationPool[i].transform.localPosition = new Vector3(pos.x + 3, pos.y + 3, pos.z + 3);
                    missilePool[i] = CreateMissile();
                    return;
                }
            }
        }

        VanityMissile CreateMissile()
        {
            VanityMissile missile = new VanityMissile();

            int archive = GetNextMissileArchive();
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, 0, null);
            DaggerfallBillboard billboard = go.GetComponent<DaggerfallBillboard>();
            billboard.FramesPerSecond = 5;
            billboard.FaceY = true;
            billboard.OneShot = false;
            billboard.GetComponent<MeshRenderer>().receiveShadows = false;

            missile.billboard = billboard;
            missile.archive = archive;

            Light light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = GetMissileColor(archive);
            light.range = 14;

            return missile;
        }

        int lastMissileArchive = 4;
        int GetNextMissileArchive()
        {
            if (++lastMissileArchive > 4)
                lastMissileArchive = 0;

            switch (lastMissileArchive)
            {
                case 0:
                    return fireMissileArchive;
                case 1:
                    return coldMissileArchive;
                case 2:
                    return magicMissileArchive;
                case 3:
                    return poisonMissileArchive;
                case 4:
                default:
                    return shockMissileArchive;
            }
        }

        Color32 GetMissileColor(int archive)
        {
            switch (archive)
            {
                case fireMissileArchive:
                    return new Color32(154, 24, 8, 255);
                case coldMissileArchive:
                    return new Color32(158, 202, 202, 255);
                case magicMissileArchive:
                    return new Color32(101, 137, 120, 255);
                case poisonMissileArchive:
                    return new Color32(101, 160, 60, 255);
                case shockMissileArchive:
                default:
                    return new Color32(68, 124, 192, 255);
            }
        }

        SoundClips GetSpellImpactSound(int archive)
        {
            switch (archive)
            {
                case fireMissileArchive:
                    return SoundClips.SpellImpactFire;
                case coldMissileArchive:
                    return SoundClips.SpellImpactCold;
                case magicMissileArchive:
                    return SoundClips.SpellImpactMagic;
                case poisonMissileArchive:
                    return SoundClips.SpellImpactPoison;
                case shockMissileArchive:
                default:
                    return SoundClips.SpellImpactShock;
            }
        }

        void Cleanup()
        {
            for(int i = 0; i < maxMissiles; i++)
            {
                if (formationPool[i])
                {
                    GameObject.Destroy(formationPool[i]);
                    formationPool[i] = null;
                }
                if (missilePool[i] != null)
                {
                    GameObject.Destroy(missilePool[i].billboard.gameObject);
                    missilePool[i] = null;
                }
            }
            if (cloneObject)
                GameObject.Destroy(cloneObject);
        }
    }
}