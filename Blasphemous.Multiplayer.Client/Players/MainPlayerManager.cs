using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.Data;
using Framework.Managers;
using UnityEngine;


namespace Blasphemous.Multiplayer.Client.Players
{
    public class MainPlayerManager
    {
        private Vector2 lastPosition;
        private byte lastAnimation;
        private bool lastDirection;

        private readonly float positionCutoff = 0.03f;
        private readonly float totalTimeBeforeSendAnimation = 0.5f;
        private float currentTimeBeforeSendAnimation = 0;

        public void Update()
        {
            if (Main.Multiplayer.NetworkManager.IsConnected && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                Vector2 newPosition = CurrentPosition;
                if (Mathf.Abs(newPosition.x - lastPosition.x) > positionCutoff || Mathf.Abs(newPosition.y - lastPosition.y) > positionCutoff)
                {
                    Main.Multiplayer.NetworkManager.SendPosition(newPosition);
                    lastPosition = newPosition;
                }

                // Check & send updated animation clip
                byte newAnimation = CurrentAnimation;
                if (newAnimation != lastAnimation)
                {
                    // Don't send new animations right after a special animation
                    if (currentTimeBeforeSendAnimation <= 0 && newAnimation != 255)
                    {
                        Main.Multiplayer.NetworkManager.SendAnimation(newAnimation);
                    }
                    lastAnimation = newAnimation;
                }

                // Check & send updated facing direction
                bool newDirection = CurrentDirection;
                if (newDirection != lastDirection)
                {
                    Main.Multiplayer.NetworkManager.SendDirection(newDirection);
                    lastDirection = newDirection;
                }
            }

            // Decrease frame counter for special animation delay
            if (currentTimeBeforeSendAnimation > 0)
                currentTimeBeforeSendAnimation -= Time.deltaTime;
        }

        // Sends the current position/animation/direction when first entering a scene or joining server
        public void SendAllLocationData()
        {
            Main.Multiplayer.NetworkManager.SendPosition(lastPosition = CurrentPosition);
            Main.Multiplayer.NetworkManager.SendAnimation(lastAnimation = 0);
            Main.Multiplayer.NetworkManager.SendDirection(lastDirection = CurrentDirection);
        }

        public void UseSpecialAnimation(byte animation)
        {
            currentTimeBeforeSendAnimation = totalTimeBeforeSendAnimation;
            Main.Multiplayer.NetworkManager.SendAnimation(animation);
        }

        private Vector2 CurrentPosition
        {
            get
            {
                return Core.Logic.Penitent.transform.position;
            }
        }

        private byte CurrentAnimation
        {
            get
            {
                AnimatorStateInfo state = Core.Logic.Penitent.Animator.GetCurrentAnimatorStateInfo(0);
                for (byte i = 0; i < AnimationStates.animations.Length; i++)
                {
                    if (state.IsName(AnimationStates.animations[i].name))
                    {
                        return i;
                    }
                }

                // This animation could not be found
                ModLog.Error("Current animation doesn't exist!");
                return 255;
            }
        }

        private bool CurrentDirection
        {
            get
            {
                return Core.Logic.Penitent.SpriteRenderer.flipX;
            }
        }
    }
}
