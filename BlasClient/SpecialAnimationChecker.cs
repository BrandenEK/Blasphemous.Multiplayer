using UnityEngine;

namespace BlasClient
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
            Main.UnityLog("Unknown animation: " + transform.parent.name);
        }

        private void playSpecialAnimation(string anim)
        {
            switch (anim)
            {
                case "Turning ON":
                    Main.Multiplayer.usingSpecialAnimation(240); return;
                case "Kneeing":
                    Main.Multiplayer.usingSpecialAnimation(241); return;
                case "Stand Up":
                    Main.Multiplayer.usingSpecialAnimation(242); return;
                case "Halfheight Collection":
                    Main.Multiplayer.usingSpecialAnimation(243); return;
                case "Floor Collection":
                    Main.Multiplayer.usingSpecialAnimation(244); return;
                case "Opening":
                    Main.Multiplayer.usingSpecialAnimation(245); return;
                case "Lever Down":
                case "Lever Up":
                    Main.Multiplayer.usingSpecialAnimation(246); return;
                case "(Open) Entering":
                    Main.Multiplayer.usingSpecialAnimation(247); return;
                case "(Closed) Entering":
                    Main.Multiplayer.usingSpecialAnimation(248); return;
                case "(KEY) Entering":
                    Main.Multiplayer.usingSpecialAnimation(249); return;
                case "FakePenitent laydown":
                    Main.Multiplayer.usingSpecialAnimation(250); return;
                case "FakePenitent gettingUp":
                    Main.Multiplayer.usingSpecialAnimation(251); return;
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
