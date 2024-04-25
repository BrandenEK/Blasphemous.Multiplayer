using Blasphemous.Multiplayer.Client.PvP.Models;

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
    public byte CalculateOffense(AttackType attack)
    {
        return 0;
    }

    /// <summary>
    /// Scales the full damage of the attack based on defense stats
    /// </summary>
    public float CalculateDefense(AttackType attack, byte damage)
    {
        foreach (var item in _defenses)
        {
            Main.Multiplayer.LogWarning(item.Id + ": " + item.Condition);
        }
        return 0;
    }
}
