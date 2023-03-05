using UnityEngine;

namespace BlasClient.MonoBehaviours
{
    public class EventReceiver : MonoBehaviour
    {
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.95f && (state.IsName("Death") || state.IsName("Death Spike")))
            {
                anim.enabled = false;
            }
        }

        public void LaunchEvent(string eventName)
        {
            if (eventName == "INTERACTION_END")
                Main.Multiplayer.finishedSpecialAnimation(gameObject.name.Substring(1));
        }
    }
}
