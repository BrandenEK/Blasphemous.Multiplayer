using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.PvP.Models;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using System.Linq;

namespace Blasphemous.Multiplayer.Client.PvP;

/// <summary>
/// Handles scaling pvp attack damage based on stats
/// </summary>
public class DamageCalculator
{
    private readonly OffenseItem[] _offenses;
    private readonly DefenseItem[] _defenses;

    /// <summary>
    /// Loads all offense and defense items
    /// </summary>
    public DamageCalculator()
    {
        Main.Multiplayer.FileHandler.LoadDataAsJson("offenses.json", out _offenses);
        Main.Multiplayer.FileHandler.LoadDataAsJson("defenses.json", out _defenses);
    }

    /// <summary>
    /// Scales the base damage of the attack based on offense stats
    /// </summary>
    public float CalculateOffense(AttackType attack)
    {
        AttackData data = Main.Multiplayer.AttackManager.GetAttackData(attack);

        return data.BaseDamage + Core.Logic.Penitent.Stats.Strength.GetUpgrades() + _offenses
            .Where(item => IsItemEquipped(item.Id) && IsConditionMet(item.Condition))
            .Select(item => GetIncreaseForType(item, data))
            .Sum();
    }

    /// <summary>
    /// Scales the full damage of the attack based on defense stats
    /// </summary>
    public float CalculateDefense(AttackType attack, byte damage)
    {
        AttackData data = Main.Multiplayer.AttackManager.GetAttackData(attack);

        return damage * _defenses
            .Where(item => IsItemEquipped(item.Id) && IsConditionMet(item.Condition))
            .Select(item => 1 - GetReductionForElement(item, data))
            .Aggregate(1f, (x, y) => x * y);
    }

    private bool IsItemEquipped(string item)
    {
        var obj = Core.InventoryManager.GetBaseObject(item, ItemModder.GetItemTypeFromId(item));
        return Core.InventoryManager.IsBaseObjectEquipped(obj);
    }

    private bool IsConditionMet(ConditionType condition)
    {
        return condition switch
        {
            ConditionType.None => true,
            ConditionType.LowHealth => Core.Logic.Penitent.Stats.Life.MissingRatio <= 0.2f,
            ConditionType.NoFlasks => Core.Logic.Penitent.Stats.Flask.Current == 0,
            _ => throw new System.Exception($"Invalid condition type: {condition}")
        };
    }

    private float GetIncreaseForType(OffenseItem item, AttackData attack)
    {
        return attack.ScalingType switch
        {
            ScalingType.Sword => item.SwordIncrease,
            ScalingType.Prayer => item.PrayerIncrease,
            ScalingType.Blood => item.BloodIncrease,
            _ => throw new System.Exception($"Invalid scaling type: {attack.ScalingType}")
        };
    }

    private float GetReductionForElement(DefenseItem item, AttackData attack)
    {
        return attack.DamageElement switch
        {
            DamageArea.DamageElement.Normal => item.PhysicalReduction,
            DamageArea.DamageElement.Fire => item.FireReduction,
            DamageArea.DamageElement.Toxic => item.ToxicReduction,
            DamageArea.DamageElement.Magic => item.MagicReduction,
            DamageArea.DamageElement.Lightning => item.LightningReduction,
            _ => throw new System.Exception($"Invalid element type: {attack.DamageElement}")
        };
    }
}
