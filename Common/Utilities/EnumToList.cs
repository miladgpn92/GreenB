using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common.Utilities
{
    public class EnumModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public static class EnumUtil
    {
        /// <summary>
        /// usage = default(enumName).ToEnumModel();
        /// </summary>
        public static List<EnumModel> ToEnumModel(this Enum value,bool showname=true)
        {
            var result = new List<EnumModel>();
            foreach (var item in Enum.GetValues(value.GetType()))
            {
                var FindedEnum = (Enum)item;
                if (showname)
                {
                    result.Add(
                   new EnumModel
                   {

                       Id = Convert.ToInt32(item),
                       Title = FindedEnum.GetDisplayName() ?? item.ToString()
                   });
                }
                else
                {
                    result.Add(
                new EnumModel
                {

                    Id = Convert.ToInt32(item),
                    Title = item.ToString()
                });
                }
               
            }
            return result;
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }


        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
           where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                if (enumValue.GetType()
                                            .GetMember(enumValue.ToString())
                                            .First()
                                            .GetCustomAttribute<DisplayAttribute>() != null)
                    return enumValue.GetType()
                                                .GetMember(enumValue.ToString())
                                                .First()
                                                .GetCustomAttribute<DisplayAttribute>()
                                                .GetName();
                else return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
