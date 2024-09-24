using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.ProgressSync;
using Gameplay.UI.Others.UIGameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Multiplayer.Client.Notifications
{
    public class NotificationManager
    {
        private const int MAX_LINES = 4;
        private const int LINE_HEIGHT = 20;

        // Created UI
        private RectTransform messageBox;
        private Text[] textLines;

        // Current message status
        private readonly List<NotificationLine> currentMessages = new ();

        // Add a new notification to the list
        public void DisplayNotification(string notification)
        {
            ModLog.Info("Notification: " + notification);

            // Add new line to list
            NotificationLine line = new NotificationLine(notification, Main.Multiplayer.config.notificationDisplaySeconds);
            currentMessages.Insert(0, line);

            // Remove first one if overfull
            if (currentMessages.Count > MAX_LINES)
                currentMessages.RemoveAt(currentMessages.Count - 1);
        }

        // Add a new notification for a progress update, but calculate it first
        public void DisplayProgressNotification(string playerName, ProgressUpdate progress)
        {
            string notification = Main.Multiplayer.ProgressManager.GetProgressNotification(progress);

            if (notification != null)
                DisplayNotification($"{playerName} {notification}");
        }

        // Update the order, text, and fade of all notification lines and box size
        public void Update()
        {
            if (messageBox == null)
                return;

            // Loop over each line of text
            float maxWidth = 0;
            float timeBeforeFade = Main.Multiplayer.config.notificationDisplaySeconds / 2;

            for (int i = 0; i < textLines.Length; i++)
            {
                // There aren't this many notifications
                if (i >= currentMessages.Count)
                {
                    textLines[i].text = string.Empty;
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
            messageBox.sizeDelta = new Vector2(maxWidth, currentMessages.Count * LINE_HEIGHT);
        }

        public void LevelLoaded()
        {
            if (messageBox == null)
                CreateUI();
        }

        // Create text background & lines
        private void CreateUI()
        {
            ModLog.Info("Creating new message box!");

            // Find canvas parent
            Transform parent = null;
            foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            {
                if (c.name == "Game UI")
                {
                    parent = c.transform;
                    break;
                }
            }
            if (parent == null)
                return;

            // Find text object
            GameObject textObject = null;
            foreach (PlayerPurgePoints purge in Object.FindObjectsOfType<PlayerPurgePoints>())
            {
                if (purge.name == "PurgePoints")
                {
                    textObject = purge.transform.GetChild(1).gameObject;
                    break;
                }
            }
            if (textObject == null)
                return;

            // Create message background
            GameObject obj = new GameObject("Message box", typeof(RectTransform), typeof(Image));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            SetOrientation(rect);
            rect.sizeDelta = new Vector2(0, 0);

            // Set image color
            Image image = obj.GetComponent<Image>();
            image.color = new Color(0.17f, 0.17f, 0.17f, 0.8f);

            // Create text lines
            textLines = new Text[MAX_LINES];
            for (int i = 0; i < MAX_LINES; i++)
            {
                Text line = Object.Instantiate(textObject, rect).GetComponent<Text>();
                SetOrientation(line.rectTransform);
                line.rectTransform.sizeDelta = new Vector2(100, 20);
                line.rectTransform.anchoredPosition = new Vector2(5, 2.5f + i * 20);
                line.raycastTarget = false;
                line.text = string.Empty;
                line.horizontalOverflow = HorizontalWrapMode.Overflow;
                line.alignment = TextAnchor.LowerLeft;
                line.color = Color.white;
                textLines[i] = line;
            }

            messageBox = rect;

            // Set a recttransform to bottom left corner
            void SetOrientation(RectTransform rect)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
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
}
