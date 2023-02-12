using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Gameplay.UI.Others.UIGameLogic;
using Framework.Managers;
using Framework.Inventory;
using Framework.FrameworkCore;
using BlasClient.Data;

namespace BlasClient.Managers
{
    public class NotificationManager
    {
        private RectTransform messageBox;
        private Text[] textLines;

        private int maxLines = 4;
        private int lineHeight = 20;

        private List<NotificationLine> currentMessages = new List<NotificationLine>();
        private static readonly object notificationLock = new object();

        // Add a new notification to the list
        public void showNotification(string notification)
        {
            lock (notificationLock)
            {
                Main.UnityLog("Notification: " + notification);

                // Add new line to list
                NotificationLine line = new NotificationLine(notification, Main.Multiplayer.config.notificationDisplaySeconds);
                currentMessages.Insert(0, line);

                // Remove first one if overfull
                if (currentMessages.Count > maxLines)
                    currentMessages.RemoveAt(currentMessages.Count - 1);
            }
        }

        // Add a new notification for a progress update, but calculates it first
        public void showProgressNotification(string playerName, byte progressType, string progressId, byte progressValue)
        {
            string notification = null;
            switch ((ProgressManager.ProgressType)progressType)
            {
                case ProgressManager.ProgressType.Bead:
                    RosaryBead bead = Core.InventoryManager.GetRosaryBead(progressId);
                    if (bead != null && progressValue == 0)
                        notification = "has obtained the " + bead.caption;
                    break;
                case ProgressManager.ProgressType.Prayer:
                    Prayer prayer = Core.InventoryManager.GetPrayer(progressId);
                    if (prayer != null && progressValue == 0)
                        notification = "has obtained the " + prayer.caption;
                    break;
                case ProgressManager.ProgressType.Relic:
                    Relic relic = Core.InventoryManager.GetRelic(progressId);
                    if (relic != null && progressValue == 0)
                        notification = "has obtained the " + relic.caption;
                    break;
                case ProgressManager.ProgressType.Heart:
                    Sword sword = Core.InventoryManager.GetSword(progressId);
                    if (sword != null && progressValue == 0)
                        notification = "has obtained the " + sword.caption;
                    break;
                case ProgressManager.ProgressType.Collectible:
                    Framework.Inventory.CollectibleItem collectible = Core.InventoryManager.GetCollectibleItem(progressId);
                    if (collectible != null && progressValue == 0)
                        notification = "has obtained the " + collectible.caption;
                    break;
                case ProgressManager.ProgressType.QuestItem:
                    QuestItem quest = Core.InventoryManager.GetQuestItem(progressId);
                    if (quest != null && progressValue == 0)
                        notification = "has obtained the " + quest.caption;
                    break;
                case ProgressManager.ProgressType.PlayerStat:
                    string stat = null;
                    if (progressId == "LIFE") stat = "maximum health";
                    else if (progressId == "FERVOUR") stat = "maximum fervour";
                    else if (progressId == "STRENGTH") stat = "mea culpa strength";
                    else if (progressId == "MEACULPA") stat = "mea culpa tier";
                    else if (progressId == "BEADSLOTS") stat = "maximum bead slots";
                    else if (progressId == "FLASK") stat = "maximum flasks";
                    else if (progressId == "FLASKHEALTH") stat = "flasks strength";
                    if (stat != null)
                        notification = "has upgraded the " + stat;
                    break;
                case ProgressManager.ProgressType.SwordSkill:
                    UnlockableSkill skill = Core.SkillManager.GetSkill(progressId);
                    if (skill != null)
                        notification = "has unlocked the " + skill.caption;
                    break;
                case ProgressManager.ProgressType.Flag:
                    FlagState flag = FlagStates.getFlagState(progressId);
                    if (flag != null)
                        notification = flag.notification;
                    break;

                // Unlocked teleports
                // Church donations
            }

            if (notification != null)
                showNotification(playerName + " " + notification);
        }

        // Update the order, text, and fade of all notification lines and box size
        public void updateNotifications()
        {
            if (messageBox == null) return;

            lock (notificationLock)
            {
                // Loop over each line of text
                float maxWidth = 0;
                float timeBeforeFade = Main.Multiplayer.config.notificationDisplaySeconds / 2;

                for (int i = 0; i < textLines.Length; i++)
                {
                    // There aren't this many notifications
                    if (i >= currentMessages.Count)
                    {
                        textLines[i].text = "";
                        continue;
                    }

                    // Update text
                    NotificationLine currentLine = currentMessages[i];
                    textLines[i].text = currentLine.text;
                    if (textLines[i].preferredWidth > maxWidth)
                        maxWidth = textLines[i].preferredWidth;

                    // Decrease the amount of time left on this notification line
                    currentLine.timeLeft -= Time.unscaledDeltaTime;
                    if (currentLine.timeLeft <= 0)
                    {
                        // Time is over, remove this message
                        currentMessages.RemoveAt(i);
                    }
                    else if (currentLine.timeLeft <= timeBeforeFade)
                    {
                        // Enough time has passed, fade this text away
                        textLines[i].color = new Color(1, 1, 1, currentLine.timeLeft / timeBeforeFade);
                    }
                    else
                    {
                        // This text line is pretty new, keep at full opacity
                        textLines[i].color = Color.white;
                    }
                }

                // Set size of message box based on notifications
                if (maxWidth > 0)
                    maxWidth += 10;
                messageBox.sizeDelta = new Vector2(maxWidth, currentMessages.Count * lineHeight);
            }
        }

        // Create text background & lines
        public void createMessageBox()
        {
            if (messageBox != null) return;
            Main.UnityLog("Creating new message box!");

            // Find canvas parent
            Transform parent = null;
            foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            {
                if (c.name == "Game UI") { parent = c.transform; break; }
            }
            if (parent == null) return;

            // Find text object
            GameObject textObject = null;
            foreach (PlayerPurgePoints purge in Object.FindObjectsOfType<PlayerPurgePoints>())
            {
                if (purge.name == "PurgePoints") { textObject = purge.transform.GetChild(1).gameObject; break; }
            }
            if (textObject == null) return;

            // Create message background
            GameObject obj = new GameObject("Message box", typeof(RectTransform), typeof(Image));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            setOrientation(rect);
            rect.sizeDelta = new Vector2(0, 0);

            // Set image color
            Image image = obj.GetComponent<Image>();
            image.color = new Color(0.17f, 0.17f, 0.17f, 0.8f);

            // Create text lines
            textLines = new Text[maxLines];
            for (int i = 0; i < maxLines; i++)
            {
                Text line = Object.Instantiate(textObject, rect).GetComponent<Text>();
                setOrientation(line.rectTransform);
                line.rectTransform.sizeDelta = new Vector2(100, 20);
                line.rectTransform.anchoredPosition = new Vector2(5, 2.5f + i * 20);
                line.raycastTarget = false;
                line.text = "";
                line.horizontalOverflow = HorizontalWrapMode.Overflow;
                line.alignment = TextAnchor.LowerLeft;
                line.color = Color.white;
                textLines[i] = line;
            }

            messageBox = rect;

            // Set a recttransform to bottom left corner
            void setOrientation(RectTransform rect)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    class NotificationLine
    {
        public string text;
        public float timeLeft;

        public NotificationLine(string text, float timeLeft)
        {
            this.text = text;
            this.timeLeft = timeLeft;
        }
    }
}
