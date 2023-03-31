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

        public void TakeHit(string playerName, byte attack)
        {
            OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(playerName);
            if (Core.Logic.Penitent == null || other == null) return;

            Hit hit = new Hit()
            {
                AttackingEntity = other.gameObject,
                DamageAmount = 10,
                //Force = 1,
                DamageElement = DamageArea.DamageElement.Normal,
                DamageType = DamageArea.DamageType.Normal,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                // Sound
            };

            other.PlayAttackAnimation(attack);
            Main.Instance.StartCoroutine(CauseDamageAfterDelay(hit, 0.1f));
        }

        private IEnumerator CauseDamageAfterDelay(Hit hit, float delay)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            Core.Logic.Penitent.Damage(hit);
        }
    }
}
