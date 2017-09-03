// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using System;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Macro data source base class. Extend this to provide context for macros.
    /// This abstract class provides default implementations so only applicable
    /// handlers need to be implemented.
    /// </summary>
    public abstract class MacroDataSource   
    // TODO: extract interface when complete set of handlers done? : IMacroDataSource
    {
        public virtual string Material()
        {   // %mat
            throw new NotImplementedException();
        }

        public virtual string Condition()
        {   // %qua
            throw new NotImplementedException();
        }

        public virtual string Weight()
        {   // %kg
            throw new NotImplementedException();
        }

        public virtual string WeaponDamage()
        {   // %wdm
            throw new NotImplementedException();
        }

        public virtual string Weapon()
        {   // %wep
            throw new NotImplementedException();
        }

        public virtual string ItemName()
        {
            throw new NotImplementedException();
        }

        public virtual string Modification()
        {   // %mod
            throw new NotImplementedException();
        }

        public virtual string Pronoun()
        {   // %g & %g1
            throw new NotImplementedException();
        }
        public virtual string Pronoun2()
        {   // %g2
            throw new NotImplementedException();
        }
        public virtual string Pronoun2self()
        {   // %g2self
            throw new NotImplementedException();
        }
        public virtual string Pronoun3()
        {   // %g3
            throw new NotImplementedException();
        }

        public virtual string QuestDate()
        {   // %qdt
            throw new NotImplementedException();
        }

        public virtual string Oath()
        {   // %oth
            throw new NotImplementedException();
        }

        public virtual string Region()
        {   // %reg
            throw new NotImplementedException();
        }

        public virtual string God()
        {   // %god
            throw new NotImplementedException();
        }

    }
}
