// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.FallExe;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by the Sanguine Rose. Spawns a Daedroth to attack the target the player is facing.
    /// </summary>
    public class SanguineRoseEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "SanguineRoseEffect";
        const float RayDistance = 3072 * MeshReader.GlobalScale;

        public override void MagicRound()
        {
            base.MagicRound();
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            Vector3 origin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            Ray ray = new Ray(origin + Vector3.up * 0.8f, mainCamera.transform.forward);
            RaycastHit hit;
            int playerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
            bool hitSomething = Physics.Raycast(ray, out hit, RayDistance, playerLayerMask);
            if (hitSomething)
            {
                // Check for mobile enemy hit
                DaggerfallEntityBehaviour target = hit.transform.GetComponent<DaggerfallEntityBehaviour>();
                if (target != null)
                {
                    // Get enemy
                    EnemyEntity enemyEntity = target.Entity as EnemyEntity;
                    if (!enemyEntity.MobileEnemy.AlliedToPlayer) // Only spawn when aiming at a non-allied enemy
                    {
                        GameObject gameObject = GameObjectHelper.CreateFoeSpawner(foeType: MobileTypes.Daedroth, spawnCount: 1, alliedToPlayer: true);
                        FoeSpawner foeSpawner = gameObject.GetComponent<FoeSpawner>();
                        return;
                    }
                }
            }
            DaggerfallUI.Instance.PopupMessage(HardStrings.noSummonedMonster);
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            bypassSavingThrows = true;
        }
    }
}
