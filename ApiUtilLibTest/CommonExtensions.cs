using System;
using System.Collections.Generic;
using System.Linq;

namespace ApexUtilLibTest
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Extension method to return an enum value of type T for the given string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
        {
            return value.IsNullOrEmpty() ? default : (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetCharp(Dictionary<string, string> value)
        {
            try
            {
                string result = value.FirstOrDefault(c => c.Key == "c#").Value;
                if (result == null)
                {
                    result = value.FirstOrDefault(c => c.Key == "default").Value;
                }

                return result;
            }
            catch (Exception)
            {
                return Convert.ToString(value);
            }
        }

        public static ApiUtilLib.SignatureMethod ParseSignatureMethod(this string value, string secret)
        {
            return value == null
                ? secret == null ? ApiUtilLib.SignatureMethod.SHA256withRSA : ApiUtilLib.SignatureMethod.HMACSHA256
                : value.ToEnum<ApiUtilLib.SignatureMethod>();
        }

        public static bool IsNullOrEmpty(this string value){
            return value == null || value == string.Empty;
        }

        public static bool ToBool(this string value)
        {
            if(!value.IsNullOrEmpty()){
                if (value.ToLower() == "true")
                {
                    return true;
                }

                if (value.ToLower() == "false")
                {
                    return false;
                }
            }

            return false;
        }
    }
}
