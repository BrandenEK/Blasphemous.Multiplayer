using BlasClient.Players;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace BlasClient.PvP
{
    public class AttackManager
    {
        private readonly Dictionary<AttackType, PlayerAttack> allAttacks;

        public AttackManager()
        {
            allAttacks = new Dictionary<AttackType, PlayerAttack>();
            LoadAttacks();
        }

        public void ReceiveAttack(string attackerName, string receiverName, AttackType attack, byte damageAmount)
        {
            if (Core.Logic.Penitent == null || !allAttacks.ContainsKey(attack)) return;

            if (receiverName == Main.Multiplayer.PlayerName)
            {
                // This is the player that got hit
                OtherPlayerScript attacker = Main.Multiplayer.OtherPlayerManager.FindActivePlayer(attackerName);
                if (attacker == null) return;

                Config config = Main.Multiplayer.config;
                byte attackerTeam = Main.Multiplayer.OtherPlayerManager.FindConnectedPlayer(attackerName).Team;
                if (!config.enablePvP || (!config.enableFriendlyFire && Main.Multiplayer.PlayerTeam == attackerTeam))
                    return;

                Main.Multiplayer.LogWarning($"Receiving hit {attack} from {attackerName}");
                DamagePlayer(attack, damageAmount, attacker.gameObject);
            }
            else
            {
                // It was a different player that got hit
                ShowDamageEffects(receiverName);
            }
        }

        public void ReceiveEffect(string playerName, EffectType effect)
        {
            OtherPlayerScript other = Main.Multiplayer.OtherPlayerManager.FindActivePlayer(playerName);
            if (other == null) return;

            other.OtherPlayerAttack.PlayEffectAnimation(effect, other.IsFacingRight);
        }

        private void DamagePlayer(AttackType attack, byte damageAmount, GameObject attacker)
        {
            // Calculate hit data based on attack & parameters
            PlayerAttack currentAttack = GetAttackData(attack);
            Hit hit = new()
            {
                AttackingEntity = attacker,
                DamageAmount = damageAmount,
                DamageElement = currentAttack.GetDamageElement(),
                DamageType = currentAttack.GetDamageType(),
                Force = currentAttack.Force,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
            };

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
            if (currentAttack.SoundId != null) // Can remove later
                Core.Audio.PlayOneShot("event:/SFX/Penitent/Damage/" + currentAttack.SoundId);
        }

        public void ShowDamageEffects(string receiverName)
        {
            OtherPlayerScript receiver = Main.Multiplayer.OtherPlayerManager.FindActivePlayer(receiverName);
            if (receiver == null) return;

            Vector3 effectPosition = receiver.transform.position + Vector3.up;
            Core.Logic.Penitent.GetComponentInChildren<SwordSparkSpawner>().GetSwordSpark(effectPosition);
            GameObject blood = Core.Logic.Penitent.GetComponentInChildren<BloodSpawner>().GetBloodFX(BloodSpawner.BLOOD_FX_TYPES.SMALL);
            blood.transform.position = effectPosition;
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
        }

        public PlayerAttack GetAttackData(AttackType type)
        {
            if (allAttacks.TryGetValue(type, out PlayerAttack attack))
            {
                return attack;
            }

            throw new System.Exception($"Attack type \"{type}\" doesn't exist!");
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
