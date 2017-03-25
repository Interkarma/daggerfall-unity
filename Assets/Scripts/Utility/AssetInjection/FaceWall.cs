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
    public float maxDistance = 0.6f;

    // Translate the GameObject so that it touches the wall
    public bool AlignToWall = true;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start ()
    {
        // Get current position and rotate accordingly
        if (!HitWall(Vector3.forward))
        {
            if (HitWall(Vector3.left))
                transform.Rotate(Vector3.up, -90f);
            else if (HitWall(Vector3.right))
                transform.Rotate(Vector3.up, 90f);
            else if (HitWall(Vector3.back))
                transform.Rotate(Vector3.up, 180f);
        }

        // Align to Wall
        if (AlignToWall)
            AlignGameObjectToWall();
    }

    /// <summary>
    /// Cast ray along direction to find the wall
    /// </summary>
    /// <param name="direction">Direction to look for the wall</param>
    /// <returns>True if wall is found</returns>
    private bool HitWall (Vector3 direction)
    {
        // Transforms direction from local space to world space.
        direction = transform.TransformDirection(direction);

        // Casts a ray from the GameObject and check if it intersect 
        // with a collider in the specified direction.
        return Physics.Raycast(gameObject.transform.position, direction, maxDistance);
    }

    /// <summary>
    /// Translate the GameObject so that it touches the wall
    /// </summary>
    private void AlignGameObjectToWall()
    {
        // Find point on wall
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, maxDistance))
            return;

        // Find nearest point on collider
        Vector3 pointOnGameObject = GetComponent<Collider>().ClosestPointOnBounds(hitInfo.point);
        pointOnGameObject.y = transform.position.y;

        // Get distance
        float distance = Vector3.Distance(pointOnGameObject, hitInfo.point);

        // Translate GameObject near wall
        transform.Translate(0, 0, distance);
    }
}
