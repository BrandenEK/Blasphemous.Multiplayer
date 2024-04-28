using Blasphemous.Multiplayer.Client.PvP;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client;

public class EffectManager
{
    private readonly Dictionary<int, GameObject> _activeEffects = new();

    public void OnReceiveEffect()
    {

    }

    private void Test()
    {
        //GameObject ranged = Core.Logic.Penitent.RangeAttack.RangeAttackProjectile;
        //GameObject gem = Object.FindObjectOfType<CloisteredGemProjectileAttack>().projectilePrefab;
        //LogWarning(gem.name);

        //Animator anim = gem.GetComponentInChildren<Animator>();
        //foreach (var clip in anim.runtimeAnimatorController.animationClips)
        //{
        //    LogError($"{clip.name} has a length of {clip.length}");
        //}

        //GameObject effect = new("Test effect");
        //effect.transform.position = Core.Logic.Penitent.transform.position;

        //SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        //sr.sortingLayerName = "Player";

        //Animator a = effect.AddComponent<Animator>();
        //a.runtimeAnimatorController = gem.GetComponentInChildren<Animator>().runtimeAnimatorController;

        //Animation a = effect.AddComponent<Animation>();
        //anim.runtimeAnimatorController.animationClips[0].legacy = true;
        //a.AddClip(anim.runtimeAnimatorController.animationClips[0], "RANGED");
        //a.Play("RANGED");
    }
}

public class EffectPacket
{
    public int Id { get; set; }
    public EffectType Type { get; set; }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }

    public bool Enable { get; set; }
    public bool Disable { get; set; }
}
