using System;
namespace Language
{
    public static class StringExtensions
    {
	public static string UnescapeXML(this string s)
	{
	    if (string.IsNullOrEmpty(s))
	    {
		return s;
	    }
	    return s.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&");
	}
    }
}
