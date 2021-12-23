using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Utilities
{
    static class RandomNumber
    {
        private static Random r = new Random();
        private static Object l = new Object();

        private static Random globalRandom = new Random();
        [ThreadStatic]
        private static Random localRandom;

        /// <summary>
        /// Generates a new random number, each time this is called a new Random class is initialized.
        /// If there is memory issues, please use the GenerateGuaranteedRandom method.
        /// </summary>
        /// <param name="min">Minimum value to return, usually 0.</param>
        /// <param name="max">Maximum value to return.</param>
        /// <returns>Random generated number.</returns>
        public static int GenerateNewRandom(int min, int max)
        {
            return new Random().Next(min, max);
        }

        /// <summary>
        /// Generates a guaranteed random number using the shared random class by only allowing one entry at a time.
        /// It achieves this by locking onto an shared object, generates a random number, returns it then allows another entry.
        /// </summary>
        /// <param name="min">Minimum value to return, usually 0.</param>
        /// <param name="max">Maximum value to return.</param>
        /// <returns>Random generated number.</returns>
        public static int GenerateLockedRandom(int min, int max)
        {
            lock (l) // only allow one entry at a time to prevent returning the same number to multiple entries.
            {
                return r.Next(min, max);
            }
        }

        public static int GenerateRandom(int min, int max)
        {
            max = max + 1;

            Random inst = localRandom;

            if (inst == null)
            {
                int seed;
                lock (globalRandom)
                {
                    seed = globalRandom.Next();
                }
                localRandom = inst = new Random(seed);
            }
            return inst.Next(min, max);
        }
    }
}
