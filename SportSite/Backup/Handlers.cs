using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SportSite
{
	public class SvgHandler : IHttpHandler
	{

		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "image/svg+xml";
			context.Response.BinaryWrite(File.ReadAllBytes(context.Request.PhysicalPath));
			context.Response.End();
		}
	}
}