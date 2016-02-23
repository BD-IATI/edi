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
            return input == null ? Activator.CreateInstance<T>() : input;
        }
        /// <summary>
        /// Return new instance if null, with modifing source instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T n<T>(this T input)
        {
            if (input == null)
                input = Activator.CreateInstance<T>();
            return input;
        }

    }
}
