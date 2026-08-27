using System;
using System.Reflection;

internal struct PropertyMetadata
{
	public MemberInfo Info;

	public bool IsField;

	public Type Type;
}
