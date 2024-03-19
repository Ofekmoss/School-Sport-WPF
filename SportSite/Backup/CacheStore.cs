using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections;

namespace SportSite
{
	public class CacheStore
	{
		#region construction
		private static CacheStore instance;
		private CacheStore()
		{
		}

		static CacheStore()
		{
			instance = new CacheStore();
		}

		public static CacheStore Instance { get { return instance; } }
		#endregion

		private Dictionary<string, DataWrapper> cache = new Dictionary<string, DataWrapper>();
		private int _timeToCleanup = 100;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Cache Store items count: ").Append(cache.Count).Append("<br />");
			cache.Keys.ToList().ForEach(key =>
			{
				DataWrapper wrapper = cache[key];
				int minutes = (int)((DateTime.Now -  wrapper.LastUpdated).TotalMinutes);
				object data = wrapper.Data;
				sb.Append(key).Append(" was last accessed ").Append(minutes).Append(" minutes ago, has expire time of ").Append(wrapper.ExpireTimeMinutes).Append(" and contains ");
				if (data == null)
				{
					sb.Append("no data");
				}
				else
				{
					Type type = data.GetType();
					if (type.IsPrimitive)
					{
						sb.Append("primitive value: ").Append(data);
					}
					else if (data is string)
					{
						sb.Append("string with ").Append((data as string).Length).Append(" characters");
					}
					else if (data is IList)
					{
						sb.Append("list with ").Append((data as IList).Count).Append(" items");
					}
					else
					{
						sb.Append("unknown type: ").Append(data);
					}
				}
				sb.Append("<hr />");
			});
			return sb.ToString();
		}

		public void Update(string key, object data, int expireTimeMinutes)
		{
			lock (this)
			{
				if (!cache.ContainsKey(key))
					cache.Add(key, new DataWrapper());
				DataWrapper wrapper = cache[key];
				wrapper.Data = data;
				wrapper.ExpireTimeMinutes = expireTimeMinutes;
				wrapper.LastUpdated = DateTime.Now;
				cache[key] = wrapper;
			}
		}

		public bool Get(string key, out object data)
		{
			data = null;

			lock (this)
			{
				_timeToCleanup--;
				if (_timeToCleanup <= 0)
				{
					cache.Keys.ToList().FindAll(k => cache[k].Expired).ForEach(k =>
					{
						this.Remove(k);
					});
					_timeToCleanup = 100;
				}
			}

			if (cache.ContainsKey(key))
			{
				DataWrapper wrapper = cache[key];
				if (!wrapper.Expired)
				{
					data = wrapper.Data;
					return true;
				}
			}
			return false;
		}

		public int RemoveAll()
		{
			int count = 0;
			lock (this)
			{
				count = cache.Count;
				cache.Clear();
			}

			return count;
		}

		public bool Remove(string key)
		{
			bool success = false;
			if (cache.ContainsKey(key))
			{
				lock (this)
				{
					cache.Remove(key);
					success = true;
				}
			}

			return success;
		}

		public bool Remove(string[] keys)
		{
			bool success = true;
			foreach (string key in keys)
				success = success && this.Remove(key);
			return success;
		}

		public string[] GetAllKeys()
		{
			return cache.Keys.ToList().ToArray();
		}

		protected class DataWrapper
		{
			public object Data { get; set; }
			public int ExpireTimeMinutes { get; set; }
			public DateTime LastUpdated { get; set; }

			public DataWrapper()
			{
				this.Data = null;
				this.ExpireTimeMinutes = 0;
				this.LastUpdated = DateTime.MinValue;
			}

			public bool Expired
			{
				get
				{
					if (this.ExpireTimeMinutes > 0 && this.LastUpdated.Year > 1900)
						return ((int)((DateTime.Now - this.LastUpdated).TotalMinutes) > this.ExpireTimeMinutes);
					return true;
				}
			}
		}
	}
}