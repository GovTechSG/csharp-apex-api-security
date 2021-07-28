﻿using System;
//using System.Text;
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

        //public static string GetCharp(dynamic value)
        //{
        //    try
        //    {
        //        return value.charp;
        //    }
        //    catch (Exception)
        //    {
        //        return Convert.ToString(value);
        //    }
        //}

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

        //public static string Unescape(this string txt)
        //{
        //    if (string.IsNullOrEmpty(txt)) { return txt; }
        //    StringBuilder retval = new StringBuilder(txt.Length);
        //    for (int ix = 0; ix < txt.Length;)
        //    {
        //        int jx = txt.IndexOf('\\', ix);
        //        if (jx < 0 || jx == txt.Length - 1) jx = txt.Length;
        //        retval.Append(txt, ix, jx - ix);
        //        if (jx >= txt.Length) break;
        //        switch (txt[jx + 1])
        //        {
        //            case 'n': retval.Append('\n'); break;  // Line feed
        //            case 'r': retval.Append('\r'); break;  // Carriage return
        //            case 't': retval.Append('\t'); break;  // Tab
        //            case '\\': retval.Append('\\'); break; // Don't escape
        //            default:                                 // Unrecognized, copy as-is
        //                retval.Append('\\').Append(txt[jx + 1]); break;
        //        }
        //        ix = jx + 2;
        //    }
        //    return retval.ToString();
        //}

        //public static string RemoveString(this string value, string[] array)
        //{
        //    foreach (var item in array)
        //    {
        //        value = value.Replace(item, "");
        //    }

        //    return value;
        //}

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
