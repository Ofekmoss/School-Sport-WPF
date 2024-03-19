using System;
using System.Collections;
using System.Web;
using SportSite.Common;
using System.Collections.Generic;
using System.Linq;

namespace SportSite.Core
{
	public class TopBannerManager
	{
		public enum TopBannerType
		{
			Basketball = 0,
			Volleyball = 1,
			BeachVolleyball = 2,
			Handball = 3,
			ZooZoo
		}

		public struct Data
		{
			public static Data Empty;
			public string FirstLine;
			public string SecondLine;
			public string ThirdLine;
			public int BannerType;
			static Data()
			{
				Empty = new Data();
				Empty.FirstLine = null;
				Empty.SecondLine = null;
				Empty.ThirdLine = null;
				Empty.BannerType = -1;
			}
		}

		public static readonly string cacheKey = "top_banners";
		public static readonly string iniSection = "top_banner_data";

		public static Data[] GetAllBanners()
		{
			if (HttpContext.Current.Cache[cacheKey] == null)
				RebuildBannersList();
			object oBanners = HttpContext.Current.Cache[cacheKey];
			if (oBanners is ArrayList)
				return (Data[])(oBanners as ArrayList).ToArray(typeof(Data));
			return (Data[])oBanners;
		}

		public static void RebuildBannersList()
		{
			HttpContext context = HttpContext.Current;
			HttpServerUtility server = context.Server;
			int count = Tools.CIntDef(Tools.ReadIniValue(iniSection, "banners_count", server), 0);
			Data[] arrBanners = new Data[count];
			for (int i = 0; i < count; i++)
			{
				Data data = new Data();
				string key = "banner_" + (i + 1) + "_";
				data.FirstLine = Tools.ReadIniValue(iniSection, key + "fl", server);
				data.SecondLine = Tools.ReadIniValue(iniSection, key + "sl", server);
				data.ThirdLine = Tools.ReadIniValue(iniSection, key + "tl", server);
				data.BannerType = Tools.CIntDef(Tools.ReadIniValue(iniSection, key + "bt", server), 0);
				arrBanners[i] = data;
			}
			context.Cache[cacheKey] = arrBanners;
		}

		public static void AddBanner(string firstLine, string secondLine, string thirdLine,
			int bannerType)
		{
			List<Data> arrBanners = GetAllBanners().ToList();
			Data data = new Data();
			data.FirstLine = firstLine;
			data.SecondLine = secondLine;
			data.ThirdLine = thirdLine;
			data.BannerType = bannerType;
			arrBanners.Add(data);
			HttpContext.Current.Cache[cacheKey] = arrBanners.ToArray();
			WriteToIni();
		}

		public static void EditBanner(int index, string firstLine, string secondLine,
			string thirdLine, int bannerType)
		{
			Data[] arrBanners = GetAllBanners();
			if (index < 0 || index >= arrBanners.Length)
			{
				System.Diagnostics.Debug.WriteLine("TopBannerManager.EditBanner: index out of bounds " +
					"(" + index + "), banners count: " + arrBanners.Length);
				return;
			}
			Data data = arrBanners[index];
			data.FirstLine = firstLine;
			data.SecondLine = secondLine;
			data.ThirdLine = thirdLine;
			data.BannerType = bannerType;
			arrBanners[index] = data;
			HttpContext.Current.Cache[cacheKey] = arrBanners;
			WriteToIni();
		}

		public static bool DeleteBanner(int index)
		{
			List<Data> arrBanners = GetAllBanners().ToList();
			if (index < 0 || index >= arrBanners.Count)
			{
				System.Diagnostics.Debug.WriteLine("TopBannerManager.DeleteBanner: index out of bounds " +
					"(" + index + "), banners count: " + arrBanners.Count);
				return false;
			}

			if (arrBanners.Count <= 1)
			{
				System.Diagnostics.Debug.WriteLine("TopBannerManager.DeleteBanner: trying to delete when " +
					"there is one or less banners (" + index + ")");
				return false;
			}

			arrBanners.RemoveAt(index);
			HttpContext.Current.Cache[cacheKey] = arrBanners.ToArray();
			WriteToIni();

			return true;
		}

		private static void WriteToIni()
		{
			Data[] arrBanners = GetAllBanners();
			HttpServerUtility server = HttpContext.Current.Server;
			Tools.WriteIniValue(iniSection, "banners_count", arrBanners.Length.ToString(), server);
			for (int i = 0; i < arrBanners.Length; i++)
			{
				string key = "banner_" + (i + 1) + "_";
				Data data = arrBanners[i];
				Tools.WriteIniValue(iniSection, key + "fl", data.FirstLine, server);
				Tools.WriteIniValue(iniSection, key + "sl", data.SecondLine, server);
				Tools.WriteIniValue(iniSection, key + "tl", data.ThirdLine, server);
				Tools.WriteIniValue(iniSection, key + "bt", data.BannerType.ToString(), server);
			}
		}
	}
}
