using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Constrains Y rotation.
    /// Used to stop particle effects from rotating with player.
    /// </summary>
    public class ConstrainYRotation : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}