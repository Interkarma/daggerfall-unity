using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Stores enemy settings for serialization and other tasks.
    /// </summary>
    public class DaggerfallEnemy : MonoBehaviour
    {
        long loadID = 0;

        public long LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }
    }
}