using System.Collections.Generic;

namespace Akua.Tool.ShuffleUtil
{
    public static class ShuffleUtil
    {
        public static T GetOneObjectFromWeightedRandom<T>(this List<T> pool, List<float> weights, float totalWeight = -1)
        {
            if (weights.Count <= 0 || pool.Count <= 0)
            {
                throw new System.Exception("Input List cannot be empty");
            }

            if (pool.Count != weights.Count)
            {
                throw new System.Exception("Input List must be the same size");
            }
            
            if (totalWeight < 0)
            {
                foreach (var weight in weights)
                {
                    totalWeight += weight;
                }
            }

            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            for (int i = 0; i < pool.Count; i++)
            {
                cumulativeWeight += weights[i];
                if (randomValue <= cumulativeWeight)
                {
                    return pool[i];
                }
            }

            // Fallback, should not reach here
            return pool[^1];
        }

        /// <summary>
        /// Shuffles the element order of the specified list using the Fisher-Yates algorithm.
        /// Works for both List<T> and T[].
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            // Loop backwards through the list
            while (n > 1)
            {
                n--;
                // Pick a random index from 0 to n
                int k = UnityEngine.Random.Range(0, n + 1);

                // Swap list[k] with list[n]
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static List<T> ShuffleUnChanged<T>(this IList<T> input)
        {
            List<T> list = new List<T>(input);

            int n = list.Count;
            // Loop backwards through the list
            while (n > 1)
            {
                n--;
                // Pick a random index from 0 to n
                int k = UnityEngine.Random.Range(0, n + 1);

                // Swap list[k] with list[n]
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }
    }
}