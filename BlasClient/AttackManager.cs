using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using BlasClient.MonoBehaviours;
using BlasClient.PvP;
using BlasClient.Structures;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        public void AttackReceived(string attackerName, string receiverName, AttackType attack)
        {
            if (Core.Logic.Penitent == null || !attackTypes.ContainsKey(attack)) return;

            if (receiverName == Main.Multiplayer.playerName)
            {
                // This is the player that got hit
                OtherPenitent attacker = Main.Multiplayer.playerManager.getPlayerObject(attackerName);
                if (attacker == null) return;

                Config config = Main.Multiplayer.config;
                if (!config.enablePvP || (!config.enableFriendlyFire && Main.Multiplayer.playerTeam == Main.Multiplayer.playerList.getPlayerTeam(attackerName)))
                    return;

                Main.Multiplayer.LogWarning($"Receiving hit {attack} from {attackerName}");
                DamagePlayer(attack, attacker.gameObject);
            }
            else
            {
                // It was a different player that got hit
                ShowDamageEffects(receiverName);
            }
        }

        public void EffectReceived(string playerName, EffectType effect)
        {
            OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(playerName);
            if (other == null) return;

            other.OtherPenitentAttack.PlayEffectAnimation(effect, other.IsFacingRight);
        }

        private void DamagePlayer(AttackType attack, GameObject attacker)
        {
            // Calculate hit data based on attack & parameters
            Hit hit = new Hit()
            {
                AttackingEntity = attacker,
                DamageElement = DamageArea.DamageElement.Normal,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
            };
            hit.DamageAmount = attackTypes[attack].BaseDamage;
            hit.DamageType = attackTypes[attack].DamageType;
            hit.Force = attackTypes[attack].Force;

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        public void ShowDamageEffects(string receiverName)
        {
            OtherPenitent receiver = Main.Multiplayer.playerManager.getPlayerObject(receiverName);
            if (receiver == null) return;

            Vector3 effectPosition = receiver.transform.position + Vector3.up;
            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        // Store all attack data in separate classes
        private readonly Dictionary<AttackType, PlayerAttack> attackTypes = new Dictionary<AttackType, PlayerAttack>()
        {
            { AttackType.Slash, new NormalAttack() },
            { AttackType.Charged, new ChargedAttack() },
            { AttackType.Lunge, new LungeAttack() },
            { AttackType.Vertical, new VerticalAttack() },
            { AttackType.Ranged, new RangedAttack() },
            { AttackType.Debla, new DeblaAttack() },
            { AttackType.Verdiales, new VerdialesAttack() },
            { AttackType.Taranto, new TarantoAttack() },
            { AttackType.Tirana, new TiranaAttack() },
            { AttackType.PoisonMist, new PoisonMistAttack() },
            { AttackType.Shield, new ShieldAttack() },
            { AttackType.Miriam, new MiriamAttack() },
            { AttackType.Aubade, new AubadeAttack() },
        };
    }
}
