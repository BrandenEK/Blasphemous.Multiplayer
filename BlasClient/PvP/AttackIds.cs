
namespace BlasClient.PvP
{
    public enum AttackType
    {
        Slash = 0,
        // Combos
        
        Charged = 10,
        Lunge = 11,
        Vertical = 12,
        Ranged = 20,

        Debla = 40,
        Taranto = 41,
        Lorquiana = 42,
    }

    public enum EffectType
    {
        SidewaysGrounded= 0,
        UpwardsGrounded = 1,
        SidewaysAir = 10,
        UpwardsAir = 11,
        Crouch = 20,
        Ranged = 30,
    }
}
