using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportSite.Core
{
	public static class CookieManager
	{
		public static string Read(string name)
		{
			if (HttpContext.Current.Request.Cookies[name] != null)
				return (HttpContext.Current.Request.Cookies[name].Value + "");
			return string.Empty;
		}

		public static void AddOrUpdate(string name, string value, int days)
		{
			CookieManager.Remove(name);
			HttpCookie cookie = new HttpCookie(name);
			cookie.Value = value;
			cookie.Expires = DateTime.Now.AddDays(days);
			HttpContext.Current.Response.Cookies.Add(cookie);
		}

		public static void Remove(string name)
		{
			HttpCookie cookie = HttpContext.Current.Response.Cookies[name];
			if (cookie != null)
			{
				cookie.Value = "";
				cookie.Expires = DateTime.Now.AddDays(-10);
				HttpContext.Current.Response.Cookies.Set(cookie);
			}
			
			/*
			if (HttpContext.Current.Request.Cookies[name] != null)
			{
				HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddYears(-1);
				HttpContext.Current.Response.Cookies[name].Value = string.Empty;

				HttpContext.Current.Request.Cookies[name].Expires = DateTime.Now.AddYears(-1);
				HttpContext.Current.Request.Cookies[name].Value = string.Empty;
			}

			HttpContext.Current.Response.Cookies.Remove(name);
			HttpContext.Current.Request.Cookies.Remove(name);
			*/
		}
	}
}