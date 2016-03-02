// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Game
{

    public class PlayerCollision : MonoBehaviour
    {
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            PlayerCollisionHandler ch = hit.gameObject.GetComponent<PlayerCollisionHandler>();
            if (ch)
                ch.OnCharacterCollided(hit, transform);
        }

    }
}
