// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Handle enemy death.
    /// </summary>
    public class EnemyDeath : MonoBehaviour
    {
        DaggerfallMobileUnit mobile;

        void Awake()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
        }

        public void Die()
        {
            Die(true);
        }

        public void Die(bool showDeathMessage)
        {
            if (mobile)
            {
                if (showDeathMessage)
                    ShowDeathMessage();

                PlaceCorpseMarker(mobile.Summary.Enemy.CorpseTexture);
                DisableEnemy();
            }
        }

        public void PlaceCorpseMarker(int corpseTexture)
        {
            // Get corpse marker texture indices
            int archive, record;
            EnemyBasics.ReverseCorpseTexture(corpseTexture, out archive, out record);

            // Spawn corpse marker
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, transform.parent, true);
            go.transform.position = transform.position;

            // Align to ground. Be generous with distance as flying enemies might have a way to drop.
            // This could also be handled by adding a Rigidbody and collider then let gravity do the work.
            // TODO: Ensure corpse markers never land on top of other monsters
            GameObjectHelper.AlignBillboardToGround(go, go.GetComponent<DaggerfallBillboard>().Summary.Size, 16f);
        }

        public void DisableEnemy()
        {
            // Disable enemy gameobject
            // Do not destroy as we must still save enemy state when dead
            gameObject.SetActive(false);
        }

        void ShowDeathMessage()
        {
            string deathMessage = HardStrings.thingJustDied;
            deathMessage = deathMessage.Replace("%s", mobile.Summary.Enemy.Name);
            DaggerfallUI.Instance.PopupMessage(deathMessage);
        }
    }
}