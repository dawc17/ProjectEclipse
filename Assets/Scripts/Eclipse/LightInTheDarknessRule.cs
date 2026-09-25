using System.Xml;

namespace Eclipse.Combat
{
    // Recovered vanilla rule shape: a spotlight follows one fighter while the
    // rest of the arena is dark. The later native build stores Radius and Shape
    // on its material and updates Center every render frame.
    public sealed class LightInTheDarknessRule : InFightRule
    {
        public float LightRadius { get; private set; }
        public float LightShape { get; private set; }

        public LightInTheDarknessRule(XmlNode node, RuleAppliance target)
            : base(BCBLLMPAMLP.RuleLightInTheDarkness, target, node)
        {
            LightRadius = node.Attributes["LightRadius"].ParseFloat(0.2f);
            LightShape = node.Attributes["LightShape"].ParseFloat(1f);
            EBJIKKBLBEM(FightEvent.RenderEvent);
        }

        public override InFightRule Copy()
        {
            return new LightInTheDarknessRule(GIFDJEEGCJI().IOJIGDNFCFL(), EDAKADCHOLE())
            {
                IsRandom = IsRandom
            };
        }
    }
}
