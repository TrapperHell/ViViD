namespace ViViD
{
    /*
     * Not much point in looking here. These are dirt simple
     * extension methods used mostly to avoid temporary
     * variable declaration.
     * 
     * Given that these are extremely fragile, these shouldn't
     * be used in production code without some checks and
     * error-handling.
    */
    public static class StringExtensions
    {
        public static string SubstringAfter(this string str, string value)
        {
            return str.Substring(str.IndexOf(value) + value.Length);
        }

        public static string SubstringAt(this string str, string value)
        {
            return str.Substring(str.IndexOf(value));
        }

        public static string SubstringBefore(this string str, string value)
        {
            return str.Substring(0, str.IndexOf(value));
        }
    }
}