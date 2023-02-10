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
                    Main.Multiplayer.usingSpecialAnimation(240);
                    Main.UnityLog("Player is activating!"); return;
                case "Kneeing":
                    Main.Multiplayer.usingSpecialAnimation(241);
                    Main.UnityLog("Player is kneeling!"); return;
                case "Stand Up":
                    Main.Multiplayer.usingSpecialAnimation(242);
                    Main.UnityLog("Player is standing up!"); return;
                default:
                    Main.UnityLog("Unimportant anim"); return;
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
            "Turning ON"
        };
    }
}
