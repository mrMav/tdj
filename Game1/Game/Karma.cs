using System;

namespace TDJGame
{
    public static class Karma
    {

        public static float maxKarma = 0f, karma = 0f;
        public static int playerCollect = 0;
        public static int playerShotsFired = 0;
        public static float playerTotalDamage = 0f;
        public static int maxCollectables = 0;
        public static double startTime = 0;
        public static double totalTime = 0;

        public static void Reset()
        {
            maxKarma = 0f;
            karma = 0f;
            playerTotalDamage = 0f;
            playerCollect = 0;
            playerShotsFired = 0;
            maxCollectables = 0;
            totalTime = 0;
            startTime = 0;
        }

        public static void AddShotFired()
        {
            playerShotsFired += 1;
        }

        public static void AddPlayerDamage(float bDamage)
        {
            playerTotalDamage += bDamage;
        }

        public static void AddCollectable()
        {
            playerCollect += 1;
        }

        public static void ReduceKarma(float bDamage)
        {
            karma -= bDamage;
        }

        public static string DetermineRank()
        {
            if (karma == maxKarma) return "P";
            if (karma == 0) return "K";

            return "N";
        }
    }
}