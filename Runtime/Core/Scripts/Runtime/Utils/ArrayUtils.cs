using System;
using System.Collections.Generic;

namespace NIX.Core.Utils
{
    public static class ArrayUtils
    {
        public static T[] Shuffle<T>(T[] array)
        {
            System.Random rng = new System.Random();
            T[] newArray = new T[array.Length];
            Array.Copy(array, newArray, array.Length);

            int n = newArray.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (newArray[k], newArray[n]) = (newArray[n], newArray[k]);
            }

            return newArray;
        }

        public static int IndexOf<T>(T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item)) return i;
            }

            return -1;
        }

        public static bool IsValidIndex<T>(T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        public static T GetPrevious<T>(T[] array, ref int currentIndex)
        {
            if (currentIndex - 1 < 0)
            {
                currentIndex = array.Length - 1;
            }
            else
            {
                currentIndex = currentIndex - 1;
            }

            return array[currentIndex];
        }

        public static T GetNext<T>(T[] array, ref int currentIndex)
        {
            if (currentIndex + 1 >= array.Length)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex = currentIndex + 1;
            }

            return array[currentIndex];
        }


        public static double GetMax(double value, double[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > value)
                {
                    return values[i];
                }
            }

            return values[^1];
        }

        public static double GetMin(double value, double[] values)
        {
            for (int i = values.Length - 1; i >= 0; i--)
            {
                if (values[i] <= value) return values[i];
            }

            return values[0];
        }

        public static T[] AppendToArray<T>(T[] array, T newElement)
        {
            if (array == null)
            {
                return new T[] { newElement };
            }

            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = newElement;
            return newArray;
        }

        /// <summary>
        /// Append a new element to the end of an array. This will resize the array.
        /// </summary>
        public static void Append<T>(ref T[] array, T item)
        {
            if (array == null)
            {
                array = new T[1];
                array[0] = item;
                return;
            }

            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;
        }

        /// <summary>
        /// Remove element at given index. This will resize the array.
        /// </summary>
        public static void RemoveAt<T>(ref T[] array, int index)
        {
            if (array == null || index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            for (int i = index; i < array.Length - 1; i++)
            {
                array[i] = array[i + 1];
            }

            Array.Resize(ref array, array.Length - 1);
        }

        /// <summary>
        /// Remove first occurrence of the given item. This will resize the array.
        /// </summary>
        public static bool Remove<T>(ref T[] array, T item)
        {
            if (array == null)
                return false;

            int index = Array.IndexOf(array, item);
            if (index < 0)
                return false;

            RemoveAt(ref array, index);
            return true;
        }


        public static bool IsPrefixOf<T>(List<T> prefix, List<T> full)
        {
            if (prefix.Count > full.Count)
                return false;

            for (int i = 0; i < prefix.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(prefix[i], full[i]))
                    return false;
            }

            return true;
        }

        public static bool AreListsEqual<T>(List<T> listA, List<T> listB)
        {
            if (listA.Count != listB.Count)
                return false;

            for (int i = 0; i < listA.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(listA[i], listB[i]))
                    return false;
            }

            return true;
        }
    }
}