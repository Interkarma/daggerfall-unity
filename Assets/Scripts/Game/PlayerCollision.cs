// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
        GameObject cachedObject;
        PlayerCollisionHandler cachedCH;

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (cachedObject == hit.transform.gameObject)
                if (cachedCH)
                    cachedCH.OnCharacterCollided(hit, transform);
                else
                    return;
            else
            {
                cachedObject = hit.transform.gameObject;
                cachedCH = hit.transform.gameObject.GetComponent<PlayerCollisionHandler>();
            }
            if (cachedCH)
                cachedCH.OnCharacterCollided(hit, transform);
        }

    }
}
