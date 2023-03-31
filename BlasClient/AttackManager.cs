using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using UnityEngine;
using BlasClient.MonoBehaviours;
using BlasClient.PvP;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        public void SwordAttack(byte type)
        {
            Main.Multiplayer.LogError("Attacking");
            Main.Multiplayer.SendNewAttack(type);
        }

        public void AttackReceived(string playerName, byte attack)
        {
            OtherPenitent other = Main.Multiplayer.playerManager.getPlayerObject(playerName);
            if (Core.Logic.Penitent == null || other == null) return;

            // If attack is 255, that means this is not an attack and simply an acknowledgment that the player 'playerName' was hit
            if (attack == 255)
            {
                AcknowledgeHit(other);
                return;
            }

            // If invalid attack type, return
            if (!attackTypes.ContainsKey(attack)) return;

            // Play attack animation based on the attack type
            other.OtherPenitentAttack.PlayAttackAnimation(attack, other.IsFacingRight);

            // Apply damage to player if the attack connects and they are on the other team
            if (Main.Multiplayer.playerTeam != Main.Multiplayer.playerList.getPlayerTeam(playerName))
                other.OtherPenitentAttack.StartAttack(attackTypes[attack], other.IsFacingRight);
        }

        private void AcknowledgeHit(OtherPenitent attackedPlayer)
        {
            Main.Multiplayer.LogWarning("Player " + attackedPlayer.name + " was hit!  Drawing blood effects!");
            Vector3 effectPosition = attackedPlayer.transform.position + Vector3.up;

            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        // Store all attack data (Delay, damage, hitbox) in separate classes
        private Dictionary<byte, PlayerAttack> attackTypes = new Dictionary<byte, PlayerAttack>()
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
