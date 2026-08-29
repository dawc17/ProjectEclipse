using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Underworld.Diagnostics
{
	public static class UnderworldRaidDiagnostics
	{
		// Keep the reference XML's WarriorPower, equipment and AttributesAlign.
		// Replacing them with arbitrary player-relative offsets erased boss tiers
		// and made equipment upgrades ineffective. Alignment is evaluated at hit time.
		public static void LogEnemies(FightList fight, ModelParameters player, List<ModelParameters> enemies)
		{
			Battle battle = fight.CNAOMDMIGLJ;
			Zone zone = battle == null ? null : battle.OAEIILGHJMG;
			if (!UnderworldZonePolicy.IsRaidZone(zone))
			{
				return;
			}

			foreach (ModelParameters enemy in enemies)
			{
				int weapon = 0;
				int defense = 0;
				int playerWeapon = 0;
				int playerDefense = 0;
				enemy.IBLHIAHECLK.Get("WeaponDamage", ref weapon);
				enemy.IBLHIAHECLK.Get("BodyDefense", ref defense);
				player.IBLHIAHECLK.Get("WeaponDamage", ref playerWeapon);
				player.IBLHIAHECLK.Get("BodyDefense", ref playerDefense);
				Debug.Log("[Underworld] battle=" + battle.get_Name() +
					" warriorPower=" + enemy.FPIMGHKNHMO + " healthBars=" + enemy.HealthBarCount +
					" bossWeapon=" + weapon + " bossDefense=" + defense +
					" playerWeapon=" + playerWeapon + " playerDefense=" + playerDefense +
					" alignmentRules=" + enemy.FKJBBIMPCBB.Count);
			}
		}
	}
}
