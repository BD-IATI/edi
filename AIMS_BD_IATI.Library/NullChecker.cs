using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library
{
    public static class NullChecker
    {
        /// <summary>
        /// Return new instance if null, without modifing source instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T N<T>(this T input)
        {
            if (input == null)
            {
                if (typeof(T).IsArray)
                {
                    Type elementType = typeof(T).GetElementType();
                    Array array = Array.CreateInstance(elementType, 1);
                    return (T)(object)array;
                }
                else
                    return Activator.CreateInstance<T>();
            }
            return input;
        }

        /// <summary>
        /// Create new instance of T if T is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T n<T>(this T input)
        {
            if (input == null)
            {
                Type UnderlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (UnderlyingType == null)
                    input = Activator.CreateInstance<T>();
                else
                    input = (T)Activator.CreateInstance(UnderlyingType);

            }
            return input;
        }
        /// <summary>
        /// Create new instance of T[] if T[] is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[] n<T>(this T[] input)
        {
            if (input == null)
            {
                //Type elementType = typeof(T).GetElementType();
                Array array = Array.CreateInstance(typeof(T), 1);
                input = (T[])array;
            }
            return input;
        }
        /// <summary>
        /// Returns the element of the array, if the array is null then returns a new instance of the element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T n<T>(this T[] input, int position)
        {
            if (input == null)
            {
                return Activator.CreateInstance<T>();
            }
            else if (position < input.Length)
                if (input[position] == null)
                    return Activator.CreateInstance<T>();
                else
                    return input[position];
            else
                return Activator.CreateInstance<T>();
        }

        public static T n<T>(this List<T> input, int position)
        {
            if (input == null)
            {
                return Activator.CreateInstance<T>();
            }
            else if (position < input.Count)
                return input[position];
            else
                return Activator.CreateInstance<T>();
        }

    }
}
