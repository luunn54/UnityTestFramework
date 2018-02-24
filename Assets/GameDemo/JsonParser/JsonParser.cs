using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AppLogEditor 
{
	public class JsonParser
	{

	    static public List<string> GetListString(IDictionary<string, object> dict, string key, List<string> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            List<object> list;
	            try
	            {
	                list = (List<object>)dict[key];
	            }
	            catch
	            {

	                if (def == null)
	                    return new List<string>();
	                else
	                    return def;
	            }
	            var listString = new List<string>();
	            // check null data
	            if (list != null)
	            {
	                foreach (object obj in list)
	                {

	                    if (obj != null)
	                    {
	                        listString.Add(obj.ToString());
	                    }
	                    else
	                    {
	                        Debug.Log("GetListString is null item, key: " + key);
	                    }
	                }
	            }
	            else
	            {
	                Debug.Log("GetListString is null list, key: " + key);
	            }

	            return listString;
	        }
	        else
	        {
	            if (def == null)
	                return new List<string>();
	            else
	                return def;
	        }
	    }

	    static public List<T> GetListEnum<T>(IDictionary<string, object> dict, string key, List<T> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            List<object> list = null;
	            try
	            {                
	                list = (List<object>)dict[key];
	            }
	            catch (Exception e)
	            {
					Debug.LogError(e.ToString());
	                if (def == null)
	                    return new List<T>();
	                else
	                    return def;
	            }
	            var listEnum = new List<T>();
	            if (list != null)
	            {
	                foreach (object obj in list)
	                {
	                    try
	                    {
	                        if (obj == null || string.IsNullOrEmpty(obj.ToString()))
	                        {
	                            continue;
	                        }
	                        T item = (T)System.Enum.Parse(typeof(T), obj.ToString(), true);
	                        listEnum.Add(item);
	                    }
						catch (Exception e){
							Debug.LogError(e.ToString());
						}
	                }
	            }

	            return listEnum;
	        }
	        else
	        {
	            if (def == null)
	                return new List<T>();
	            else
	                return def;
	        }
	    }

	    static public Dictionary<string, object> GetDict(IDictionary<string, object> dict, string key, Dictionary<string, object> def = null)
	    {
	        if (dict.ContainsKey(key) && dict[key] is IDictionary)
	        {
	            return (Dictionary<string, object>)dict[key];
	        }
	        else
	        {
	            if (def == null)
	                return new Dictionary<string, object>();
	            else
	                return def;
	        }
	    }

	    static public Dictionary<string, Dictionary<string, int>> GetDictDict(IDictionary<string, object> dict)
	    {
	        var res = new Dictionary<string, Dictionary<string, int>>();
	        foreach (KeyValuePair<string, object> ent in dict)
	        {
	            var levelDict = new Dictionary<string, int>();
	            foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)ent.Value)
	            {
	                levelDict.Add(entry.Key, int.Parse(entry.Value.ToString()));
	            }
	            res.Add(ent.Key, levelDict);
	        }
	        return res;
	    }

	    static public string GetString(IDictionary<string, object> dict, string key, string defaultValue = "")
	    {
	        object value;
	        if (dict.TryGetValue(key, out value) && value != null)
	        {
	            return value.ToString();
	        }
	        else
	        {
	            return defaultValue;
	        }
	    }

	    static public bool GetBool(IDictionary<string, object> dict, string key, bool def = false)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return (bool)dict[key];
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public byte GetByte(IDictionary<string, object> dict, string key, byte def = 0)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return byte.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public ushort GetUshort(IDictionary<string, object> dict, string key, ushort def = 0)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return ushort.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public int GetInt(IDictionary<string, object> dict, string key, int def = 0)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return int.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public long GetLong(IDictionary<string, object> dict, string key, long def = 0)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return long.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public float GetFloat(IDictionary<string, object> dict, string key, float def = 0.0f)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return float.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public double GetDouble(IDictionary<string, object> dict, string key, double def = 0.0f)
	    {
	        if (dict.ContainsKey(key))
	        {
	            return double.Parse(dict[key].ToString());
	        }
	        else
	        {
	            return def;
	        }
	    }

	    static public List<int> GetListInt(IDictionary<string, object> dict, string key, List<int> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            var list = (List<object>)dict[key];
	            var listInt = new List<int>();
	            foreach (object obj in list)
	            {
	                listInt.Add(int.Parse(obj.ToString()));
	            }
	            return listInt;
	        }
	        else
	        {
	            if (def == null)
	                return new List<int>();
	            else
	                return def;
	        }
	    }

	    static public List<long> GetListLong(IDictionary<string, object> dict, string key, List<long> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            var list = (List<object>)dict[key];
	            var listlong = new List<long>();
	            foreach (object obj in list)
	            {
	                listlong.Add(long.Parse(obj.ToString()));
	            }
	            return listlong;
	        }
	        else
	        {
	            if (def == null)
	                return new List<long>();
	            else
	                return def;
	        }
	    }

	    static public List<float> GetListFloat(IDictionary<string, object> dict, string key, List<float> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            var list = (List<object>)dict[key];
	            var listfloat = new List<float>();
	            foreach (object obj in list)
	            {
	                listfloat.Add(float.Parse(obj.ToString()));
	            }
	            return listfloat;
	        }
	        else
	        {
	            if (def == null)
	                return new List<float>();
	            else
	                return def;
	        }
	    }

	    static public List<object> GetListDict(IDictionary<string, object> dict, string key, List<object> def = null)
	    {
	        if (dict.ContainsKey(key))
	        {
	            var list = (List<object>)dict[key];
	            var listDict = new List<object>();
	            foreach (object obj in list)
	            {
	                listDict.Add(obj);
	            }
	            return listDict;
	        }
	        else
	        {
	            if (def == null)
	                return new List<object>();
	            else
	                return def;
	        }
	    }

	    public static T GetEnum<T>(IDictionary<string, object> dict, string key, T def)
	    {
	        string value = GetString(dict, key, def.ToString());
	        return ParseEnum<T>(value);
	    }
			
	    static public bool IsValidInTime(long startDate, long endDate, long date)
	    {
	        if (date < startDate) return false;
	        if (date > endDate) return false;
	        return true;
	    }

	    static public Dictionary<long, int> GetLongDict(Dictionary<string, object> dict, string key)
	    {

	        dict = GetDict(dict, key);

	        Dictionary<long, int> result = new Dictionary<long, int>();

	        if (dict != null)
	        {
	            foreach (KeyValuePair<string, object> keyVal in dict)
	            {
	                result.Add(long.Parse(keyVal.Key), int.Parse(keyVal.Value.ToString()));
	            }

	        }
	        return result;
	    }

	    public static T ParseEnum<T>(string value)
	    {
	        try
	        {
	            if (string.IsNullOrEmpty(value))
	            {
	                return default(T);
	            }
	            return (T)System.Enum.Parse(typeof(T), value, true);
	        }
	        catch
	        {
	            return default(T);
	        }
	    }

	    public static T ParseEnum<T>(string value, T def)
	    {
	        try
	        {
	            if (string.IsNullOrEmpty(value))
	            {
	                return def;
	            }
	            return (T)System.Enum.Parse(typeof(T), value, true);
	        }
			catch
	        {
	            return def;
	        }
	    }

	    public static T ParseEnumDefault<T>(string value)
	    {
	        try
	        {
	            if (string.IsNullOrEmpty(value))
	            {
	                return default(T);
	            }
	            return (T)System.Enum.Parse(typeof(T), value, true);
	        }
	        catch
	        {
	            return default(T);
	        }
	    }

	    static private void AddTime(Dictionary<string, object> dict)
	    {
	        var str = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
	        dict.Add("time", str);
	    }

	    /**
	     * Chuyen time seconds thanh dang "hh:mm.ss"
	     **/
	    static public string SecondsToSting(float time)
	    {
	        System.TimeSpan t = System.TimeSpan.FromSeconds(time);
	        string timerFormatted = string.Format("{0:D2}:{1:D2}.{2:D2}", t.Hours, t.Minutes, t.Seconds);
	        return timerFormatted;
	    }

	    /**
	     * Chuyen time seconds thanh dang "hh:mm.ss"
	     **/
	    static public string SecondsGameToString(float time)
	    {
	        System.TimeSpan t = System.TimeSpan.FromSeconds(time);
	        string timerFormatted = "";
	        string format = "{0:D2}:{1:D2}.{2:D2}";
	        if (time < 0)
	        {
	            return "--:--.--";
	        }
	        if (t.Hours > 0)
	        {
	            timerFormatted = string.Format(format, 59, 59, 99);
	        }
	        else
	        {
	            timerFormatted = string.Format(format, t.Minutes, t.Seconds, t.Milliseconds / 10);
	        }
	        return timerFormatted;
	    }

	    static public string ConvertToUTF8String(string str)
	    {
	        var utf8 = new UTF8Encoding();
	        var encodedBytes = utf8.GetBytes(str);
	        var decodedString = utf8.GetString(encodedBytes);
	        return decodedString;
	    }

	    //-------------------Ticks --- Timstamp -----------------------

	    /**Date hien tai tren local. javaTimestamp: milis */
	    public static DateTime DateTimeFromJavaTimestamp(long javaTimestamp)
	    {
	        long tick1970 = (new DateTime(1970, 1, 1)).Ticks;
	        long timeStampInTicks = javaTimestamp * TimeSpan.TicksPerMillisecond;
	        var date = new DateTime(tick1970 + timeStampInTicks).ToLocalTime();
	        return date;
	    }

	    /**Tra ve timestamp (milis), date: date hien tai tren local */
	    public static long JavaTimestampFromDateTime(DateTime date)
	    {
	        long timestampInTicks = date.Ticks - (new DateTime(1970, 1, 1)).ToLocalTime().Ticks;
	        return (long)(timestampInTicks / TimeSpan.TicksPerMillisecond);
	    }

	    //---------------------------------------------------------------
	    static byte[] GetBytes(string str)
	    {
	        byte[] bytes = new byte[str.Length * sizeof(char)];
	        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
	        return bytes;
	    }

	    static string GetString(byte[] bytes)
	    {
	        char[] chars = new char[bytes.Length / sizeof(char)];
	        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
	        return new string(chars);
	    }

	    public static string Base10to32(int number)
	    {
	        string hexnumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
	        string hex = "";
	        int remainder;
	        do
	        {
	            remainder = number % 32;
	            number = number / 32;
	            hex = hexnumbers[remainder] + hex;
	        }
	        while (number > 0);
	        return hex;
	    }

	    public static string ExtractNumber(string s)
	    {
	        var result = string.Empty;
	        for (int i = 0; i < s.Length; i++)
	        {
	            if (Char.IsDigit(s[i]))
	                result += s[i];
	        }
	        return result;
	    }

	    public static string UnescapeJsonString(string value)
	    {
	        const char BACK_SLASH = '\\';
	        const char SLASH = '/';
	        const char DBL_QUOTE = '"';

	        var output = new StringBuilder(value.Length);
	        for (int i = 0; i < value.Length; i++)
	        {
	            char c = value[i];
	            if (c == BACK_SLASH)
	            {

	                var d = value[i + 1];
	                switch (d)
	                {
	                    case SLASH:
	                        output.AppendFormat("{0}", SLASH);
	                        i++;
	                        break;
	                    case BACK_SLASH:
	                        output.AppendFormat("{0}", BACK_SLASH);
	                        i++;
	                        break;
	                    case DBL_QUOTE:
	                        output.AppendFormat("{0}", DBL_QUOTE);
	                        i++;
	                        break;
	                    default:
	                        break;
	                }
	            }
	            else
	            {
	                output.Append(c);
	            }
	        }

	        return output.ToString();
	    }

	    public static string EscapeJsonString(string value)
	    {
	        const char BACK_SLASH = '\\';
	        const char SLASH = '/';
	        const char DBL_QUOTE = '"';

	        var output = new StringBuilder(value.Length);
	        foreach (char c in value)
	        {
	            switch (c)
	            {
	                case SLASH:
	                    output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
	                    break;

	                case BACK_SLASH:
	                    output.AppendFormat("{0}{0}", BACK_SLASH);
	                    break;

	                case DBL_QUOTE:
	                    output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
	                    break;

	                default:
	                    output.Append(c);
	                    break;
	            }
	        }

	        return output.ToString();
	    }
	}
}
