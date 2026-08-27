using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class Rand
{
	private static short _seed;

	private static List<short> _source;

	private static List<short> FMOOJNFJFNB;

	public static short FHNPGKOLJFF
	{
		get
		{
			return CLCJNCMBDNB();
		}
		set
		{
			set_Seed(value);
		}
	}

	private static short OFNCIJGCBNO
	{
		get
		{
			return CHOMDFOKMPA();
		}
	}

	static Rand()
	{
		_source = new List<short>();
		FMOOJNFJFNB = new List<short>();
		Reset(0);
	}

	public static short CLCJNCMBDNB()
	{
		return _seed;
	}

	public static void set_Seed(short value)
	{
		if (_seed != (ushort)(value % 65535))
		{
			Reset(value);
		}
	}

	public static void Init(int OKGKLCLEDFN)
	{
		if (_seed != (short)(OKGKLCLEDFN % 32767))
		{
			Reset(OKGKLCLEDFN);
		}
	}

	private static void Reset(int OKGKLCLEDFN)
	{
		_seed = (short)(OKGKLCLEDFN % 32767);
		FMOOJNFJFNB.Clear();
		_source.Clear();
		for (short num = 0; num < short.MaxValue; num++)
		{
			_source.Add(num);
		}
		_source.Sort(delegate
		{
			return Random.Range(-1, 1);
		});
		FMOOJNFJFNB.AddRange(_source.GetRange(0, _seed));
		_source.RemoveRange(0, _seed);
	}

	private static void ALIKAKJLCAN()
	{
		FMOOJNFJFNB = Interlocked.Exchange(ref _source, FMOOJNFJFNB);
		AdvLog.Log("after dirty: " + FMOOJNFJFNB.Count + " source: " + _source.Count);
	}

	private static short CHOMDFOKMPA()
	{
		if (_source.Count == 0)
		{
			ALIKAKJLCAN();
		}
		int index = Random.Range(0, _source.Count);
		short num = _source[index];
		_source.RemoveAt(index);
		FMOOJNFJFNB.Add(num);
		return num;
	}

	public static int Range(int IOFHCAAOELD, int IPMPAMAHLJG)
	{
		if (IOFHCAAOELD > IPMPAMAHLJG)
		{
			IPMPAMAHLJG = Interlocked.Exchange(ref IOFHCAAOELD, IPMPAMAHLJG);
		}
		int num = IPMPAMAHLJG - IOFHCAAOELD;
		return (num != 0) ? (IOFHCAAOELD + CHOMDFOKMPA() % num) : 0;
	}
}
