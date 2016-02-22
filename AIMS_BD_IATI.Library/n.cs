using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library
{
    public static class N
    {
        public static T nl<T>(this T input)
        {
            return input == null ? Activator.CreateInstance<T>() : input;
        }
        public static T n<T>(this T input)
        {
            if (input == null)
                input = Activator.CreateInstance<T>();
            return input;
        }

    }
}
