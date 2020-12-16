using System;

namespace HYDB.Services.Common.Services
{
    public class PropertyTypeResolver
    {
        public static Type Resolve(string propType)
        {
            Type type;
            switch (propType)
            {
                case "text":
                    type = typeof(string);
                    break;

                case "number":
                    type = typeof(double);
                    break;

                default:
                    type = typeof(string);
                    break;
            }

            return type;
        }
    }
}