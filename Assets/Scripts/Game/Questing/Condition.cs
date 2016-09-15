using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Conditions are executed to check if something has happened in relation to quest.
    /// Condition checks seem to operate continuously once initiated.
    /// For example, the "pc at" condition for _exitstarter_ in _BRISIEN is
    ///   flagged true/false anytime player enters/exits Privateer's Hold.
    /// </summary>
    public class Condition
    {
    }
}