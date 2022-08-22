// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//


using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.Entity
{
    public class  PetEntity : DaggerfallEntity
    {
        #region Fields

        int careerIndex = -1;
        EntityTypes entityType = EntityTypes.None;
        MobilePet mobilePet;
        bool suppressInfighting = true;

        #endregion

        #region Properties

        public EntityTypes EntityType
        {
            get { return entityType; }
        }

        public int CareerIndex
        {
            get { return careerIndex; }
        }

        public MobilePet MobilePet
        {
            get { return mobilePet; }
        }
        #endregion

        #region Constructors

        public PetEntity(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns default entity settings.
        /// </summary>
        public override void SetEntityDefaults()
        {
        }

        /// <summary>
        /// Sets pet career and prepares entity settings.
        /// </summary>
        public void SetPetCareer(MobilePet mobilePet, EntityTypes entityType)
        {
            // Try custom career first
            career = GetCustomCareerTemplate(mobilePet.ID);

            if (career != null)
            {
                // Custom pet
                careerIndex = mobilePet.ID;
                stats.SetPermanentFromCareer(career);
                // Default like a class pet
                level = GameManager.Instance.PlayerEntity.Level;
                maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
            }
            else
            {
                career = new DFCareer();
                careerIndex = -1;
                return;
            }

            this.mobilePet = mobilePet;
            this.entityType = entityType;
            name = career.Name;

            FillVitalSigns();
        }
        #endregion
    }
}