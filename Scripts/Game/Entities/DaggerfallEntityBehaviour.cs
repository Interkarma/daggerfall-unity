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
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Hosts DaggerfallEntity for living GameObjects.
    /// </summary>
    public class DaggerfallEntityBehaviour : MonoBehaviour
    {
        #region Fields

        public EntityTypes EntityType = EntityTypes.None;

        EntityTypes lastEntityType = EntityTypes.None;
        DaggerfallEntity entity = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets entity as PlayerEntity.
        /// </summary>
        public DaggerfallEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetEntityType(EntityType);
        }

        void Update()
        {
            if (EntityType != lastEntityType)
            {
                SetEntityType(EntityType);
                lastEntityType = EntityType;
            }
        }

        #endregion

        #region Private Methods

        void SetEntityType(EntityTypes type)
        {
            switch(type)
            {
                case EntityTypes.None:
                    entity = null;
                    break;
                case EntityTypes.Player:
                    entity = new PlayerEntity();
                    break;
            }

            if (entity != null)
                entity.SetEntityDefaults();
        }

        #endregion
    }
}