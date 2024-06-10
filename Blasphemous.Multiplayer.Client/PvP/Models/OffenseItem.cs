
namespace Blasphemous.Multiplayer.Client.PvP.Models;

public class OffenseItem(string id, ConditionType condition, float swordIncrease, float prayerIncrease, float bloodIncrease)
{
    public string Id { get; } = id;
    public ConditionType Condition { get; } = condition;

    public float SwordIncrease { get; } = swordIncrease;
    public float PrayerIncrease { get; } = prayerIncrease;
    public float BloodIncrease { get; } = bloodIncrease;
}
