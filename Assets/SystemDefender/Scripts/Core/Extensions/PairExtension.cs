using SystemDefender.Scripts.Core.Structures;
using UnityEngine;

namespace SystemDefender.Scripts.Core
{
    public static class PairExtension
    {
        public static float Distance<F, S>(this Pair<F, S> __thisPair, Pair<F, S> __anotherPair)
        {
            if (int.TryParse(__thisPair.firstItem.ToString(), out int firstItem_1) &&
                int.TryParse(__thisPair.secondItem.ToString(), out int secondItem_1) &&
                int.TryParse(__anotherPair.firstItem.ToString(), out int firstItem_2) &&
                int.TryParse(__anotherPair.secondItem.ToString(), out int secondItem_2))
            {
                return Mathf.Sqrt(Mathf.Pow(secondItem_1 - firstItem_1, 2) + Mathf.Pow(secondItem_2 - secondItem_1, 2));
            }
            else
            {
                Debug.LogError("Not a number pair");

                return 0.0f;
            }
        }
    }
}