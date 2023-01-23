using UnityEngine;
using Framework.Managers;
using Framework.FrameworkCore;

namespace BlasClient
{
    public class Multiplayer
    {
        private Client client;
        private PlayerControl playerControl;
        private string playerName;

        private bool inLevel;
        private Vector2 lastPosition;
        private byte lastAnimation;

        private bool shouldSendData
        {
            get { return inLevel && client != null && client.connected; }
        }

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            LevelManager.OnBeforeLevelLoad += onLevelUnloaded;
            playerControl = new PlayerControl();
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
            LevelManager.OnBeforeLevelLoad -= onLevelUnloaded;
        }

        public string tryConnect(string ip, string name, string password)
        {
            playerName = name;
            client = new Client(ip);
            bool result = client.Connect();

            return result ? "Successfully connected to " + ip : "Failed to connect to " + ip;
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            inLevel = newLevel.LevelName != "MainMenu";
            playerControl.loadScene(newLevel.LevelName);   

            if (shouldSendData)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
            }
        }

        private void onLevelUnloaded(Level oldLevel, Level newLevel)
        {
            if (shouldSendData)
            {
                // Left a scene
                Main.UnityLog("Leaving old scene");
                client.sendPlayerLeaveScene();
            }

            inLevel = false;
            playerControl.unloadScene();
        }

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                
            }

            if (shouldSendData && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                Transform penitentTransform = Core.Logic.Penitent.transform;
                if (positionHasChanged(penitentTransform.position))
                {
                    // Position has been updated
                    Main.UnityLog("Sending new player position");
                    bool dir = !Core.Logic.Penitent.SpriteRenderer.flipX;
                    client.sendPlayerPostition(penitentTransform.position.x, penitentTransform.position.y, dir);
                    lastPosition = penitentTransform.position;
                }

                // Check & send updated animation clip
                Animator penitentAnimator = Core.Logic.Penitent.Animator;
                AnimatorStateInfo state = penitentAnimator.GetCurrentAnimatorStateInfo(0);
                if (animationHasChanged(state))
                {
                    // Animation has been updated
                    //Main.UnityLog("Sending new player animation");
                    for (byte i = 0; i < PlayerAnimator.animations.Length; i++)
                    {
                        if (state.IsName(PlayerAnimator.animations[i].name))
                        {
                            Main.UnityLog("New anim: " + PlayerAnimator.animations[i].name);
                            client.sendPlayerAnimation(i);
                            lastAnimation = i;
                        }
                    }
                    // Check if animation wasn't found
                }
            }

            // Update other player's data
            playerControl.updatePlayers();
        }

        private bool positionHasChanged(Vector2 currentPosition)
        {
            float cutoff = 0.03f;
            return Mathf.Abs(currentPosition.x - lastPosition.x) > cutoff || Mathf.Abs(currentPosition.y - lastPosition.y) > cutoff;
        }

        private bool animationHasChanged(AnimatorStateInfo state)
        {
            return !state.IsName(PlayerAnimator.animations[lastAnimation].name);
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos, bool facingDirection)
        {
            if (inLevel)
                playerControl.queuePosition(playerName, new Vector2(xPos, yPos));
        }

        // Received animation data from server
        public void playerAnimationUpdated(string playerName, byte animation)
        {
            if (inLevel)
                playerControl.queueAnimation(playerName, animation);
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName)
        {
            if (inLevel)
                playerControl.addPlayer(playerName);
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel)
                playerControl.removePlayer(playerName);
        }
    }
}
