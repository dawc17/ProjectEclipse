using UnityEngine;
using UnityEngine.UI;

public class PreciseLocaleText : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Text>().text = string.Format("LANGUAGE ID: {0} \nLANGUAGE: {1} \n REGION: {2} \n CURRENCY CODE: {3} \n CURRENCY SYMBOL: {4}", PreciseLocale.BGMAJFGKCEB(), PreciseLocale.PBPAPAFAMJB(), PreciseLocale.FBPILFMCNGJ(), PreciseLocale.OHHPBPBCFPL(), PreciseLocale.HIMMFECDKCI());
	}
}
