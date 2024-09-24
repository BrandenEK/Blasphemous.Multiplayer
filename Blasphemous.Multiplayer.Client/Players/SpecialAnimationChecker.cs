using Blasphemous.ModdingAPI;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class SpecialAnimationChecker : MonoBehaviour
    {
        private string lastAnimation;
        private Animator anim;

        void Start()
        {
            lastAnimation = string.Empty;
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(lastAnimation))
                return;

            // Animator state has changed since last frame
            for (int i = 0; i < specialAnimations.Length; i++)
            {
                if (state.IsName(specialAnimations[i]))
                {
                    playSpecialAnimation(specialAnimations[i]);
                    lastAnimation = specialAnimations[i];
                    return;
                }
            }

            // This animator is playing an unknown animation state
            ModLog.Info("Unknown animation: " + transform.parent.name);
        }

        private void playSpecialAnimation(string anim)
        {
            switch (anim)
            {
                case "Turning ON":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(240); return;
                case "Kneeing":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(241); return;
                case "Stand Up":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(242); return;
                case "Halfheight Collection":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(243); return;
                case "Floor Collection":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(244); return;
                case "Opening":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(245); return;
                case "Lever Down":
                case "Lever Up":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(246); return;
                case "(Open) Entering":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(247); return;
                case "(Closed) Entering":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(248); return;
                case "(KEY) Entering":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(249); return;
                case "FakePenitent laydown":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(250); return;
                case "FakePenitent gettingUp":
                    Main.Multiplayer.MainPlayerManager.UseSpecialAnimation(251); return;
                    // Perpetva chest
                    // Pick gemino flower
                    // Place altasgracias item
                    // Altar
                    // Teleporter
            }
        }

        private static string[] specialAnimations = new string[]
        {
            "Waiting", // Prie Dieu
            "Kneeing",
            "Knee (No Aura)",
            "Knee (Aura ON)",
            "Knee (Aura OFF)",
            "Stand Up",
            "Turning ON",
            "Idle", // Collectible item
            "Halfheight Collection",
            "Floor Collection",
            "Unused", // Chest
            "Opening",
            "Switch", // Lever
            "Lever Down",
            "Lever Up",
            "(Open) Entering", // Door
            "(Closed) Entering",
            "(KEY) Entering",
            "FakePenitent laydown", // Fake penitent
            "FakePenitent gettingUp",
            "FakePenitent end"
        };
    }
}
