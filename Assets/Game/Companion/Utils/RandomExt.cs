using UnityEngine;

namespace DaggerfallWorkshop.Game.Companion.Utils
{
    public static class RandomExt
    {
        /// <summary>
        ///Returns a random point on the surface of a circle with radius 1 and y == 0(Read Only).
        /// </summary>
        public static Vector3 OnUnitCircle
        {
            get { return Quaternion.Euler(0, Random.Range(-180f, 180f), 0) * Vector3.forward; }
        }
    }
}