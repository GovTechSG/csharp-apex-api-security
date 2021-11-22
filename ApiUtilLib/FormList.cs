using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiUtilLib
{
    public abstract class BaseList : List<KeyValuePair<string, FormField>>
    {
        public void Add(string key, string[] value)
        {
            FormField newValue = new FormField(value);
            Add(key, newValue);
        }

        public void Add(string key, string value)
        {
            FormField newValue = new FormField(value);
            Add(key, newValue);
        }

        private void Add(string key, FormField value)
        {
            KeyValuePair<string, FormField> formField = this.FirstOrDefault(x => x.Key == key);

            if (formField.Key == key)
            {
                formField.Value.Add(value.RawValue);
            }
            else
            {
                KeyValuePair<string, FormField> item = new KeyValuePair<string, FormField>(key, value);

                value.Key = key;

                Add(item);
            }
        }

        public virtual string ToString(bool urlSafeString = true)
        {
            string delimiter = "&";

            List<string> list = new List<string>();

            string format = "{0}={1}";

            foreach (KeyValuePair<string, FormField> item in this)
            {
                if (urlSafeString)
                {
                    list.Add(string.Format(format, System.Net.WebUtility.UrlEncode(item.Key), item.Value.FormValue));
                }
                else
                {
                    list.Add(string.Format(format, item.Key, item.Value.Value));
                }
            }

            return string.Join(delimiter, list.ToArray());
        }

        internal static T SetupList<T>(Dictionary<object, object> data = null) where T : BaseList, new()
        {
            try
            {
                T dataList = new T();

                if (data != null)
                {
                    foreach (KeyValuePair<object, object> item in data)
                    {
                        object key = item.Key ?? "";
                        object value = item.Value ?? "";

                        string value_s = value.ToString().Trim();

                        if (!key.ToString().IsNullOrEmpty())
                        {
                            string[] _queryParams = { "" };
                            string val = null;

                            if (!value_s.IsNullOrEmpty() && !(value_s.StartsWith("{", StringComparison.InvariantCulture) && value_s.EndsWith("}", StringComparison.InvariantCulture)))
                            {

                                val = value_s.RemoveString(new string[] { "\\", "\\ ", " \\", "\"", "\\  ", "\n" }).Unescape();

                                if (val == "True")
                                {
                                    val = "true";
                                }

                                if (val == "False")
                                {
                                    val = "false";
                                }

                                if (val.StartsWith("[", StringComparison.InvariantCulture) && val.EndsWith("]", StringComparison.InvariantCulture))
                                {

                                    string[] _paramValues = { "" };
                                    val = val.RemoveString(new string[] { "[", "]", " " });
                                    _paramValues = val.Split(',');
                                    foreach (string paramvalue in _paramValues)
                                    {
                                        string _paramvalue = paramvalue;
                                        dataList.Add(key.ToString(), _paramvalue.Unescape());
                                    }

                                }
                                else
                                {
                                    dataList.Add(key.ToString(), val);
                                }
                            }
                            else
                            {
                                dataList.Add(key.ToString(), val);
                            }

                        }
                    }
                }

                return dataList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class FormData : BaseList
    {
        public override string ToString()
        {
            return base.ToString();
        }

        internal List<KeyValuePair<string, string>> GetBaseStringList()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, FormField> item in this)
            {
                KeyValuePair<string, string> newItem = new KeyValuePair<string, string>(item.Key, item.Value.Value);

                list.Add(newItem);
            }

            return list;
        }


        public static FormData SetupList(Dictionary<object, object> data = null)
        {
            return SetupList<FormData>(data);
        }
    }

    public class QueryData : BaseList
    {
        public override string ToString()
        {
            string queryString = base.ToString();

            if (queryString.Length > 0)
            {
                queryString = string.Format("?{0}", queryString);
            }

            return queryString;
        }

        public override string ToString(bool urlSafeString = true)
        {
            string queryString = base.ToString(urlSafeString);

            if (queryString.Length > 0)
            {
                queryString = string.Format("?{0}", queryString);
            }

            return queryString;
        }

        public static QueryData SetupList(Dictionary<object, object> data = null)
        {
            return SetupList<QueryData>(data);
        }
    }

    public class FormField
    {
        private string _key = null;

        private string _value = null;
        private string[] _arrayValue = null;

        internal string Key
        {
            set => _key = value;
        }

        internal void Add(string[] newValue)
        {
            List<string> tempList = new List<string>();

            if (_arrayValue != null)
            {
                foreach (string item in _arrayValue)
                {
                    tempList.Add(item);
                }
            }
            else
            {
                tempList.Add(_value);
                _value = null;
            }
            tempList.AddRange(newValue);

            // You can convert it back to an array if you would like to
            _arrayValue = tempList.ToArray();
        }

        internal FormField(string value)
        {
            _value = value;
        }

        internal FormField(string[] value)
        {
            _arrayValue = value;
        }

        internal string[] RawValue => _value != null ? (new string[] { _value }) : _arrayValue;

        public string FormValue
        {
            get
            {
                string delimiter = "&";
                string value = "";

                if (_arrayValue == null)
                {
                    value = System.Net.WebUtility.UrlEncode(_value);
                }
                else
                {
                    int index = 0;

                    foreach (string item in _arrayValue)
                    {
                        if (index == 0)
                        {
                            value = System.Net.WebUtility.UrlEncode(item);
                        }
                        else
                        {
                            value += string.Format("{0}{1}={2}", delimiter, System.Net.WebUtility.UrlEncode(_key), System.Net.WebUtility.UrlEncode(item));
                        }
                        index++;
                    }
                }

                return value;
            }
        }

        public string Value
        {
            get
            {
                string delimiter = "&";
                string value = "";

                if (_arrayValue == null)
                {
                    value = _value;
                }
                else
                {
                    int index = 0;

                    foreach (string item in _arrayValue)
                    {
                        if (index == 0)
                        {
                            value = item;
                        }
                        else
                        {
                            value += string.Format("{0}{1}={2}", delimiter, _key, item);
                        }
                        index++;
                    }
                }

                return value;
            }
        }
    }
}