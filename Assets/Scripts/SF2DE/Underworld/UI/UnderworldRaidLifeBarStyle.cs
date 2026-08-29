using Nekki.SF2.GUI;
using UnityEngine;

namespace SF2DE.Underworld.UI
{
	public sealed class UnderworldRaidLifeBarStyle
	{
		private bool _capturedNormalStyle;
		private string _normalHealthSprite;
		private string _normalBackgroundSprite;
		private Color _normalBackgroundColor;
		private Color _normalHealthColor;

		public void Apply(ResolutionImageSkew healthBar, ResolutionImageSkew background, bool raidBoss)
		{
			if (healthBar == null || background == null)
			{
				return;
			}
			if (!_capturedNormalStyle)
			{
				_normalHealthSprite = healthBar.get_SpriteName();
				_normalBackgroundSprite = background.get_SpriteName();
				_normalBackgroundColor = background.color;
				_normalHealthColor = healthBar.color;
				_capturedNormalStyle = true;
			}

			// Use the recovered blue gradient, not a tint of the red/orange bar.
			// All layers retain the prefab's fixed width, skew and fill direction.
			// The ordinary gold hit layer remains the delayed damage indicator.
			healthBar.set_SpriteName(raidBoss ? "FightUI.Raid_HealthBar_Full" : _normalHealthSprite);
			background.set_SpriteName(raidBoss ? "FightUI.Raid_HealthBar_Full" : _normalBackgroundSprite);
			healthBar.color = raidBoss ? Color.white : _normalHealthColor;
			background.color = raidBoss ? new Color(0.25f, 0.44f, 0.38f, 1f) : _normalBackgroundColor;
		}
	}
}
