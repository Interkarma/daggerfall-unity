using UnityEngine;
namespace DaggerfallWorkshop.Game.Utility
{
    public class Dice100
    {
        private Dice100()
        {
        }

        public static int Roll()
        {
            return Random.Range(1, 101);
        }

        public static bool SuccessRoll(int chanceSuccess)
        {
            return Random.Range(0, 100) < chanceSuccess; // Same as Random.Range(1, 101) <= chanceSuccess
        }

        public static bool FailedRoll(int chanceSuccess)
        {
            return Random.Range(0, 100) >= chanceSuccess; // Same as Random.Range(1, 101) > chanceSuccess
        }
    }
}
