// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    public abstract partial class BaseEntityEffect
    {
        EntityEffectMacroDataSource dataSource;

        public virtual MacroDataSource GetMacroDataSource()
        {
            if (dataSource == null)
                dataSource = new EntityEffectMacroDataSource(this);

            return dataSource;
        }

        private class EntityEffectMacroDataSource : MacroDataSource
        {
            readonly BaseEntityEffect parent;
            public EntityEffectMacroDataSource(BaseEntityEffect parent)
            {
                this.parent = parent;
            }

            public override string DurationBase()
            {
                return parent.Settings.DurationBase.ToString();
            }

            public override string DurationPlus()
            {
                return parent.Settings.DurationPlus.ToString();
            }

            public override string DurationPerLevel()
            {
                return parent.Settings.DurationPerLevel.ToString();
            }

            public override string ChanceBase()
            {
                return parent.Settings.ChanceBase.ToString();
            }

            public override string ChancePlus()
            {
                return parent.Settings.ChancePlus.ToString();
            }

            public override string ChancePerLevel()
            {
                return parent.Settings.ChancePerLevel.ToString();
            }

            public override string MagnitudeBaseMin()
            {
                return parent.Settings.MagnitudeBaseMin.ToString();
            }

            public override string MagnitudeBaseMax()
            {
                return parent.Settings.MagnitudeBaseMax.ToString();
            }

            public override string MagnitudePlusMin()
            {
                return parent.Settings.MagnitudePlusMin.ToString();
            }

            public override string MagnitudePlusMax()
            {
                return parent.Settings.MagnitudePlusMax.ToString();
            }

            public override string MagnitudePerLevel()
            {
                return parent.Settings.MagnitudePerLevel.ToString();
            }
        }
    }
}
