using System;

namespace Z3.Utils
{
    public static class EnumUtils
    {
        public static T[] CastEnum<T>()
        {
            return Enum.GetValues(typeof(T)) as T[];
        }
    }
}