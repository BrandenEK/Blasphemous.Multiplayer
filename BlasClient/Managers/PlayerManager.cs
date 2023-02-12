using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using Tools.Level;
using Tools.Level.Interactables;
using BlasClient.Structures;
using BlasClient.Data;

namespace BlasClient.Managers
{
    public class PlayerManager
    {
        private List<GameObject> players = new List<GameObject>();
        private List<Text> nametags = new List<Text>();

        private Transform canvas; // Optimize to not have to find these every scene change
        private GameObject textPrefab;
        private RuntimeAnimatorController playerController;

        // Queued player updates
        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, byte> queuedAnimations = new Dictionary<string, byte>();
        private Dictionary<string, bool> queuedDirections = new Dictionary<string, bool>();

        private static readonly object positionLock = new object();
        private static readonly object animationLock = new object();
        private static readonly object directionLock = new object(); // Might also need to lock players list when adding/removing

        public void loadScene(string scene)
        {
            // Remove all existing player objects and nametags
            destroyPlayers();
            
            // Create any players that are already in this scene
            foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
            {
                if (Main.Multiplayer.connectedPlayers[playerName].currentScene == scene)
                    addPlayer(playerName);
            }

            // Find textPrefab
            foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
            {
                if (obj.name == "PurgePoints") { textPrefab = obj.transform.GetChild(1).gameObject; break; }
            }
            // Find canvas parent
            foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            {
                if (c.name == "Game UI") { canvas = c.transform; break; }
            }
            // Find animator controller
            if (Core.Logic.Penitent != null) playerController = Core.Logic.Penitent.Animator.runtimeAnimatorController;

            // Add special animation checker to certain interactors
            foreach (Interactable interactable in Object.FindObjectsOfType<Interactable>())
            {
                System.Type type = interactable.GetType();
                if (type != typeof(PrieDieu) && type != typeof(CollectibleItem) && type != typeof(Chest) && type != typeof(Lever) && type != typeof(Door))
                    continue;

                foreach (Transform child in interactable.transform)
                {
                    if (child.name.ToLower().Contains("interactor"))
                    {
                        // Only add this to the interactor animator of certain interactables
                        child.gameObject.AddComponent<SpecialAnimationChecker>();
                        break;
                    }
                }
            }
            if (scene == "D17Z01S01")
            {
                // Add this to the fake penitent intro animator
                GameObject fakePenitent = GameObject.Find("FakePenitent");
                if (fakePenitent != null) fakePenitent.AddComponent<SpecialAnimationChecker>();
            }

            // Create main player's nametag
            createPlayerNameTag();
        }

        public void unloadScene()
        {
            
        }

        // Should be optimized to not use dictionaries
        public void updatePlayers()
        {
            // Update any player's new position
            lock (positionLock)
            {
                foreach (string name in queuedPositions.Keys)
                    updatePlayerPosition(name, queuedPositions[name]);
                queuedPositions.Clear();
            }
            // Update any player's new animation
            lock (animationLock)
            {
                foreach (string name in queuedAnimations.Keys)
                    updatePlayerAnimation(name, queuedAnimations[name]);
                queuedAnimations.Clear();
            }
            // Update any player's new direction
            lock (directionLock)
            {
                foreach (string name in queuedDirections.Keys)
                    updatePlayerDirection(name, queuedDirections[name]);
                queuedDirections.Clear();
            }

            // Check status of player skins and potentially update the textures
            foreach (string name in Main.Multiplayer.connectedPlayers.Keys)
            {
                SkinStatus playerSkin = Main.Multiplayer.connectedPlayers[name].skin;
                if (playerSkin.updateStatus == 2)
                {
                    // Set that one update cycle has passed
                    playerSkin.updateStatus = 1;
                }
                else if (playerSkin.updateStatus == 1)
                {
                    // Set the player texture
                    setSkinTexture(name, playerSkin.skinName);
                    playerSkin.updateStatus = 0;
                }
            }

            // Update position of all name tags
            for (int i = 0; i < nametags.Count; i++)
            {
                RectTransform nametag = nametags[i].transform as RectTransform;
                string name = nametags[i].name;

                // Get player with this name
                GameObject player = name == Main.Multiplayer.playerName ? Core.Logic.Penitent.gameObject : getPlayerObject(name);
                if (player != null)
                {
                    Vector3 viewPosition = Camera.main.WorldToViewportPoint(player.transform.position + Vector3.up * 3.1f);
                    nametag.anchorMin = viewPosition;
                    nametag.anchorMax = viewPosition;
                    nametag.anchoredPosition = Vector2.zero;
                }
            }
        }

        // When disconnected from server or loading new scene, remove all players
        public void destroyPlayers()
        {
            for (int i = 0; i < players.Count; i++)
                Object.Destroy(players[i]);
            players.Clear();
            for (int i = 0; i < nametags.Count; i++)
                Object.Destroy(nametags[i].gameObject);
            nametags.Clear();
        }

        // When a player enters a scene, create a new player object
        public void addPlayer(string name) // Maybe take in playerstatus instead
        {
            // Create base object
            GameObject player = new GameObject("_" + name, typeof(SpriteRenderer), typeof(Animator), typeof(EventReceiver));  // Change to create prefab at initialization, and instantiate a new instance
            players.Add(player);

            // Set up sprite rendering
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.material = Core.Logic.Penitent.SpriteRenderer.material;
            render.sortingLayerName = "Player";

            // Hide player object until skin texture is set - must be delayed
            Main.Multiplayer.getPlayerStatus(name).skin.updateStatus = 2;
            render.enabled = false;

            // Set up animations
            Animator anim = player.GetComponent<Animator>();
            anim.runtimeAnimatorController = playerController;

            // Set up name tag
            if (Main.Multiplayer.config.displayNametags)
                createNameTag(name);

            Main.UnityLog("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        public void removePlayer(string name)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                players.Remove(player);
                Object.Destroy(player);
                Main.UnityLog("Removed player object for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't remove player object for " + name);
            }
            Text nametag = getPlayerNametag(name);
            if (nametag != null)
            {
                nametags.Remove(nametag);
                Object.Destroy(nametag);
                Main.UnityLog("Removed nametag for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerPosition(string name, Vector2 position)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = position;
                //Main.UnityLog("Updating player object position for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't update object position for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerAnimation(string name, byte animation)
        {
            GameObject player = getPlayerObject(name);
            PlayerStatus playerStatus = Main.Multiplayer.getPlayerStatus(name);
            if (player != null)
            {
                Animator anim = player.GetComponent<Animator>();
                if (animation < 240)
                {
                    // Regular animation
                    if (playerStatus.specialAnimation > 0)
                    {
                        // Change back to regular animations
                        anim.runtimeAnimatorController = playerController;
                        playerStatus.specialAnimation = 0;
                    }
                    anim.SetBool("IS_CROUCH", false);
                    //anim.SetBool("IS_DEAD") might need one for vertical attack
                    // If anim is ladder climbing, set speed to 0

                    // Set required parameters to keep player onject in this animation
                    PlayerAnimState animState = AnimationStates.animations[animation];
                    for (int i = 0; i < animState.parameterNames.Length; i++)
                    {
                        anim.SetBool(animState.parameterNames[i], animState.parameterValues[i]);
                    }
                    anim.Play(animState.name);
                    //Main.UnityLog("Updating player object animation for " + name);
                }
                else
                {
                    // Special animation
                    if (playSpecialAnimation(anim, animation))
                    {
                        playerStatus.specialAnimation = animation;
                        Main.UnityLog("Playing special animation for " + name);
                    }
                    else
                        Main.UnityLog("Failed to play special animation for " + name);
                }
            }
            else
            {
                Main.UnityLog("Error: Can't update object animation for " + name);
            }
        }

        // When receiving a player direction update, find the player and change its direction
        private void updatePlayerDirection(string name, bool direction)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                SpriteRenderer render = player.GetComponent<SpriteRenderer>();
                render.flipX = direction;
                //Main.UnityLog("Updating player object direction for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't update object direction for " + name);
            }
        }

        // Instantiates a nametag object
        private void createNameTag(string name)
        {
            if (canvas == null || textPrefab == null)
            {
                Main.UnityLog("Error: Failed to create nametag for " + name);
                return;
            }

            Text nametag = Object.Instantiate(textPrefab, canvas).GetComponent<Text>();
            nametag.rectTransform.sizeDelta = new Vector2(100, 50);
            nametag.rectTransform.SetAsFirstSibling();
            nametag.name = name;
            nametag.text = name;
            nametag.alignment = TextAnchor.LowerCenter;
            nametags.Add(nametag);
        }

        // Creates a nametag specifically for the main player
        public void createPlayerNameTag()
        {
            if (Main.Multiplayer.config.displayOwnNametag)
                createNameTag(Main.Multiplayer.playerName);
        }

        // Sets the skin texture of a player's object - must be delayed until after object creation
        private void setSkinTexture(string name, string skin)
        {
            // Get player object with this name
            GameObject player = getPlayerObject(name);
            if (player == null)
            {
                Main.UnityLog("Error: Can't update object skin for " + name);
                return;
            }

            // Make player visible
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.enabled = true;

            // Get skin texture for this player
            Sprite palette = Core.ColorPaletteManager.GetColorPaletteById(skin);
            if (palette == null)
            {
                Main.UnityLog("Error: Couldn't find skin named " + skin);
                return;
            }

            Main.UnityLog("Setting skin texture for " + name);
            render.material.SetTexture("_PaletteTex", palette.texture);
        }

        // Gets the animator controller of an interactable object in the scene & plays special animation
        private bool playSpecialAnimation(Animator anim, byte type)
        {
            if (type == 240 || type == 241 || type == 242)
            {
                // Prie Dieu
                PrieDieu priedieu = Object.FindObjectOfType<PrieDieu>();
                if (priedieu == null)
                    return false;

                anim.runtimeAnimatorController = priedieu.transform.GetChild(4).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 240)
                {
                    anim.SetTrigger("ACTIVATION");
                }
                else if (type == 241)
                {
                    anim.SetTrigger("KNEE_START");
                }
                else
                {
                    anim.Play("Stand Up");
                }
            }
            else if (type == 243 || type == 244)
            {
                // Collectible item
                CollectibleItem item = Object.FindObjectOfType<CollectibleItem>();
                if (item == null)
                    return false;

                anim.runtimeAnimatorController = item.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController;
                anim.Play(type == 244 ? "Floor Collection" : "Halfheight Collection");
            }
            else if (type == 245)
            {
                // Chest
                Chest chest = Object.FindObjectOfType<Chest>();
                if (chest == null)
                    return false;

                anim.runtimeAnimatorController = chest.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                anim.SetTrigger("USED");
            }
            else if (type == 246)
            {
                // Lever
                Lever lever = Object.FindObjectOfType<Lever>();
                if (lever == null)
                    return false;

                anim.runtimeAnimatorController = lever.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                anim.SetTrigger("DOWN");
            }
            else if (type == 247 || type == 248 || type == 249)
            {
                // Door
                Door door = Object.FindObjectOfType<Door>();
                if (door == null)
                    return false;

                anim.runtimeAnimatorController = door.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 247)
                {
                    anim.SetTrigger("OPEN_ENTER");
                }
                else if (type == 248)
                {
                    anim.SetTrigger("CLOSED_ENTER");
                }
                else
                {
                    anim.SetTrigger("KEY_ENTER");
                }
            }
            else if (type == 250 || type == 251)
            {
                // Fake penitent
                GameObject logic = GameObject.Find("LOGIC");
                if (logic != null)
                {
                    anim.runtimeAnimatorController = logic.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                    anim.Play(type == 250 ? "FakePenitent laydown" : "FakePenitent gettingUp");
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        // Finishes playing a special animation and returns to idle
        public void finishSpecialAnimation(string playerName)
        {
            byte currentSpecialAnimation = Main.Multiplayer.getPlayerStatus(playerName).specialAnimation;
            if (currentSpecialAnimation >= 247 && currentSpecialAnimation <= 249)
            {
                // If finished entering door, disable renderer
                GameObject player = getPlayerObject(playerName);
                if (player != null)
                    player.GetComponent<SpriteRenderer>().enabled = false;
            }

            updatePlayerAnimation(playerName, 0);
        }

        // Finds a specified player in the scene
        private GameObject getPlayerObject(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == "_" + name)
                    return players[i];
            }
            return null;
        }

        // Find a specified player's nametag
        private Text getPlayerNametag(string name)
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i].name == name)
                    return nametags[i];
            }
            return null;
        }

        public void queuePosition(string playerName, Vector2 position)
        {
            lock (positionLock)
            {
                if (queuedPositions.ContainsKey(playerName))
                    queuedPositions[playerName] = position;
                else
                    queuedPositions.Add(playerName, position);
            }
        }

        public void queueAnimation(string playerName, byte animation)
        {
            lock (animationLock)
            {
                if (queuedAnimations.ContainsKey(playerName))
                    queuedAnimations[playerName] = animation;
                else
                    queuedAnimations.Add(playerName, animation);
            }
        }

        public void queueDirection(string playerName, bool direction)
        {
            lock (directionLock)
            {
                if (queuedDirections.ContainsKey(playerName))
                    queuedDirections[playerName] = direction;
                else
                    queuedDirections.Add(playerName, direction);
            }
        }
    }
}
