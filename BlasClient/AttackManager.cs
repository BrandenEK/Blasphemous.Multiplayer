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
        private Dictionary<AttackType, PlayerAttack> allAttacks;

        public AttackManager()
        {
            allAttacks = new Dictionary<AttackType, PlayerAttack>();
            LoadAttacks();
        }

        public void AttackReceived(string attackerName, string receiverName, AttackType attack)
        {
            if (Core.Logic.Penitent == null || !allAttacks.ContainsKey(attack)) return;

            if (receiverName == Main.Multiplayer.playerName)
            {
                // This is the player that got hit
                OtherPenitent attacker = Main.Multiplayer.playerManager.FindPlayerObject(attackerName);
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
            OtherPenitent other = Main.Multiplayer.playerManager.FindPlayerObject(playerName);
            if (other == null) return;

            other.OtherPenitentAttack.PlayEffectAnimation(effect, other.IsFacingRight);
        }

        private void DamagePlayer(AttackType attack, GameObject attacker)
        {
            // Calculate hit data based on attack & parameters
            Hit hit = new Hit()
            {
                AttackingEntity = attacker,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
            };
            PlayerAttack currentAttack = allAttacks[attack];
            hit.DamageAmount = currentAttack.BaseDamage;
            hit.DamageType = currentAttack.GetDamageType();
            hit.DamageElement = currentAttack.GetDamageElement();
            hit.Force = currentAttack.Force;

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
            if (currentAttack.SoundId != null) // Can remove later
            Core.Audio.PlayOneShot("event:/SFX/Penitent/Damage/" + currentAttack.SoundId);
        }

        public void ShowDamageEffects(string receiverName)
        {
            OtherPenitent receiver = Main.Multiplayer.playerManager.FindPlayerObject(receiverName);
            if (receiver == null) return;

            Vector3 effectPosition = receiver.transform.position + Vector3.up;
            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        private void LoadAttacks()
        {
            if (!Main.Multiplayer.FileUtil.loadDataText("attackValues.json", out string text))
            {
                Main.Multiplayer.LogDisplay("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Main.Multiplayer.LogError("Failed to load attack data!");
                return;
            }
            
            PlayerAttack[] attacks = Main.Multiplayer.FileUtil.jsonObject<PlayerAttack[]>(text);
            foreach (PlayerAttack attack in attacks)
            {
                allAttacks.Add(attack.GetAttackType(), attack);
            }
            Main.Multiplayer.Log("Successfully loaded all attack data!");
        }
    }
}
