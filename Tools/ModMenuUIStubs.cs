// Only for the isolated UI fixture. Production title/menu UI and preference code
// are copied unchanged; scene transition/save sequencing has its own test.
using UnityEngine;
public enum ScreenType { ModulePreloader }
public static class SceneManagerSF { public static int Loads; public static void Load(ScreenType t) { Loads++; } }
public static class GamePad
{
    public enum LCNPGEANNDP { LeftStick, RightStick }
    public enum PFENLAPGKFM { A }
    public enum GGAKHLLMPMM { One }
    public enum HKKPDLMCPIF { Left, Right }
    public static bool NFCGBMHPKMA(PFENLAPGKFM a, GGAKHLLMPMM b) => false;
    public static float MAJINGINCHM(HKKPDLMCPIF a, GGAKHLLMPMM b, bool c) => 0;
}
namespace Nekki.SF2.GUI.Menu
{
    public class MainMenu : MonoBehaviour { public MenuScroll Scroll; }
    public class MenuScroll : MonoBehaviour
    {
        public enum ANJKEGGALAG { ScrollOpen, ScrollClose }
        public ANJKEGGALAG CurScrollState;
    }
}
