using UnityEngine;

namespace BlasClient.MonoBehaviours
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
            Main.Multiplayer.Log("Unknown animation: " + transform.parent.name);
        }

        private void playSpecialAnimation(string anim)
        {
            switch (anim)
            {
                case "Turning ON":
                    Main.Multiplayer.UseSpecialAnimation(240); return;
                case "Kneeing":
                    Main.Multiplayer.UseSpecialAnimation(241); return;
                case "Stand Up":
                    Main.Multiplayer.UseSpecialAnimation(242); return;
                case "Halfheight Collection":
                    Main.Multiplayer.UseSpecialAnimation(243); return;
                case "Floor Collection":
                    Main.Multiplayer.UseSpecialAnimation(244); return;
                case "Opening":
                    Main.Multiplayer.UseSpecialAnimation(245); return;
                case "Lever Down":
                case "Lever Up":
                    Main.Multiplayer.UseSpecialAnimation(246); return;
                case "(Open) Entering":
                    Main.Multiplayer.UseSpecialAnimation(247); return;
                case "(Closed) Entering":
                    Main.Multiplayer.UseSpecialAnimation(248); return;
                case "(KEY) Entering":
                    Main.Multiplayer.UseSpecialAnimation(249); return;
                case "FakePenitent laydown":
                    Main.Multiplayer.UseSpecialAnimation(250); return;
                case "FakePenitent gettingUp":
                    Main.Multiplayer.UseSpecialAnimation(251); return;
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
