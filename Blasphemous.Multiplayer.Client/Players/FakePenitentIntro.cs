using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class FakePenitentIntro : MonoBehaviour
    {
        private Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void LateUpdate()
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("FakePenitent end"))
            {
                // Once the player reaches this animation, finish it and return to idle
                gameObject.GetComponent<OtherPlayerScript>().FinishSpecialAnimation();
            }
        }
    }
}
