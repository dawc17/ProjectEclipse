using System.Collections.Generic;

public class Bezier
{
	private Vector3f MNMDLCKNLDJ = new Vector3f();

	private Vector3f GAHMKEODMPJ = new Vector3f();

	private float OKEMEKMBMPF;

	private float PBLPKOLLBCL;

	private float NJPFIIODEHP;

	private int _count;

	public Bezier(int count)
	{
		OKEMEKMBMPF = 1f / (float)count;
		PBLPKOLLBCL = (0f - OKEMEKMBMPF) / (float)count;
		NJPFIIODEHP = 0f - PBLPKOLLBCL - PBLPKOLLBCL;
		_count = count;
		OKEMEKMBMPF += OKEMEKMBMPF;
	}

	private void JJJAPLECBOD(Vector3f HAEJICBDOKC, Vector3f MILMANCOCLK, Vector3f DMECFLFKOPA, int count, List<Vector3f> OEMALIFPGPO)
	{
		float num = PBLPKOLLBCL;
		if (OEMALIFPGPO.Count != count)
		{
			OEMALIFPGPO.CPCAJIKOIEE(count);
			for (int i = 0; i < count; i++)
			{
				if (Vector2f.LFPMCJPCJBD(OEMALIFPGPO[i], null))
				{
					OEMALIFPGPO[i] = new Vector3f();
				}
			}
		}
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 0f;
		foreach (Vector3f item in OEMALIFPGPO)
		{
			num += NJPFIIODEHP;
			num2 -= OKEMEKMBMPF - num;
			num3 += OKEMEKMBMPF - num - num;
			num4 += num;
			item.JPFALPBDBAP(num2 * HAEJICBDOKC.GILCBJJPKBK() + num3 * MILMANCOCLK.GILCBJJPKBK() + num4 * DMECFLFKOPA.GILCBJJPKBK());
			item.IBNFLLGPOLD(num2 * HAEJICBDOKC.OBIMBNIBEFG() + num3 * MILMANCOCLK.OBIMBNIBEFG() + num4 * DMECFLFKOPA.OBIMBNIBEFG());
			item.set_Z(num2 * HAEJICBDOKC.KMFEKANLCFO() + num3 * MILMANCOCLK.KMFEKANLCFO() + num4 * DMECFLFKOPA.KMFEKANLCFO());
		}
	}

	public void CFCFNHONDML(Vector3f MLGFPMDKOHD, Vector3f DMMNCDKPCCI, Vector3f PIBOFKAMIDL, List<Vector3f> OEMALIFPGPO)
	{
		MNMDLCKNLDJ.SetMiddlePoint3D(MLGFPMDKOHD, DMMNCDKPCCI);
		GAHMKEODMPJ.SetMiddlePoint3D(DMMNCDKPCCI, PIBOFKAMIDL);
		JJJAPLECBOD(MNMDLCKNLDJ, DMMNCDKPCCI, GAHMKEODMPJ, _count, OEMALIFPGPO);
	}
}
