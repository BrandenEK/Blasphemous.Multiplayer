using Blasphemous.Multiplayer.Client.Players;
using Blasphemous.Multiplayer.Client.PvP.Models;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.PvP
{
    public class AttackManager
    {
        private readonly Dictionary<AttackType, AttackData> allAttacks;

        public AttackManager()
        {
            allAttacks = new Dictionary<AttackType, AttackData>();
            LoadAttacks();
        }

        public void ReceiveAttack(string attackerName, string receiverName, AttackType attack, byte damageAmount)
        {
            if (Core.Logic.Penitent == null || !allAttacks.ContainsKey(attack)) return;

            if (receiverName == Main.Multiplayer.PlayerName) // This is the player that got hit
            {
                // Find the attacker in the scene
                OtherPlayerScript attacker = Main.Multiplayer.OtherPlayerManager.FindActivePlayer(attackerName);
                if (attacker == null)
                    return;

                // Ensure your input is not blocked
                if (Core.Input.HasBlocker("ANY"))
                    return;

                // Ensure pvp between you is allowed
                Config config = Main.Multiplayer.config;
                byte attackerTeam = Main.Multiplayer.OtherPlayerManager.FindConnectedPlayer(attackerName).Team;
                if (!config.enablePvP || (!config.enableFriendlyFire && Main.Multiplayer.PlayerTeam == attackerTeam))
                    return;

                // Perform the damage
                Main.Multiplayer.LogWarning($"Receiving hit {attack} from {attackerName}");
                DamagePlayer(attack, damageAmount, attacker.gameObject);
            }
            else // It was a different player that got hit
            {
                // Simply display an effect
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
            // Calculate final damage amount based on resistances
            float finalDamage = Main.Multiplayer.DamageCalculator.CalculateDefense(attack, damageAmount);

            // Calculate hit data based on attack & parameters
            AttackData currentAttack = GetAttackData(attack);
            Hit hit = new()
            {
                AttackingEntity = attacker,
                DamageAmount = finalDamage,
                DamageElement = currentAttack.DamageElement,
                DamageType = currentAttack.DamageType,
                Force = currentAttack.Force,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                HitSoundId = $"PVP{currentAttack.SoundId}"
            };

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
        }

        /// <summary>
        /// Damages the player with an attack without validation
        /// </summary>
        public void DamagePlayer_Internal(AttackType type, byte amount)
        {
            DamagePlayer(type, amount, Core.Logic.Penitent.gameObject);
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

        public AttackData GetAttackData(AttackType type)
        {
            if (allAttacks.TryGetValue(type, out AttackData attack))
            {
                return attack;
            }

            throw new System.Exception($"Attack type \"{type}\" doesn't exist!");
        }

        private void LoadAttacks()
        {
            if (!Main.Multiplayer.FileHandler.LoadDataAsJson("attacks.json", out AttackData[] attacks))
            {
                Main.Multiplayer.LogError("Failed to load attack data!");
                return;
            }
            
            foreach (AttackData attack in attacks)
            {
                allAttacks.Add(attack.Name, attack);
            }
            Main.Multiplayer.Log("Successfully loaded all attack data!");
        }
    }
}
