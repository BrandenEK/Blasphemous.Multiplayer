
namespace Blasphemous.Multiplayer.Client.PvP.Models;

public class DefenseItem(string id, ConditionType condition, float physicalReduction, float fireReduction, float toxicReduction, float magicReduction, float lightningReduction)
{
    public string Id { get; } = id;
    public ConditionType Condition { get; } = condition;

    public float PhysicalReduction { get; } = physicalReduction;
    public float FireReduction { get; } = fireReduction;
    public float ToxicReduction { get; } = toxicReduction;
    public float MagicReduction { get; } = magicReduction;
    public float LightningReduction { get; } = lightningReduction;
}
