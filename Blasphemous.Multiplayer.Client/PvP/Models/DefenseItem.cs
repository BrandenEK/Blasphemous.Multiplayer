
namespace Blasphemous.Multiplayer.Client.PvP.Models;

public class DefenseItem
{
    public string Id { get; }
    public ConditionType Condition { get; }

    public float PhysicalReduction { get; }
    public float FireReduction { get; }
    public float ToxicReduction { get; }
    public float MagicReduction { get; }
    public float LightningReduction { get; }
}
