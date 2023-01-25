using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.UI.Others.UIGameLogic;

namespace BlasClient
{
    public class NotificationManager
    {
        private RectTransform messageBox;
        private Text[] lines;

        private Vector2 boxSize = new Vector2(220, 100);
        private int maxLines = 5;
        private float timeDisplayed = 4f;
        private float timeBeforeFade = 3f;

        private List<NotificationLine> currentMessages = new List<NotificationLine>();
        private static readonly object notificationLock = new object();

        // Add a new notification to the list
        public void showNotification(string notification)
        {
            lock (notificationLock)
            {
                // Add new line to list
                NotificationLine line = new NotificationLine(notification, timeDisplayed);
                currentMessages.Add(line);

                // Remove first one if overfull
                if (currentMessages.Count > maxLines)
                    currentMessages.RemoveAt(0);
            }
        }

        // Update the order, text, and fade of all notification lines and box size
        public void updateNotifications()
        {
            lock (notificationLock)
            {

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
            rect.sizeDelta = boxSize;

            // Set image color
            Image image = obj.GetComponent<Image>();
            image.color = new Color(0.24f, 0.24f, 0.24f, 0.8f);

            // Create text lines
            lines = new Text[maxLines];
            for (int i = 0; i < maxLines; i++)
            {
                Text line = Object.Instantiate(textObject, rect).GetComponent<Text>();
                setOrientation(line.rectTransform);
                line.rectTransform.sizeDelta = new Vector2(100, 20);
                line.rectTransform.anchoredPosition = new Vector2(5, 3 + i * 20);
                line.raycastTarget = false;
                line.text = "This is a test line of text";
                line.horizontalOverflow = HorizontalWrapMode.Overflow;
                line.alignment = TextAnchor.LowerLeft;
                line.color = Color.white;
                lines[i] = line;
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

    public class NotificationLine
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
