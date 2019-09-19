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
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Handles magical concealment for entities other than player.
    /// Inherit from this class for custom entity concealment handling.
    /// </summary>
    [RequireComponent(typeof(DaggerfallEntityBehaviour))]
    [RequireComponent(typeof(EntityEffectManager))]
    public class EntityConcealmentBehaviour : MonoBehaviour
    {
        protected DaggerfallEntityBehaviour entityBehaviour;
        protected EntityEffectManager entityEffectManager;
        protected MeshRenderer meshRenderer;


        private bool isConcealed = false;

        public bool IsConcealed
        {
            get { return (isConcealed); }
        }

        // Override any concealment to force billboard to always be hidden
        public bool forceConcealment { get; set; }
        
        #region Unity

        void Awake()
        {
            CacheReferences();
        }

        void Update()
        {
            if (!entityBehaviour || entityBehaviour.Entity == null)
                return;

            isConcealed = (entityBehaviour && entityBehaviour.Entity.IsMagicallyConcealed);
            MakeConcealed(isConcealed || forceConcealment);
        }

        #endregion

        #region Protected Methods

        protected void CacheReferences()
        {
            entityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            entityEffectManager = GetComponent<EntityEffectManager>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        protected void MakeConcealed(bool concealed)
        {
            if (meshRenderer)
            {
                meshRenderer.enabled = !concealed;
            }
        }

        #endregion
    }
}
