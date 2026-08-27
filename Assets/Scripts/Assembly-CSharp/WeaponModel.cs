public class WeaponModel : Model
{
	private bool HLAOOPKDLGA;

	public override bool FDELMAHAAJD
	{
		get
		{
			return KIAFPPHPEEK();
		}
	}

	public WeaponModel(ModelParameters data)
		: base(data)
	{
	}

	public override bool KIAFPPHPEEK()
	{
		return true;
	}

	public override bool PlayAnimation(InfoAnimation CMGIPKIPIPA, int AOJJBKLCHJO = 0, bool HHJGACBCGBP = false, int BADKABIKMBD = -1)
	{
		bool flag = !HLAOOPKDLGA;
		if (!HLAOOPKDLGA)
		{
			NJDJHGDMCIJ().KNCKHDNGKFO(this);
			HLAOOPKDLGA = true;
		}
		bool result = base.PlayAnimation(CMGIPKIPIPA, AOJJBKLCHJO, HHJGACBCGBP, BADKABIKMBD);
		if (flag)
		{
			OCPMJKIEPIG().Render();
		}
		return result;
	}
}
