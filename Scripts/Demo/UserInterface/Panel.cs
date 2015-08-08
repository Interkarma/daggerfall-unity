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
using System.Collections.Generic;
using System.Text;

namespace DaggerfallWorkshop.Demo.UserInterface
{
    /// <summary>
    /// A screen component that contains other screen components.
    /// </summary>
    public class Panel : BaseScreenComponent
    {
        ScreenComponentCollection components;
        public ScreenComponentCollection Components
        {
            get { return components; }
        }

        public Panel()
            :base()
        {
            this.components = new ScreenComponentCollection(this);
        }

        public override void Update()
        {
            base.Update();

            // Update child components
            foreach (BaseScreenComponent component in components)
            {
                component.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Draw child components
            foreach (BaseScreenComponent component in components)
            {
                component.Draw();
            }
        }
    }
}
