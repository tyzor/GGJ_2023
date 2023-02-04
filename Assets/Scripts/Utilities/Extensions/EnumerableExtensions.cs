using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetRandomItem<T>(this List<T> collection)
        {
            return collection[Random.Range(0, collection.Count)];
        }
        
        public static T GetRandomItem<T>(this T[] collection)
        {
            return collection[Random.Range(0, collection.Length)];
        }
        
        public static T GetRandomItem<T>(this IEnumerable<T> collection)
        {
            var array = collection.ToArray();
            return array[Random.Range(0, array.Length)];
        }
    }
}