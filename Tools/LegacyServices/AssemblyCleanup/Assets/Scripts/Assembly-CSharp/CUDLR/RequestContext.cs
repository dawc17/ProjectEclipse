using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CUDLR
{
	public class RequestContext
	{
		public HttpListenerContext context;

		public Match match;

		public bool pass;

		public string path;

		public int currentRoute;

		public HttpListenerRequest Request
		{
			get
			{
				return context.Request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return context.Response;
			}
		}

		public RequestContext(HttpListenerContext ctx)
		{
			context = ctx;
			match = null;
			pass = false;
			path = WWW.UnEscapeURL(context.Request.Url.AbsolutePath);
			if (path == "/")
			{
				path = "/index.html";
			}
			currentRoute = 0;
		}
	}
}
