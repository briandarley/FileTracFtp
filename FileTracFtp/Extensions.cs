using System;

namespace FileTracFtp
{
    public static class Extensions
    {

        public static string AppSetting(this string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static Int32 ToInt(this string value)
        {
            return Int32.Parse(value);
        }
    }
}
