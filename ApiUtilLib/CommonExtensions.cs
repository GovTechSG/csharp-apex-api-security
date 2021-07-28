using System.Text;

namespace ApiUtilLib
{
    public static class CommonExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value == string.Empty;
        }

        public static string Unescape(this string txt)
        {
            if (string.IsNullOrEmpty(txt)) { return txt; }
            StringBuilder retval = new StringBuilder(txt.Length);
            for (int ix = 0; ix < txt.Length;)
            {
                int jx = txt.IndexOf('\n', ix);
                if (jx < 0 || jx == txt.Length - 1)
                {
                    jx = txt.Length;
                }

                retval.Append(txt, ix, jx - ix);
                if (jx >= txt.Length)
                {
                    break;
                }

                //char str = txt[jx + 1];
                switch (txt[jx + 1])
                {
                    case 'n': retval.Append('\n'); break;  // Line feed
                    case 'r': retval.Append('\r'); break;  // Carriage return
                    case 't': retval.Append('\t'); break;  // Tab
                    case '\\': retval.Append('\\'); break; // Don't escape
                    default:                                 // Unrecognized, copy as-is
                        retval.Append('\\').Append(txt[jx + 1]); break;
                }
                ix = jx + 2;
            }
            return retval.ToString();
        }

        public static string RemoveString(this string value, string[] array)
        {
            foreach (string item in array)
            {
                value = value.Replace(item, "");
            }

            return value;
        }
    }
}
