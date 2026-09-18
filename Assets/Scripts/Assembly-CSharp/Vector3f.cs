using System;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class Vector3f : Vector2f
{
	private float Z;

	public float MMHBGPFEICN
	{
		get
		{
			return GetZ();
		}
		set
		{
			SetZ(value);
		}
	}

	public float IHGONCCOKMK
	{
		get
		{
			return KLIOMCPELLF();
		}
	}

	public float EDEIJKAKONA
	{
		get
		{
			return IGJNMAOKEKK();
		}
	}

	public static Vector3f NNCHJCLKHHA
	{
		get
		{
			return GGGOJLHLFOJ();
		}
	}

	public static Vector3f FJBHJIFKOMF
	{
		get
		{
			return LOOCMJMEFIM();
		}
	}

	public static Vector3f DDLDNKIDIGO
	{
		get
		{
			return JCFHLFIGEMK();
		}
	}

	public static Vector3f Zero
	{
		get
		{
			return GIJAIFMOOOI();
		}
	}

	public static Vector3f MOAPAKKGMPF
	{
		get
		{
			return GMFLBGGIMBL();
		}
	}

	public Vector3f()
	{
		X = (Y = (Z = 0f));
	}

	public Vector3f(float LHNJJFDIJKK, float FFFHIOALHGM = 0f, float PDCENMEKIAP = 0f)
	{
		X = LHNJJFDIJKK;
		Y = FFFHIOALHGM;
		Z = PDCENMEKIAP;
	}

	public Vector3f(Vector3f BEHOPOPCJGB)
	{
		X = BEHOPOPCJGB.GetX();
		Y = BEHOPOPCJGB.GetY();
		Z = BEHOPOPCJGB.GetZ();
	}

	public Vector3f(Vector3 BEHOPOPCJGB)
	{
		X = BEHOPOPCJGB.x;
		Y = BEHOPOPCJGB.y;
		Z = BEHOPOPCJGB.z;
	}

	public float GetZ()
	{
		return Z;
	}

	public void SetZ(float value)
	{
		Z = value;
	}

	public float KLIOMCPELLF()
	{
		return Mathf.Sqrt(X * X + Y * Y + Z * Z);
	}

	public float IGJNMAOKEKK()
	{
		return Mathf.Sqrt(X * X + Y * Y);
	}

	public static Vector3f GGGOJLHLFOJ()
	{
		return new Vector3f(1f);
	}

	public static Vector3f LOOCMJMEFIM()
	{
		return new Vector3f(0f, 1f);
	}

	public static Vector3f JCFHLFIGEMK()
	{
		return new Vector3f(0f, 1f);
	}

	public static Vector3f GIJAIFMOOOI()
	{
		return new Vector3f(0f);
	}

	public static Vector3f GMFLBGGIMBL()
	{
		return new Vector3f(1f, 1f, 1f);
	}

	[SpecialName]
	public static Vector3f PHEFFKMOOCM(Vector3f BEHOPOPCJGB, float LIAILCGJBDK)
	{
		return new Vector3f(BEHOPOPCJGB.GetX() + LIAILCGJBDK, BEHOPOPCJGB.GetY() + LIAILCGJBDK, BEHOPOPCJGB.GetZ() + LIAILCGJBDK);
	}

	[SpecialName]
	public static Vector3f PHEFFKMOOCM(Vector3f NBMEGFBPGFE, Vector3f AKKEJFKBIHF)
	{
		return new Vector3f(NBMEGFBPGFE.GetX() + AKKEJFKBIHF.GetX(), NBMEGFBPGFE.GetY() + AKKEJFKBIHF.GetY(), NBMEGFBPGFE.GetZ() + AKKEJFKBIHF.GetZ());
	}

	[SpecialName]
	public static Vector3f MJOKEBGPHKB(Vector3f BEHOPOPCJGB, float LIAILCGJBDK)
	{
		return new Vector3f(BEHOPOPCJGB.GetX() - LIAILCGJBDK, BEHOPOPCJGB.GetY() - LIAILCGJBDK, BEHOPOPCJGB.GetZ() - LIAILCGJBDK);
	}

	[SpecialName]
	public static Vector3f MJOKEBGPHKB(Vector3f NBMEGFBPGFE, Vector3f AKKEJFKBIHF)
	{
		return new Vector3f(NBMEGFBPGFE.GetX() - AKKEJFKBIHF.GetX(), NBMEGFBPGFE.GetY() - AKKEJFKBIHF.GetY(), NBMEGFBPGFE.GetZ() - AKKEJFKBIHF.GetZ());
	}

	[SpecialName]
	public static Vector3f op_Multiply(Vector3f BEHOPOPCJGB, float LIAILCGJBDK)
	{
		return new Vector3f(BEHOPOPCJGB.GetX() * LIAILCGJBDK, BEHOPOPCJGB.GetY() * LIAILCGJBDK, BEHOPOPCJGB.GetZ() * LIAILCGJBDK);
	}

	[SpecialName]
	public static float op_Multiply(Vector3f NBMEGFBPGFE, Vector3f AKKEJFKBIHF)
	{
		return NBMEGFBPGFE.GetX() * AKKEJFKBIHF.GetX() + NBMEGFBPGFE.GetY() * AKKEJFKBIHF.GetY() + NBMEGFBPGFE.GetZ() * AKKEJFKBIHF.GetZ();
	}

	[SpecialName]
	public static Vector3 op_Implicit(Vector3f BEHOPOPCJGB)
	{
		return new Vector3(BEHOPOPCJGB.GetX(), BEHOPOPCJGB.GetY(), BEHOPOPCJGB.GetZ());
	}

	[SpecialName]
	public static Vector3f op_Implicit(Vector3 BEHOPOPCJGB)
	{
		return new Vector3f(BEHOPOPCJGB.x, BEHOPOPCJGB.y, BEHOPOPCJGB.z);
	}

	public static float Distance(Vector3f NBMEGFBPGFE, Vector3f AKKEJFKBIHF)
	{
		float num = AKKEJFKBIHF.X - NBMEGFBPGFE.X;
		float num2 = AKKEJFKBIHF.Y - NBMEGFBPGFE.Y;
		float num3 = AKKEJFKBIHF.Z - NBMEGFBPGFE.Z;
		return Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	public float Distance(Vector3f BEHOPOPCJGB)
	{
		return Distance(BEHOPOPCJGB, this);
	}

	public Vector3f Add(float LIAILCGJBDK)
	{
		X += LIAILCGJBDK;
		Y += LIAILCGJBDK;
		Z += LIAILCGJBDK;
		return this;
	}

	public Vector3f Add(float LHNJJFDIJKK, float FFFHIOALHGM, float PDCENMEKIAP)
	{
		X += LHNJJFDIJKK;
		Y += FFFHIOALHGM;
		Z += PDCENMEKIAP;
		return this;
	}

	public Vector3f Add(Vector3f BEHOPOPCJGB)
	{
		X += BEHOPOPCJGB.GetX();
		Y += BEHOPOPCJGB.GetY();
		Z += BEHOPOPCJGB.GetZ();
		return this;
	}

	public void Add(Vector3f BEHOPOPCJGB, float LMBKGOKPDGM)
	{
		X += BEHOPOPCJGB.X * LMBKGOKPDGM;
		Y += BEHOPOPCJGB.Y * LMBKGOKPDGM;
		Z += BEHOPOPCJGB.Z * LMBKGOKPDGM;
	}

	public void GLGNIMKANCA(Vector3f BEHOPOPCJGB, float LMBKGOKPDGM)
	{
		X += BEHOPOPCJGB.X * LMBKGOKPDGM;
		Y += BEHOPOPCJGB.Y * LMBKGOKPDGM;
	}

	public new Vector3f Add(Vector2f PALAIICCALN)
	{
		X += PALAIICCALN.GetX();
		Y += PALAIICCALN.GetY();
		return this;
	}

	public Vector3f Subtract(float LIAILCGJBDK)
	{
		X -= LIAILCGJBDK;
		Y -= LIAILCGJBDK;
		Z -= LIAILCGJBDK;
		return this;
	}

	public Vector3f Subtract(float LHNJJFDIJKK, float FFFHIOALHGM, float PDCENMEKIAP)
	{
		X -= LHNJJFDIJKK;
		Y -= FFFHIOALHGM;
		Z -= PDCENMEKIAP;
		return this;
	}

	public Vector3f Subtract(Vector3f BEHOPOPCJGB)
	{
		X -= BEHOPOPCJGB.GetX();
		Y -= BEHOPOPCJGB.GetY();
		Z -= BEHOPOPCJGB.GetZ();
		return this;
	}

	public new Vector3f EHGLHOGAIDI(Vector2f PALAIICCALN)
	{
		X -= PALAIICCALN.GetX();
		Y -= PALAIICCALN.GetY();
		return this;
	}

	public new Vector3f Multiply(float LIAILCGJBDK)
	{
		X *= LIAILCGJBDK;
		Y *= LIAILCGJBDK;
		Z *= LIAILCGJBDK;
		return this;
	}

	public Vector3f Multiply(float LHNJJFDIJKK, float FFFHIOALHGM, float PDCENMEKIAP)
	{
		X *= LHNJJFDIJKK;
		Y *= FFFHIOALHGM;
		Z *= PDCENMEKIAP;
		return this;
	}

	public Vector3f Cross(Vector3f BEHOPOPCJGB)
	{
		return new Vector3f(Y * BEHOPOPCJGB.GetZ() - Z * BEHOPOPCJGB.GetY(), Z * BEHOPOPCJGB.GetX() - X * BEHOPOPCJGB.GetZ(), X * BEHOPOPCJGB.GetY() - Y - BEHOPOPCJGB.GetX());
	}

	public bool IsEqual(float CBNDEKHFGIJ, float NHELNADHNBA, float FEAAEKAELOH)
	{
		return X == CBNDEKHFGIJ && Y == NHELNADHNBA && Z == FEAAEKAELOH;
	}

	public static Vector3f Cross(Vector3f GIIIFLBEONP, Vector3f OIFPGODHFGH, Vector3f HNAPOAIMLGE, Vector3f LOJLJJPGPEC)
	{
		float num = (OIFPGODHFGH.GetY() - GIIIFLBEONP.GetY()) * (HNAPOAIMLGE.GetX() - LOJLJJPGPEC.GetX()) - (HNAPOAIMLGE.GetY() - LOJLJJPGPEC.GetY()) * (OIFPGODHFGH.GetX() - GIIIFLBEONP.GetX());
		float num2 = (OIFPGODHFGH.GetY() - GIIIFLBEONP.GetY()) * (HNAPOAIMLGE.GetX() - GIIIFLBEONP.GetX()) - (HNAPOAIMLGE.GetY() - GIIIFLBEONP.GetY()) * (OIFPGODHFGH.GetX() - GIIIFLBEONP.GetX());
		float num3 = (HNAPOAIMLGE.GetY() - GIIIFLBEONP.GetY()) * (HNAPOAIMLGE.GetX() - LOJLJJPGPEC.GetX()) - (HNAPOAIMLGE.GetY() - LOJLJJPGPEC.GetY()) * (HNAPOAIMLGE.GetX() - GIIIFLBEONP.GetX());
		if ((double)num == 0.0 && (double)num2 == 0.0 && (double)num3 == 0.0)
		{
			return null;
		}
		if ((double)num == 0.0)
		{
			return null;
		}
		float num4 = num2 / num;
		float num5 = num3 / num;
		float lHNJJFDIJKK = GIIIFLBEONP.GetX() + (OIFPGODHFGH.GetX() - GIIIFLBEONP.GetX()) * num5;
		float fFFHIOALHGM = GIIIFLBEONP.GetY() + (OIFPGODHFGH.GetY() - GIIIFLBEONP.GetY()) * num5;
		if (0f < num4 && num4 < 1f && ((0f < num5) & (num5 < 1f)))
		{
			return new Vector3f(lHNJJFDIJKK, fFFHIOALHGM);
		}
		return null;
	}

	public static Vector3f Middle(Vector3f GIIIFLBEONP, Vector3f OIFPGODHFGH)
	{
		return PHEFFKMOOCM(GIIIFLBEONP, op_Multiply(MJOKEBGPHKB(OIFPGODHFGH, GIIIFLBEONP), 0.5f));
	}

	public static void Middle(Vector3f GIIIFLBEONP, Vector3f OIFPGODHFGH, Vector3f AMKKLMOONEP)
	{
		AMKKLMOONEP.X = GIIIFLBEONP.X + (OIFPGODHFGH.X - GIIIFLBEONP.X) * 0.5f;
		AMKKLMOONEP.Y = GIIIFLBEONP.Y + (OIFPGODHFGH.Y - GIIIFLBEONP.Y) * 0.5f;
		AMKKLMOONEP.Z = GIIIFLBEONP.Z + (OIFPGODHFGH.Z - GIIIFLBEONP.Z) * 0.5f;
	}

	public static Vector3f Closest(Vector3f PALAIICCALN, Vector3f NJIAPLENBIL, Vector3f COMFFMDIPBM)
	{
		Vector3f nBMEGFBPGFE = MJOKEBGPHKB(PALAIICCALN, NJIAPLENBIL);
		float num = op_Multiply(nBMEGFBPGFE, COMFFMDIPBM);
		float num2 = op_Multiply(COMFFMDIPBM, COMFFMDIPBM);
		float lIAILCGJBDK = 0f;
		if (num2 != 0f)
		{
			lIAILCGJBDK = num / num2;
		}
		return PHEFFKMOOCM(NJIAPLENBIL, op_Multiply(COMFFMDIPBM, lIAILCGJBDK));
	}

	public void Reset()
	{
		X = 0f;
		Y = 0f;
		Z = 0f;
	}

	public static Vector3f Round(Vector3f BEHOPOPCJGB, float CFCPPNJKNAL)
	{
		BEHOPOPCJGB.SetX(Round(BEHOPOPCJGB.GetX(), CFCPPNJKNAL));
		BEHOPOPCJGB.SetY(Round(BEHOPOPCJGB.GetY(), CFCPPNJKNAL));
		BEHOPOPCJGB.SetZ(Round(BEHOPOPCJGB.GetZ(), CFCPPNJKNAL));
		return BEHOPOPCJGB;
	}

	public Vector3f Round(float CFCPPNJKNAL)
	{
		X = Round(X, CFCPPNJKNAL);
		Y = Round(Y, CFCPPNJKNAL);
		Z = Round(Z, CFCPPNJKNAL);
		return this;
	}

	public Vector3f NBDMEIKNJBG()
	{
		float num = KLIOMCPELLF();
		if (num != 0f)
		{
			num = 1f / num;
		}
		X *= num;
		Y *= num;
		Z *= num;
		return this;
	}

	public static Vector3f KBNMOFDDLOM(Vector3f NBMEGFBPGFE, Vector3f AKKEJFKBIHF)
	{
		return MJOKEBGPHKB(NBMEGFBPGFE, AKKEJFKBIHF).Cross(JCFHLFIGEMK()).NBDMEIKNJBG();
	}

	public static float Factor(Vector3f AKDMNIEKKKC, Vector3f FOIDOJMGNHP, Vector3f CJJFBEHAMBL)
	{
		FOIDOJMGNHP.SetZ(0f);
		AKDMNIEKKKC.SetZ(0f);
		CJJFBEHAMBL.SetZ(0f);
		return Distance(FOIDOJMGNHP, CJJFBEHAMBL) / Distance(FOIDOJMGNHP, AKDMNIEKKKC);
	}

	public Vector3f Clone()
	{
		return new Vector3f(X, Y, Z);
	}

	public Vector3f Set(Vector3f BEHOPOPCJGB)
	{
		X = BEHOPOPCJGB.X;
		Y = BEHOPOPCJGB.Y;
		Z = BEHOPOPCJGB.Z;
		return this;
	}

	public Vector3f Set(Vector3 BEHOPOPCJGB)
	{
		X = BEHOPOPCJGB.x;
		Y = BEHOPOPCJGB.y;
		Z = BEHOPOPCJGB.z;
		return this;
	}

	public Vector3f Set(float LHNJJFDIJKK = 0f, float FFFHIOALHGM = 0f, float PDCENMEKIAP = 0f)
	{
		X = LHNJJFDIJKK;
		Y = FFFHIOALHGM;
		Z = PDCENMEKIAP;
		return this;
	}

	public void SetMiddlePoint3D(Vector3f LHBNIMGFKIB, Vector3f AAOIAEJJINO)
	{
		X = LHBNIMGFKIB.GetX() + (AAOIAEJJINO.GetX() - LHBNIMGFKIB.GetX()) * 0.5f;
		Y = LHBNIMGFKIB.GetY() + (AAOIAEJJINO.GetY() - LHBNIMGFKIB.GetY()) * 0.5f;
		Z = LHBNIMGFKIB.GetZ() + (AAOIAEJJINO.GetZ() - LHBNIMGFKIB.GetZ()) * 0.5f;
	}

	public void OBGEKPIDMOP()
	{
		X = (int)X;
		Y = (int)Y;
		Z = (int)Z;
	}

	public static Vector3f Create(XmlNode MEEAKLDGLDF)
	{
		if (MEEAKLDGLDF == null)
		{
			return null;
		}
		Vector3f eMAFACPEPDK = new Vector3f();
		try
		{
			eMAFACPEPDK.X = float.Parse(MEEAKLDGLDF.Attributes["X"].Value);
		}
		catch
		{
			throw new Exception("Error : parse X eeror type");
		}
		try
		{
			eMAFACPEPDK.Y = float.Parse(MEEAKLDGLDF.Attributes["Y"].Value);
			return eMAFACPEPDK;
		}
		catch
		{
			throw new Exception("Error : parse Y eeror type");
		}
	}

	public override string ToString()
	{
		return "X=" + X.ToString("F4") + " Y=" + Y.ToString("F4") + " Z=" + Z.ToString("F4");
	}

	public new static float Round(float Value, float JMLMHGAMNKN)
	{
		return Mathf.Floor(Value * JMLMHGAMNKN + 0.5f) / JMLMHGAMNKN;
	}

	public static Vector3f GetDivisionPoint3D(Vector3f LHBNIMGFKIB, Vector3f AAOIAEJJINO, float ratio)
	{
		return new Vector3f(LHBNIMGFKIB.X + (AAOIAEJJINO.X - LHBNIMGFKIB.X) * ratio, LHBNIMGFKIB.Y + (AAOIAEJJINO.Y - LHBNIMGFKIB.Y) * ratio, LHBNIMGFKIB.Z + (AAOIAEJJINO.Z - LHBNIMGFKIB.Z) * ratio);
	}

	public static void GetDivisionPoint3D(Vector3f LHBNIMGFKIB, Vector3f AAOIAEJJINO, float ratio, Vector3f FFFLNOBCBGL)
	{
		FFFLNOBCBGL.Set(LHBNIMGFKIB.X + (AAOIAEJJINO.X - LHBNIMGFKIB.X) * ratio, LHBNIMGFKIB.Y + (AAOIAEJJINO.Y - LHBNIMGFKIB.Y) * ratio, LHBNIMGFKIB.Z + (AAOIAEJJINO.Z - LHBNIMGFKIB.Z) * ratio);
	}
}
