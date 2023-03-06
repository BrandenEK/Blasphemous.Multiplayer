using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitent : MonoBehaviour
    {
        private SpriteRenderer renderer;
        private Animator anim;

        // Adds necessary components & initializes them
        public void createPenitent(RuntimeAnimatorController animatorController, Material material)
        {
            // General

            // Rendering
            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.material = material;
            renderer.sortingLayerName = "Player";
            renderer.enabled = false;

            // Animation
            anim = gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = animatorController;
        }

        // If the death animation has ended, disable the animator
        private void Update()
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.95f && (state.IsName("Death") || state.IsName("Death Spike")))
            {
                anim.enabled = false;
            }
        }

        // Finish a special animation when the event is received
        public void LaunchEvent(string eventName)
        {
            if (eventName == "INTERACTION_END")
                Main.Multiplayer.finishedSpecialAnimation(gameObject.name.Substring(1));
        }
    }
}
