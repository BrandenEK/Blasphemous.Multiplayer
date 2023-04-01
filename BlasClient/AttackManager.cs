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
            if (Core.Logic.Penitent == null) return;

            if (receiverName == Main.Multiplayer.playerName)
            {
                // This is the player that got hit
                OtherPenitent attacker = Main.Multiplayer.playerManager.getPlayerObject(attackerName);
                if (attacker == null) return;

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

            //OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(attackerName);
            // If attack is 255, that means this is not an attack and simply an acknowledgment that the player 'playerName' was hit
            //if (attack == 255)
            //{
            //    AcknowledgeHit(other);
            //    return;
            //}

            // If invalid attack type, return
            //if (!attackTypes.ContainsKey(attack)) return;
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
                // Sound
            };
            hit.DamageAmount = 20;
            hit.DamageType = DamageArea.DamageType.Normal;
            hit.Force = 1;

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

        // Store all attack data (Delay, damage, hitbox) in separate classes
        private readonly Dictionary<byte, PlayerAttack> attackTypes = new Dictionary<byte, PlayerAttack>()
        {
            { 0, new SidewaysAttack() },
            { 1, new UpwardsAttack() },
            { 2, new SidewaysAttack() },
            { 3, new UpwardsAttack() },
            { 4, new CrouchAttack() },
            { 10, new ChargedAttack() },
        };
    }
}
