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
			item.SetX(num2 * HAEJICBDOKC.GetX() + num3 * MILMANCOCLK.GetX() + num4 * DMECFLFKOPA.GetX());
			item.SetY(num2 * HAEJICBDOKC.GetY() + num3 * MILMANCOCLK.GetY() + num4 * DMECFLFKOPA.GetY());
			item.SetZ(num2 * HAEJICBDOKC.GetZ() + num3 * MILMANCOCLK.GetZ() + num4 * DMECFLFKOPA.GetZ());
		}
	}

	public void CFCFNHONDML(Vector3f MLGFPMDKOHD, Vector3f DMMNCDKPCCI, Vector3f PIBOFKAMIDL, List<Vector3f> OEMALIFPGPO)
	{
		MNMDLCKNLDJ.SetMiddlePoint3D(MLGFPMDKOHD, DMMNCDKPCCI);
		GAHMKEODMPJ.SetMiddlePoint3D(DMMNCDKPCCI, PIBOFKAMIDL);
		JJJAPLECBOD(MNMDLCKNLDJ, DMMNCDKPCCI, GAHMKEODMPJ, _count, OEMALIFPGPO);
	}
}
