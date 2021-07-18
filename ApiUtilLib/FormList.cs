using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiUtilLib
{
    public class FormList : List<KeyValuePair<string, FormField>>
    {
        public FormList()
        {
        }

        public void Add(string key, string[] value)
        {
            var newValue = new FormField(value);
            this.Add(key, newValue);
        }

        public void Add(string key, string value)
        {
            var newValue = new FormField(value);
            this.Add(key, newValue);
        }

        private void Add(string key, FormField value)
        {
            var formField = this.FirstOrDefault(x => x.Key == key);

            if (formField.Key == key)
            {
                formField.Value.Add(value.RawValue);
            }
            else
            {
                KeyValuePair<string, FormField> item = new KeyValuePair<string, FormField>(key, value);

                value.Key = key;

                this.Add(item);
            }
        }

        // baseString
        public List<KeyValuePair<string, string>> GetFormList()
        {
            var list = new List<KeyValuePair<string, string>>();

            foreach (var item in this)
            {
                var newItem = new KeyValuePair<string, string>(item.Key, item.Value.Value);

                list.Add(newItem);
            }

            return list;
        }

        public string ToFormData()
        {
            string delimiter = "&";

            var list = new List<string>();

            string format = "{0}={1}";

            foreach (var item in this)
            {
                list.Add(string.Format(format, System.Net.WebUtility.UrlEncode(item.Key), item.Value.FormValue));
            }

            return String.Join(delimiter, list.ToArray());
        }
        public string ToQueryString()
        {
            return "?" + ToFormData();
        }

        public static FormList Convert(ApiList apiList)
        {
            var formList = new FormList();

            foreach (var item in apiList)
            {
                formList.Add(item.Key, item.Value);
            }

            return formList;
        }
    }

    public class FormField
    {
        string _key = null;

        string _value = null;
        string[] _arrayValue = null;

        internal string Key
        {
            set
            {
                this._key = value;
            }
        }

        internal void Add(string[] newValue)
        {
            var tempList = new List<string>();

            if (_arrayValue != null)
            {
                foreach (var item in _arrayValue)
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
            this._value = value;
        }

        internal FormField(string[] value)
        {
            this._arrayValue = value;
        }

        internal string[] RawValue
        {
            get
            {
                if (_value != null) return new string[] { _value };

                return _arrayValue;
            }
        }

        public string FormValue
        {
            get
            {
                string delimiter = "&";
                string value = "";

                if (_arrayValue == null)
                {
                    value = System.Net.WebUtility.UrlEncode(this._value);
                }
                else
                {
                    int index = 0;

                    foreach (string item in this._arrayValue)
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
                    value = this._value;
                }
                else
                {
                    int index = 0;

                    foreach (string item in this._arrayValue)
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
