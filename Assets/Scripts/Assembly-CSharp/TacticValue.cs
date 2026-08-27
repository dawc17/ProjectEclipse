using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TacticValue
{
	private enum NJDJLPHNAKG
	{
		Exponential = 0,
		Linear = 1
	}

	private float _base;

	private float FJNMMCGLOJH;

	private float KCPECJLPBAH;

	private float MEOEEBFJHAA;

	private float BFCLFALKDJE;

	private float MPJNDEEMEAC;

	private float GIGOKAGAAHE;

	private float KAMCPBICLNJ;

	private float AIFFLEEOLHG;

	private float PBLKIFMDEHC;

	private float FLLAGMNBGLB;

	private float _limit;

	private float LFCOMPLOFIM;

	private float HAOPIJJPNBD;

	private NJDJLPHNAKG MJKHAOMOOMK = NJDJLPHNAKG.Linear;

	private List<global::Pair<string, TacticValue>> BMPNBKLELPH = new List<global::Pair<string, TacticValue>>();

	private List<global::Pair<InfoAnimation, float>> GKNOGDMPNHC = new List<global::Pair<InfoAnimation, float>>();

	private List<global::Pair<InfoAnimation, float>> MHMDMMGJLEH = new List<global::Pair<InfoAnimation, float>>();

	public TacticValue()
	{
	}

	public TacticValue(XmlNode AFHNINCKJEE)
	{
		Parse(AFHNINCKJEE);
	}

	public TacticValue(TacticValue value)
	{
		CopyFrom(value);
	}

	public void Parse(XmlNode node)
	{
		if (node != null)
		{
			_base = node.Attributes["Base"].ParseFloat();
			FJNMMCGLOJH = node.Attributes["CounterFactor"].ParseFloat();
			KCPECJLPBAH = node.Attributes["DamageFactor"].ParseFloat();
			MEOEEBFJHAA = node.Attributes["HealthFactor"].ParseFloat();
			BFCLFALKDJE = node.Attributes["EnemyHealthFactor"].ParseFloat();
			MPJNDEEMEAC = node.Attributes["AnimationFramesFactor"].ParseFloat();
			PBLKIFMDEHC = node.Attributes["ChildFramesFactor"].ParseFloat();
			GIGOKAGAAHE = node.Attributes["MagicBulletFactor"].ParseFloat();
			KAMCPBICLNJ = node.Attributes["MissileBulletFactor"].ParseFloat();
			AIFFLEEOLHG = node.Attributes["HitFactor"].ParseFloat();
			FLLAGMNBGLB = node.Attributes["DistanceFactor"].ParseFloat();
			HAOPIJJPNBD = node.Attributes["Shift"].ParseFloat();
			_limit = node.Attributes["Limit"].ParseFloat();
			LFCOMPLOFIM = node.Attributes["AntiLimit"].ParseFloat();
			SetFactorType(node.Attributes["FactorType"].CIPOICEEIBK(string.Empty));
			ParseAnimations(node);
		}
	}

	public void ParseAnimations(XmlNode node)
	{
		BMPNBKLELPH.Clear();
		int num = 0;
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "AnimationFactors")
			{
				BMPNBKLELPH.Add(new global::Pair<string, TacticValue>(string.Empty, new TacticValue()));
				BMPNBKLELPH[num].First = childNode.Attributes["Animation"].CIPOICEEIBK(string.Empty);
				BMPNBKLELPH[num].Second.Parse(childNode);
				num++;
			}
			else if (childNode.Name == "CurrentAnimation")
			{
				global::Pair<InfoAnimation, float> cCKLNOPEKHO = new global::Pair<InfoAnimation, float>(null, 0f);
				cCKLNOPEKHO.First = AnimationData.BCIFKBJAFEC(childNode.Attributes["Animation"].CIPOICEEIBK(string.Empty));
				cCKLNOPEKHO.Second = childNode.Attributes["Factor"].ParseFloat();
				string text = childNode.Attributes["Player"].CIPOICEEIBK("Me");
				if (text == "Enemy")
				{
					MHMDMMGJLEH.Add(cCKLNOPEKHO);
				}
				else if (text == "Me")
				{
					GKNOGDMPNHC.Add(cCKLNOPEKHO);
				}
			}
		}
	}

	public float GetValue(TacticFactors JCICKLIMBEF)
	{
		float num = JCICKLIMBEF.EOGLBDCLMBM * FJNMMCGLOJH;
		float num2 = JCICKLIMBEF.KFMJMBANIGF * KCPECJLPBAH;
		float num3 = (1f - JCICKLIMBEF.MGICNNKKCAN) * MEOEEBFJHAA;
		float num4 = (1f - JCICKLIMBEF.DDGNCMJGDAG) * BFCLFALKDJE;
		float num5 = (float)JCICKLIMBEF.OLCKGMBDGOG * MPJNDEEMEAC;
		float num6 = (float)JCICKLIMBEF.JJDNDOLCMMN * GIGOKAGAAHE;
		float num7 = (float)JCICKLIMBEF.DNPPDCPPGLM * KAMCPBICLNJ;
		float num8 = JCICKLIMBEF.AAKOCIPFDNM * AIFFLEEOLHG;
		float num9 = (float)JCICKLIMBEF.NGMLGDJGBCD * PBLKIFMDEHC;
		float num10 = JCICKLIMBEF.DDFBIOFIDIH * FLLAGMNBGLB;
		float num11 = num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9 + num10 + HAOPIJJPNBD;
		foreach (global::Pair<string, TacticValue> item in BMPNBKLELPH)
		{
			float count = 0f;
			float CKKFKEIELCP = 0f;
			float JOOJIMPEPOJ = 0f;
			JCICKLIMBEF.FAKEJAAEPJG.GetCountAndDamage(true, item.First, ref count, ref CKKFKEIELCP, ref JOOJIMPEPOJ);
			float num12 = count * item.Second.FJNMMCGLOJH;
			float num13 = CKKFKEIELCP * item.Second.KCPECJLPBAH;
			float num14 = JOOJIMPEPOJ * item.Second.AIFFLEEOLHG;
			num11 += num12 + num13 + num14;
		}
		num11 += GetAnimationSummands(JCICKLIMBEF.HDCPIAPMFNO, GKNOGDMPNHC);
		num11 += GetAnimationSummands(JCICKLIMBEF.PBDLLNEOIDG, MHMDMMGJLEH);
		if (MJKHAOMOOMK == NJDJLPHNAKG.Exponential)
		{
			return CalculateExponentialChance(num11);
		}
		if (MJKHAOMOOMK == NJDJLPHNAKG.Linear)
		{
			return CalculateLinearChance(num11);
		}
		return 0f;
	}

	private float CalculateExponentialChance(float IGAPINAEDPP)
	{
		float num = 0f;
		if (0f <= IGAPINAEDPP)
		{
			return _limit + (_base - _limit) * Mathf.Pow(2f, 0f - IGAPINAEDPP);
		}
		return LFCOMPLOFIM + (_base - LFCOMPLOFIM) * Mathf.Pow(2f, IGAPINAEDPP);
	}

	private float CalculateLinearChance(float IGAPINAEDPP)
	{
		float num = 0f;
		if (0f <= IGAPINAEDPP)
		{
			return _base + (_limit - _base) * Mathf.Min(1f, IGAPINAEDPP);
		}
		return _base + (LFCOMPLOFIM - _base) * Mathf.Min(1f, 0f - IGAPINAEDPP);
	}

	private void SetFactorType(string JNPHBPCMFEH)
	{
		if (JNPHBPCMFEH == "Linear")
		{
			MJKHAOMOOMK = NJDJLPHNAKG.Linear;
		}
		else if (JNPHBPCMFEH == "Exponential")
		{
			MJKHAOMOOMK = NJDJLPHNAKG.Exponential;
		}
		else
		{
			MJKHAOMOOMK = NJDJLPHNAKG.Linear;
		}
	}

	private float GetAnimationSummands(InfoAnimation DBOLBEOCEME, List<global::Pair<InfoAnimation, float>> JCJDOODBPBB)
	{
		foreach (global::Pair<InfoAnimation, float> item in JCJDOODBPBB)
		{
			if (DBOLBEOCEME == item.First)
			{
				return item.Second;
			}
		}
		return 0f;
	}

	private void CopyFrom(TacticValue JFMALLHPPMH)
	{
		_base = JFMALLHPPMH._base;
		FJNMMCGLOJH = JFMALLHPPMH.FJNMMCGLOJH;
		KCPECJLPBAH = JFMALLHPPMH.KCPECJLPBAH;
		MEOEEBFJHAA = JFMALLHPPMH.MEOEEBFJHAA;
		BFCLFALKDJE = JFMALLHPPMH.BFCLFALKDJE;
		MPJNDEEMEAC = JFMALLHPPMH.MPJNDEEMEAC;
		PBLKIFMDEHC = JFMALLHPPMH.PBLKIFMDEHC;
		GIGOKAGAAHE = JFMALLHPPMH.GIGOKAGAAHE;
		KAMCPBICLNJ = JFMALLHPPMH.KAMCPBICLNJ;
		AIFFLEEOLHG = JFMALLHPPMH.AIFFLEEOLHG;
		_limit = JFMALLHPPMH._limit;
		LFCOMPLOFIM = JFMALLHPPMH.LFCOMPLOFIM;
		HAOPIJJPNBD = JFMALLHPPMH.HAOPIJJPNBD;
		BMPNBKLELPH = JFMALLHPPMH.BMPNBKLELPH;
		MJKHAOMOOMK = JFMALLHPPMH.MJKHAOMOOMK;
		FLLAGMNBGLB = JFMALLHPPMH.FLLAGMNBGLB;
		GKNOGDMPNHC = JFMALLHPPMH.GKNOGDMPNHC;
		MHMDMMGJLEH = JFMALLHPPMH.MHMDMMGJLEH;
	}
}
