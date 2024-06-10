
namespace Blasphemous.Multiplayer.Client.PvP.Models;

public enum ConditionType
{
    None = 0,
    LowHealth = 1,
    NoFlasks = 2,
}

public enum ScalingType
{
    Sword = 0,
    Prayer = 1,
    Blood = 2,
}

public enum AttackType
{
    Slash = 0,

    ComboNormal = 10,
    ComboUp = 11,
    ComboDown = 12,

    Charged = 20,
    Lunge = 21,
    Vertical = 22,
    Ranged = 23,
    ChargedProjectile = 24,
    RangedExplosion = 25,

    Gem = 30,
    PrayerHit = 31,

    Debla = 40,
    Taranto = 50,
    Verdiales = 60,
    Lorquiana = 70,
    Tirana = 80,
    PoisonMist = 90,
    Shield = 100,
    Miriam = 110,
    MiriamSpike = 111,
    Aubade = 120,
    Cherubs = 130,
    CanteJondo = 140,

    NoDamage = 255
}

public enum EffectType
{
    SidewaysGrounded = 0,
    UpwardsGrounded = 1,
    SidewaysAir = 10,
    UpwardsAir = 11,
    Crouch = 20,
    Ranged = 30,

    Debla = 40,
    Verdiales = 60,
}
