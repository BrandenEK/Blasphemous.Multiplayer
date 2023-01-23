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
        private bool lastDirection;

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
                    Main.UnityLog("Sending new player position");
                    client.sendPlayerPostition(penitentTransform.position.x, penitentTransform.position.y);
                    lastPosition = penitentTransform.position;
                }

                // Check & send updated animation clip
                Animator penitentAnimator = Core.Logic.Penitent.Animator;
                AnimatorStateInfo state = penitentAnimator.GetCurrentAnimatorStateInfo(0);
                if (animationHasChanged(state))
                {
                    // Animation has been updated
                    //Main.UnityLog("Sending new player animation"); // bring back once all animations are added
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

                // Check & send updated facing direction
                SpriteRenderer penitentRenderer = Core.Logic.Penitent.SpriteRenderer;
                if (directionHasChanged(penitentRenderer.flipX))
                {
                    Main.UnityLog("Sending new player direction");
                    client.sendPlayerDirection(penitentRenderer.flipX);
                    lastDirection = penitentRenderer.flipX;
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

        private bool animationHasChanged(AnimatorStateInfo currentState)
        {
            return !currentState.IsName(PlayerAnimator.animations[lastAnimation].name);
        }

        private bool directionHasChanged(bool currentDirection)
        {
            return currentDirection != lastDirection;
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos)
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

        // Received direction data from server
        public void playerDirectionUpdated(string playerName, bool direction)
        {
            if (inLevel)
                playerControl.queueDirection(playerName, direction);
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
