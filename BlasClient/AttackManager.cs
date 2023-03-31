using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Effects.Player.Sparks;
using BlasClient.MonoBehaviours;
using System.Collections;
using UnityEngine;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        public void SwordAttack(byte type)
        {
            Main.Multiplayer.LogError("Attacking");
            Main.Multiplayer.SendNewAttack(type);
        }

        public void HitReceived(string playerName, byte attack)
        {
            OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(playerName);
            if (Core.Logic.Penitent == null || other == null) return;

            // If attack is 255, that means this is not an attack and simply an acknowledgment that the player 'playerName' was hit
            if (attack == 255)
            {
                AcknowledgeHit(other);
                return;
            }

            // Play attack animation based on the attack type
            other.PlayAttackAnimation(attack);

            // Calculate delay
            float delay = attack == 10 ? 0.35f : 0.15f; // Longer for charged attack

            // Apply damage to player if the attack connects and they are on the other team
            if (Main.Multiplayer.playerTeam != Main.Multiplayer.playerList.getPlayerTeam(playerName))
                Main.Instance.StartCoroutine(CauseDamageAfterDelay(other, attack, delay));
        }

        private void ProcessHit(byte attack, OtherPenitent attacker)
        {
            if (Core.Logic.Penitent == null || attacker == null)
                return;

            // Calculate attack area based on attack
            Collider2D attackerArea = attacker.GetAttackArea(attack);
            if (attackerArea == null) return;

            bool hitPlayer = false;
            Collider2D[] colliders = Physics2D.OverlapAreaAll(attackerArea.bounds.min, attackerArea.bounds.max, 1 << 18);
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.name == "Body")
                {
                    hitPlayer = true;
                    break;
                }
            }
            if (!hitPlayer) return;

            // Calculate hit data based on attack & parameters
            Hit hit = new Hit()
            {
                AttackingEntity = attacker.gameObject,
                DamageAmount = 10,
                //Force = 1,
                DamageElement = DamageArea.DamageElement.Normal,
                DamageType = DamageArea.DamageType.Normal,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                // Sound
            };
            if (attack == 10)
            {
                hit.DamageType = DamageArea.DamageType.Heavy;
                hit.Force = 2;
            }

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
            Main.Multiplayer.SendNewAttack(255);
        }

        private IEnumerator CauseDamageAfterDelay(OtherPenitent attacker, byte attack, float delay)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            ProcessHit(attack, attacker);
        }

        private void AcknowledgeHit(OtherPenitent attackedPlayer)
        {
            Main.Multiplayer.LogWarning("Player " + attackedPlayer.name + " was hit!  Drawing blood effects!");
            Vector3 effectPosition = attackedPlayer.transform.position + Vector3.up;

            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
        }

        // Store all attack data (Delay, damage, hitbox) in separate classes
    }
}
