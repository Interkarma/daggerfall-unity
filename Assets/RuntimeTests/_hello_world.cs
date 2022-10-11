using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class _hello_world
{

    [UnityTest]
    public IEnumerator RigidBody_can_be_simulated ()
    {
        // create test setup:
        var go = new GameObject();
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = new Vector3( 0 , -1 , 0 );
        
        // measurements:
        Vector3 before = go.transform.position;
        {
            if( Physics.autoSimulation ) yield return new WaitForFixedUpdate();
            else Physics.Simulate( Time.fixedDeltaTime );
        }
        Vector3 after = go.transform.position;

        // assertions on measurement results:
        Assert.AreNotEqual( before , after );
    }

}
