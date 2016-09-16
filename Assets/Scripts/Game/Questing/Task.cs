// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Tasks are subsets of execution flow triggered by other tasks or clock timeouts. Somewhat like a subroutine.
    /// Tasks can contain conditions at start that if not met appear to prevent further execution of task.
    /// Most tasks need to be executed directly, but certain tasks (e.g. until _variable_ performed) will
    ///   start and continue to check condition until they can execute.
    /// Tasks expose a flag stating if they have been completed or not.
    /// Variables are a special task with no conditions or actions, just set/unset.
    /// </summary>
    public class Task
    {
    }
}