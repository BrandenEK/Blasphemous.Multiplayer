using UnityEngine;
using BlasClient.Data;
using Tools.Level.Interactables;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitent : MonoBehaviour
    {
        private string penitentName;

        private SpriteRenderer renderer;
        private Animator CharacterAnim { get; set; }
        private Animator SwordAnim { get; set; }
        private RuntimeAnimatorController penitentAnimatorController;

        // Adds necessary components & initializes them
        public void createPenitent(string name, RuntimeAnimatorController animatorController, RuntimeAnimatorController swordAnimatorController, Material material)
        {
            penitentName = name;
            Main.Multiplayer.playerList.setPlayerSkinUpdateStatus(name, 2);

            // Rendering
            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.material = material;
            renderer.sortingLayerName = "Player";
            renderer.enabled = false;

            // Animation
            CharacterAnim = gameObject.AddComponent<Animator>();
            CharacterAnim.runtimeAnimatorController = animatorController;
            penitentAnimatorController = animatorController;

            // Sword handler
            GameObject sword = new GameObject("Sword");
            sword.transform.SetParent(transform);
            sword.transform.localPosition = Vector3.zero;
            sword.AddComponent<SpriteRenderer>();
            SwordAnim = sword.AddComponent<Animator>();
            SwordAnim.runtimeAnimatorController = swordAnimatorController;
        }

        public void updatePosition(Vector2 positon)
        {
            transform.position = positon;
        }

        public void updateAnimation(byte animation)
        {
            if (animation < 240)
            {
                // Regular animation
                if (Main.Multiplayer.playerList.getPlayerSpecialAnimation(penitentName) > 0)
                {
                    // Change back to regular animations
                    CharacterAnim.runtimeAnimatorController = penitentAnimatorController;
                    Main.Multiplayer.playerList.setPlayerSpecialAnimation(penitentName, 0);
                }
                CharacterAnim.SetBool("IS_CROUCH", false);

                // Logic for ladder climbing

                // Set required parameters to keep player object in this animation
                PlayerAnimState animState = AnimationStates.animations[animation];
                for (int i = 0; i < animState.parameterNames.Length; i++)
                {
                    CharacterAnim.SetBool(animState.parameterNames[i], animState.parameterValues[i]);
                }
                CharacterAnim.Play(animState.name);
            }
            else
            {
                // Special animation
                if (playSpecialAnimation(animation))
                {
                    Main.Multiplayer.playerList.setPlayerSpecialAnimation(penitentName, animation);
                    Main.Multiplayer.Log("Playing special animation for " + name);
                }
                else
                    Main.Multiplayer.LogWarning("Failed to play special animation for " + name);
            }
        }

        public void updateDirection(bool facingDirection)
        {
            renderer.flipX = facingDirection;
        }

        public void updateSkin(Texture2D skin)
        {
            renderer.enabled = true;
            renderer.material.SetTexture("_PaletteTex", skin);
        }

        // Gets the animator controller of an interactable object in the scene & plays special animation
        private bool playSpecialAnimation(byte type)
        {
            if (type == 240 || type == 241 || type == 242)
            {
                // Prie Dieu
                PrieDieu priedieu = FindObjectOfType<PrieDieu>();
                if (priedieu == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = priedieu.transform.GetChild(4).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 240)
                {
                    CharacterAnim.SetTrigger("ACTIVATION");
                }
                else if (type == 241)
                {
                    CharacterAnim.SetTrigger("KNEE_START");
                }
                else
                {
                    CharacterAnim.Play("Stand Up");
                }
            }
            else if (type == 243 || type == 244)
            {
                // Collectible item
                CollectibleItem item = FindObjectOfType<CollectibleItem>();
                if (item == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = item.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.Play(type == 244 ? "Floor Collection" : "Halfheight Collection");
            }
            else if (type == 245)
            {
                // Chest
                Chest chest = FindObjectOfType<Chest>();
                if (chest == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = chest.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.SetTrigger("USED");
            }
            else if (type == 246)
            {
                // Lever
                Lever lever = FindObjectOfType<Lever>();
                if (lever == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = lever.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.SetTrigger("DOWN");
            }
            else if (type == 247 || type == 248 || type == 249)
            {
                // Door
                Door door = FindObjectOfType<Door>();
                if (door == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = door.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 247)
                {
                    CharacterAnim.SetTrigger("OPEN_ENTER");
                }
                else if (type == 248)
                {
                    CharacterAnim.SetTrigger("CLOSED_ENTER");
                }
                else
                {
                    CharacterAnim.SetTrigger("KEY_ENTER");
                }
            }
            else if (type == 250 || type == 251)
            {
                // Fake penitent
                GameObject logic = GameObject.Find("LOGIC");
                if (logic == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = logic.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.Play(type == 250 ? "FakePenitent laydown" : "FakePenitent gettingUp");
            }
            else
            {
                return false;
            }

            return true;
        }

        // Finishes playing a special animation and returns to idle
        public void finishSpecialAnimation()
        {
            byte currentSpecialAnimation = Main.Multiplayer.playerList.getPlayerSpecialAnimation(penitentName);
            if (currentSpecialAnimation >= 247 && currentSpecialAnimation <= 249)
            {
                // If finished entering door, disable renderer
                renderer.enabled = false;
            }

            updateAnimation(0);
        }

        // If the death animation has ended, disable the animator
        private void Update()
        {
            AnimatorStateInfo state = CharacterAnim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.95f && (state.IsName("Death") || state.IsName("Death Spike") || state.IsName("Death Fall")))
            {
                CharacterAnim.enabled = false;
            }
        }

        // Finish a special animation when the event is received
        public void LaunchEvent(string eventName)
        {
            if (eventName == "INTERACTION_END")
                finishSpecialAnimation();
        }

        // When receiving an attack from another player, make their character play the sword/prayer animation
        public void PlayAttackAnimation(byte attack)
        {
            SwordAnim.Play("Basic1_Lv1");
        }
    }
}
