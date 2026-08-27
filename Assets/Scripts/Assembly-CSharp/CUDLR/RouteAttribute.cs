using System;
using System.Text.RegularExpressions;

namespace CUDLR
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RouteAttribute : Attribute
	{
		public delegate void Callback(RequestContext context);

		public Regex m_route;

		public Regex m_methods;

		public bool m_runOnMainThread;

		public Callback m_callback;

		public RouteAttribute(string route, string methods = "(GET|HEAD)", bool runOnMainThread = true)
		{
			m_route = new Regex(route, RegexOptions.IgnoreCase);
			m_methods = new Regex(methods);
			m_runOnMainThread = runOnMainThread;
		}
	}
}
