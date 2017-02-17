// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes: version 1.0
//

using UnityEngine;

/// <summary>
/// Rotate a GameObject so that it appears hung on the wall.
/// The GameObject will face the wall along the Unity Z/Blue axis of the model.
/// This is useful for models meant to replace sprites as they miss the rotation component.
/// This script needs to be added as a component in the inspector, only for models wich
/// actually require this adjustment (like torches).
/// </summary>
public class FaceWall : MonoBehaviour
{
    // Max distance between the GameObject and the wall. 
    // A big value can cause wrong results.
    float maxDistance = 0.6f;

    // Initialization
    void Start ()
    {
        // Get GameObject Transform
        Transform transform = gameObject.transform;

        // Get current position and rotate accordingly
        if (HitWall(Vector3.forward))
            return;
        else if (HitWall(Vector3.left))
            transform.Rotate(Vector3.up, -90f);
        else if (HitWall(Vector3.right))
            transform.Rotate(Vector3.up, 90f);
        else if (HitWall(Vector3.back))
            transform.Rotate(Vector3.up, 180f);
    }

    // Cast ray along direction to find the wall
    bool HitWall (Vector3 direction)
    {
        // Transforms direction from local space to world space.
        direction = transform.TransformDirection(direction);

        // Casts a ray from the GameObject and check if it intersect 
        // with a collider in the specified direction.
        if (Physics.Raycast(gameObject.transform.position, direction, maxDistance))
            return true;
        else
            return false;
    }
}
