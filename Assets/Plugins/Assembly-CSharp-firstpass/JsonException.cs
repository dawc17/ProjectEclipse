using System;

public class JsonException : Exception
{
	public JsonException()
	{
	}

	internal JsonException(ParserToken JLFCBDKNAGP)
		: base(string.Format("Invalid token '{0}' in input string", JLFCBDKNAGP))
	{
	}

	internal JsonException(ParserToken JLFCBDKNAGP, Exception IADJLHGKHGL)
		: base(string.Format("Invalid token '{0}' in input string", JLFCBDKNAGP), IADJLHGKHGL)
	{
	}

	internal JsonException(int ILHDJDNPFKH)
		: base(string.Format("Invalid character '{0}' in input string", (char)ILHDJDNPFKH))
	{
	}

	internal JsonException(int ILHDJDNPFKH, Exception IADJLHGKHGL)
		: base(string.Format("Invalid character '{0}' in input string", (char)ILHDJDNPFKH), IADJLHGKHGL)
	{
	}

	public JsonException(string LIOGIBJBHAH)
		: base(LIOGIBJBHAH)
	{
	}

	public JsonException(string LIOGIBJBHAH, Exception IADJLHGKHGL)
		: base(LIOGIBJBHAH, IADJLHGKHGL)
	{
	}
}
