// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Example enemy death.
    /// </summary>
    public class EnemyDeath : MonoBehaviour
    {
        DaggerfallMobileUnit mobile;

        void Start()
        {
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();
        }

        public void Die()
        {
            if (!mobile)
                return;

            // Get corpse marker texture indices
            int archive, record;
            EnemyBasics.ReverseCorpseTexture(mobile.Summary.Enemy.CorpseTexture, out archive, out record);

            // Leave corpse marker
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (dfUnity)
            {
                // Spawn marker
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, transform.parent, true);
                go.transform.position = transform.position;

                // Align to ground. Be generous with distance as flying enemies might have a way to drop.
                // This could also be hanlded by adding a Rigidbody and collider then let gravity do the work.
                GameObjectHelper.AlignBillboardToGround(go, go.GetComponent<DaggerfallBillboard>().Summary.Size, 16f);
            }

            // Disable enemy gameobject and schedule for destruction
            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);
        }
    }
}