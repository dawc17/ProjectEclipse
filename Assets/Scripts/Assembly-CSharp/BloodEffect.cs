using System;
using SF2DE.Rendering.Interpolation;
using UnityEngine;

internal class BloodEffect
{
	private GameObject _UnityObject;

	public int index;

	public int BIPGHENGCFI;

	private Vector3f KKIKIDNALOL = new Vector3f();

	private int frames;

	private FightTransformInterpolation _Interpolation;

	public BloodEffect(Vector3f JLHLMAFLMFO)
	{
		_UnityObject = new GameObject("BloodEffect");
		_Interpolation = _UnityObject.AddComponent<FightTransformInterpolation>();
		_Interpolation.Snap(Vector3.zero, Quaternion.identity);
		frames = 0;
		index = 0;
		BIPGHENGCFI = 0;
		int min = -40;
		int max = 40;
		int min2 = -60;
		int max2 = 20;
		float num = 200f;
		KKIKIDNALOL.JPFALPBDBAP(JLHLMAFLMFO.GILCBJJPKBK() / num + (float)UnityEngine.Random.Range(min, max) / 10f);
		KKIKIDNALOL.IBNFLLGPOLD(JLHLMAFLMFO.OBIMBNIBEFG() / num + (float)UnityEngine.Random.Range(min2, max2) / 10f);
	}

	public void CreateSprite(string ONEIGMLOGDC, Color OHJKNABLCMF)
	{
		SpriteRenderer spriteRenderer = _UnityObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = ResourcesAndBundles.Load<Sprite>(ONEIGMLOGDC);
		spriteRenderer.color = OHJKNABLCMF;
	}

	public void SetParent(GameObject PKHKBAJOHHF)
	{
		_UnityObject.transform.SetParent(PKHKBAJOHHF.transform, false);
	}

	public void Render()
	{
		Vector3 localPosition = _Interpolation.CurrentPosition;
		localPosition.x += KKIKIDNALOL.GILCBJJPKBK();
		localPosition.y += KKIKIDNALOL.OBIMBNIBEFG();
		Vector3f kKIKIDNALOL = KKIKIDNALOL;
		kKIKIDNALOL.IBNFLLGPOLD(kKIKIDNALOL.OBIMBNIBEFG() + 0.2f);
		int num = ((!(KKIKIDNALOL.GILCBJJPKBK() < 0f)) ? 1 : (-1));
		float z = Mathf.Atan((0f - KKIKIDNALOL.OBIMBNIBEFG()) / KKIKIDNALOL.GILCBJJPKBK()) / (float)Math.PI * 180f - 90f * (float)num + 180f;
		Quaternion worldRotation = Quaternion.Euler(0f, 0f, z);
		Quaternion localRotation = (_UnityObject.transform.parent == null) ? worldRotation : Quaternion.Inverse(_UnityObject.transform.parent.rotation) * worldRotation;
		_Interpolation.Push(localPosition, localRotation);
	}

	public void SetPosition(Vector3f NAAPALOFBCI)
	{
		_Interpolation.Snap(new Vector3(NAAPALOFBCI.GILCBJJPKBK(), NAAPALOFBCI.OBIMBNIBEFG(), 0f), _Interpolation.CurrentRotation);
	}

	public void SetScale(float JDCCBCNFENK)
	{
		_UnityObject.transform.localScale = new Vector3(JDCCBCNFENK, JDCCBCNFENK, JDCCBCNFENK);
	}

	public void AGNODHKEJCJ()
	{
		UnityEngine.Object.Destroy(_UnityObject);
	}
}
