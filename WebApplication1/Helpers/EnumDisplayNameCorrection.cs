using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApplication1.Helpers
{
    public static class EnumDisplayNameCorrection
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? enumValue.ToString();
        }
    }
}
