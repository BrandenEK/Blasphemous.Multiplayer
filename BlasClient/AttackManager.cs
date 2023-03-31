using Framework.Managers;
using Gameplay.GameControllers.Entities;
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

            // Play attack animation based on the attack type
            other.PlayAttackAnimation(attack);

            // Apply damage to player if the attack connects and they are on the other team
            if (Main.Multiplayer.playerTeam != Main.Multiplayer.playerList.getPlayerTeam(playerName))
                Main.Instance.StartCoroutine(CauseDamageAfterDelay(other, attack, 0.15f));
        }

        private void ProcessHit(byte attack, OtherPenitent attacker)
        {
            if (Core.Logic.Penitent == null || attacker == null)
                return;

            // Calculate attack area based on attack
            Vector3 playerPosition = Core.Logic.Penitent.transform.position;
            Vector3 attackerPosition = attacker.transform.position;


            // Return if attack doesnt connect

            // Calculate hit data based on attack & parameters
            Hit hit = new Hit()
            {
                AttackingEntity = attacker.gameObject,
                DamageAmount = 10,
                //Force = 1,
                DamageElement = DamageArea.DamageElement.Normal,
                DamageType = DamageArea.DamageType.Normal,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                // Sound
            };

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
        }

        private IEnumerator CauseDamageAfterDelay(OtherPenitent attacker, byte attack, float delay)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            ProcessHit(attack, attacker);
        }
    }
}
