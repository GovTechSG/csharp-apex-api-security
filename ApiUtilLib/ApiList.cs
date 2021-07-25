using System;
using System.Collections.Generic;
using System.Linq;
//using System.Collections;

namespace ApiUtilLib
{
	internal class ApiList : List<KeyValuePair<string, string>>
	{

		public void Add(string key, string value)
		{
            KeyValuePair<string, string> item = new KeyValuePair<string, string>(key, value);

			Add(item);
		}

        // for BaseString
		public string ToString(string delimiter = "&", bool sort = true, bool quote = false)
		{
            List<string> list = new List<string>();

			string format = "{0}={1}";
			if (quote)
            {
                format = "{0}=\"{1}\"";
            }

            if (sort)
            {
                // sort by key, than by value
                IOrderedEnumerable<KeyValuePair<string, string>> sortedList = this.OrderBy(k => k.Key,StringComparer.Ordinal).ThenBy(v => v.Value,StringComparer.Ordinal); //Fixed issue to sort by capital letter.

				foreach (KeyValuePair<string, string> item in sortedList)
				{
                    format = "{0}={1}";
                    if (quote)
                    {
                        format = "{0}=\"{1}\"";
                    }

                    if (item.Value.IsNullOrEmpty() && !quote)
                    {
                        list.Add(string.Format("{0}", item.Key, item.Value));
                    }
                    else
                    {
                        list.Add(string.Format(format, item.Key, item.Value));
                    }
				}
			}
            else
            {
                 foreach (KeyValuePair<string, string> item in this)
                {
                    list.Add(string.Format(format, item.Key, item.Value));
                }
            }

            return string.Join(delimiter, list.ToArray());
		}
	}
}
