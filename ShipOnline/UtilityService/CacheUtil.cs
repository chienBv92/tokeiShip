//------------------------------------------------------------------------
// Version		: 001
// Designer		: ChienBV
// Date			: 2018/01/23
// Comment		: Create new
//------------------------------------------------------------------------

using ShipOnline.Resources;
using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Web;
using ShipOnline.Models;

namespace ShipOnline.UtilityService
{
	/// <summary>
	/// Provide method to save/get cache, avoid the session issue on load balancing Web farm
	/// </summary>
	public static class CacheUtil
	{
		#region Application
		/// <summary>
		/// Short Live Cache is cache that existed during live of a request.
		/// After server response to client, the Short Live Cache will be clear.
		/// </summary>
		/// <param name="key">key string</param>
		/// <returns>object value</returns>
		public static object GetShortLiveCache(string key)
		{
			Dictionary<string, Dictionary<string, object>> cachedDictionary
				= (Dictionary<string, Dictionary<string, object>>)HttpContext.Current.Application["shortLiveCache"];
			if (!cachedDictionary.ContainsKey(HttpContext.Current.Session.SessionID))
			{
				Dictionary<string, object> sessionCachedDictionary = cachedDictionary[HttpContext.Current.Session.SessionID];
				if (sessionCachedDictionary.ContainsKey(key))
				{
					return sessionCachedDictionary[key];
				}
			}
			return null;
		}

		/// <summary>
		/// Short Live Cache is cache that existed during live of a request.
		/// After server response to client, the Short Live Cache will be clear.
		/// </summary>
		/// <param name="key">key string</param>
		/// <param name="value">object value</param>
		public static void SaveShortLiveCache(string key, object value)
		{
			Dictionary<string, Dictionary<string, object>> cachedDictionary
				= (Dictionary<string, Dictionary<string, object>>)HttpContext.Current.Application["shortLiveCache"];
			Dictionary<string, object> sessionCachedDictionary;
			if (cachedDictionary.ContainsKey(HttpContext.Current.Session.SessionID))
			{
				sessionCachedDictionary = cachedDictionary[HttpContext.Current.Session.SessionID];
				if (sessionCachedDictionary.ContainsKey(key))
				{
					sessionCachedDictionary[key] = value;
				}
				else
				{
					sessionCachedDictionary.Add(key, value);
				}
			}
			else
			{
				sessionCachedDictionary = new Dictionary<string, object>();
				sessionCachedDictionary.Add(key, value);
				cachedDictionary.Add(HttpContext.Current.Session.SessionID, sessionCachedDictionary);
			}
		}

		/// <summary>
		/// Gets the CMN entity model.
		/// </summary>
		/// <value>
		/// The CMN entity model.
		/// </value>
		private static CmnEntityModel CmnEntityModel
		{
			get
			{
				if (HttpContext.Current.Session["CmnEntityModel"] == null)
				{
					HttpContext.Current.Session["CmnEntityModel"] = new CmnEntityModel();
				}
				return (CmnEntityModel)HttpContext.Current.Session["CmnEntityModel"];
			}
		}

		/// <summary>
		/// Short Live Cache is cache that existed during live of a request.
		/// After server response to client, the Short Live Cache will be clear.
		/// </summary>
		public static void ClearShortLiveCache()
		{
			Dictionary<string, Dictionary<string, object>> cachedDictionary
				= (Dictionary<string, Dictionary<string, object>>)HttpContext.Current.Application["shortLiveCache"];
			if (cachedDictionary.ContainsKey(HttpContext.Current.Session.SessionID))
			{
				cachedDictionary.Remove(HttpContext.Current.Session.SessionID);
			}
		}
		#endregion

		#region Session
		public static void SaveCache(string key, object value)
		{			
			HttpContext.Current.Session[key] = value;
		}

		public static T GetCache<T>(string key, object defaultValue = null)
		{			
			if (HttpContext.Current.Session[key] == null)
			{
				return (T)defaultValue;
			}

			return (T)HttpContext.Current.Session[key];
		}

		public static void RemoveCache(string key)
		{			
			HttpContext.Current.Session.Remove(key);
		}

		public static void RemoveCache(string tabID, string screenID)
		{
			List<string> sessionKey = new List<string>();
			// Loop all key session
			foreach (string key in HttpContext.Current.Session.Keys)
			{
				// Select key which contains tabID and not contain screen ID and add it to list
				if (key.Contains(tabID) && !key.Contains(screenID))
				{
					sessionKey.Add(key);
				}
			}

			// Remove cache key after add to list
			foreach (string key in sessionKey)
			{
				RemoveCache(key);
			}
		}
		#endregion

		#region Cookie
		//private static void SaveCacheCookie(string key, object value)
		//{
		//	// Add
		//	int cookieIndex = 1;
		//	HttpCookie cookie;
		//	JavaScriptSerializer js = new JavaScriptSerializer();
		//	string json = js.Serialize(value);
		//	string temp;
		//	while (json.Length > 0)
		//	{
		//		int length = json.Length;
		//		if (json.Length >= 100)
		//		{
		//			length = 100;
		//		}
		//		temp = json.Substring(0, length);
		//		cookie = new HttpCookie("LTP" + cookieIndex++.ToString() + key);
		//		cookie.Expires = DateTime.Now.AddMinutes(Convert.ToDouble(SystemSettingUtil.GetTimeoutMinute()));
		//		cookie.Value = temp;
		//		HttpContext.Current.Response.Cookies.Add(cookie);
		//		json = json.Remove(0, length);
		//	}

		//	// Remove
		//	RemoveCacheCookie(key, cookieIndex);
		//}

		//private static T GetCacheCookie<T>(string key, object defaultValue = null)
		//{
		//	int cookieIndex = 1;
		//	HttpCookie cookie = HttpContext.Current.Request.Cookies["LTP" + cookieIndex++.ToString() + key];
		//	string json = string.Empty;
		//	while (cookie != null)
		//	{
		//		json += cookie.Value;
		//		cookie = HttpContext.Current.Request.Cookies["LTP" + cookieIndex++.ToString() + key];
		//	}
		//	if (!string.IsNullOrEmpty(json))
		//	{
		//		JavaScriptSerializer js = new JavaScriptSerializer();
		//		try
		//		{
		//			return js.Deserialize<T>(json);
		//		}
		//		catch
		//		{
		//			return (T)defaultValue;
		//		}
		//	}
		//	return (T)defaultValue;
		//}

		//private static void RemoveCacheCookie(string key, int cookieIndex = 1)
		//{
		//	HttpCookie cookie = HttpContext.Current.Request.Cookies["LTP" + cookieIndex.ToString() + key];
		//	while (cookie != null)
		//	{
		//		cookie.Expires = DateTime.Now.AddDays(-1d);
		//		HttpContext.Current.Response.Cookies.Add(cookie);
		//		HttpContext.Current.Request.Cookies.Remove("LTP" + cookieIndex++.ToString() + key);
		//		cookie = HttpContext.Current.Request.Cookies["LTP" + cookieIndex.ToString() + key];
		//	}
		//}

		//private static void RemoveCacheCookie(string key)
		//{
		//	RemoveCacheCookie(key, 1);
		//}
		#endregion
	}
}