using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using BlasClient.MonoBehaviours;
using BlasClient.PvP;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        public void AttackReceived(string attackerName, string receiverName, byte attack)
        {
            if (Core.Logic.Penitent == null || !attackTypes.ContainsKey(attack)) return;

            if (receiverName == Main.Multiplayer.playerName)
            {
                // This is the player that got hit
                OtherPenitent attacker = Main.Multiplayer.playerManager.getPlayerObject(attackerName);
                if (attacker == null) return;

                Main.Multiplayer.Log($"Receiving hit {attack} from {attackerName}");
                DamagePlayer(attack, attacker.gameObject);
            }
            else
            {
                // It was a different player that got hit
                OtherPenitent receiver = Main.Multiplayer.playerManager.getPlayerObject(receiverName);
                if (receiver == null) return;

                Main.Multiplayer.LogWarning("Player " + receiverName + " was hit!  Drawing blood effects!"); // remove later
                ShowDamageEffects(receiver.gameObject);
            }
        }

        public void EffectReceived(string playerName, byte effect)
        {
            OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(playerName);
            if (other == null) return;

            other.OtherPenitentAttack.PlayAttackAnimation(effect, other.IsFacingRight);
        }

        private void DamagePlayer(byte attack, GameObject attacker)
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

        private void ShowDamageEffects(GameObject receiver)
        {
            Vector3 effectPosition = receiver.transform.position + Vector3.up;

            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        // Store all attack data in separate classes
        private readonly Dictionary<byte, PlayerAttack> attackTypes = new Dictionary<byte, PlayerAttack>()
        {
            { 0, new NormalAttack() },
            { 10, new ChargedAttack() },
            { 11, new LungeAttack() },
            { 12, new VerticalAttack() },
        };
    }
}
