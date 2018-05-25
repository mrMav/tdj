using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using System;

namespace TDJGame
{
    public static class Karma
    {

        public static float maxKarma, karma;
        public static int playerCollect;
        public static int playerShotsFired = 0;
        public static float playerTotalDamage = 0f;

        public static int ShotFired(int playerShotsFired)
        {
            playerShotsFired += 1;
            Console.WriteLine("Shots fired by player: " + playerShotsFired);
            return playerShotsFired;
        }

        public static float AddPlayerDamage(float playerTotalDamage, float bDamage)
        {
            playerTotalDamage += bDamage;
            Console.WriteLine("Total dmg: " + playerTotalDamage);
            return playerTotalDamage;
        }

        public static int TotalCollectables(int playerCollect)
        {
            playerCollect += 1;
            Console.WriteLine("Collectables: " + playerCollect);
            return playerCollect;
        }

        public static float DefineMaxKarma(float maxKarma, float enemyHealth)
        {
            maxKarma += enemyHealth;
            Console.WriteLine("MaxKarma: " + maxKarma);
            return maxKarma;
        }

        public static float ReduceKarma(float karma, float bDamage)
        {
            karma -= (float)bDamage;
            Console.WriteLine("Karma: " + karma);
            return karma;
        }
    }
}
