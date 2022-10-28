// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Andrzej Łukasik (andrew.r.lukasik)
// Contributors:    
// 
// Notes:
//

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class _Physics
{

    [UnityTest]
    public IEnumerator RigidBody_velocity_moves_an_object()
    {
        GameObject gameobject = new GameObject(nameof(RigidBody_velocity_moves_an_object));
        try
        {
            // test setup:
            Rigidbody rigidbody = gameobject.AddComponent<Rigidbody>();
            rigidbody.position = Vector3.zero;
            rigidbody.velocity = new Vector3(0, 1, 0);

            // step simulation:
            Vector3 before = rigidbody.position;
            if (Physics.autoSimulation) yield return new WaitForFixedUpdate();
            else Physics.Simulate(Time.fixedDeltaTime);
            Vector3 after = rigidbody.position;
            Debug.Log($"Physics.autoSimulation: {Physics.autoSimulation}");

            // assertions on measurements:
            float distanceMoved = Vector3.Distance(after, before);
            Debug.Log($"distance moved: {distanceMoved}");
            Assert.Greater(distanceMoved, 0);
        }
        finally// cleanup
        {
            GameObject.Destroy(gameobject);
        }
    }

    [UnityTest]
    public IEnumerator Raycast_hits_a_BoxCollider()
    {
        var gameobject = new GameObject(nameof(Raycast_hits_a_BoxCollider));
        try
        {
            int layer = 7;
            var collider = gameobject.AddComponent<BoxCollider>();
            collider.transform.position = Vector3.zero;
            collider.size = new Vector3(1, 1, 1);
            collider.gameObject.layer = layer;

            Debug.Log($"collider.transform.position: {collider.transform.position}");
            Debug.Log($"collider.size: {collider.size}");

            yield return null;
            float rayMaxDistance = 2f;
            var ray = new Ray(new Vector3(0, 2, 0), Vector3.down);
            bool raycastConnected = Physics.Raycast(ray, out var hit, rayMaxDistance, 1 << layer);

            Debug.Log($"ray.origin: {ray.origin}");
            Debug.Log($"ray.direction: {ray.direction}");
            Debug.Log($"raycast connected: {raycastConnected}");
            Debug.Log($"hit.point: {hit.point}");

            Assert.True(raycastConnected, "raycast hit nothing");
            Assert.AreSame(collider, hit.collider, $"raycast hit a wrong collider");
        }
        finally// cleanup
        {
            GameObject.Destroy(gameobject);
        }
    }

    [UnityTest]
    public IEnumerator Raycast_hits_a_SphereCollider()
    {
        var gameobject = new GameObject(nameof(Raycast_hits_a_SphereCollider));
        try
        {
            int layer = 7;
            var collider = gameobject.AddComponent<SphereCollider>();
            collider.transform.position = Vector3.zero;
            collider.radius = 1;
            collider.gameObject.layer = layer;

            Debug.Log($"collider.transform.position: {collider.transform.position}");
            Debug.Log($"collider.radius: {collider.radius}");

            yield return null;
            float rayMaxDistance = 2f;
            var ray = new Ray(new Vector3(0, 2, 0), Vector3.down);
            bool raycastConnected = Physics.Raycast(ray, out var hit, rayMaxDistance, 1 << layer);

            Debug.Log($"ray.origin: {ray.origin}");
            Debug.Log($"ray.direction: {ray.direction}");
            Debug.Log($"raycast connected: {raycastConnected}");
            Debug.Log($"hit.point: {hit.point}");

            Assert.True(raycastConnected, "Physics.Raycast hit nothing");
        }
        finally// cleanup
        {
            GameObject.Destroy(gameobject);
        }
    }

    [UnityTest]
    public IEnumerator Raycast_hits_a_MeshCollider()
    {
        var gameobject = new GameObject(nameof(Raycast_hits_a_MeshCollider));
        try
        {
            int layer = 7;
            var collider = gameobject.AddComponent<MeshCollider>();
            collider.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            collider.transform.position = Vector3.zero;
            collider.gameObject.layer = layer;

            Debug.Log($"collider.transform.position: {collider.transform.position}");

            yield return null;

            float rayMaxDistance = 2f;
            var ray = new Ray(new Vector3(0, 2, 0), Vector3.down);
            bool raycastConnected = Physics.Raycast(ray, out var hit, rayMaxDistance, 1 << layer);

            Debug.Log($"ray.origin: {ray.origin}");
            Debug.Log($"ray.direction: {ray.direction}");
            Debug.Log($"raycast connected: {raycastConnected}");
            Debug.Log($"hit.point: {hit.point}");

            Assert.True(raycastConnected, "Physics.Raycast hit nothing");
        }
        finally// cleanup
        {
            GameObject.Destroy(gameobject);
        }
    }

}
