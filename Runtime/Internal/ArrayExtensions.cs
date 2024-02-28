using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public static class ArrayExtensions
    {
        public static T Last<T>(this T[] array)
        {
            return array[array.Length - 1];
        }

        public static ref T LastRef<T>(this T[] array)
        {
            return ref array[array.Length - 1];
        }
    }
}
