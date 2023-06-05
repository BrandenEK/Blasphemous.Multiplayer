using BlasClient.ProgressSync;
using BlasClient.PvP;
using UnityEngine;

namespace BlasClient.Network
{
    public class NetworkManager
    {
        // For now the receive functions are public, make them private once client is implemented

        // Position

        public void SendPosition(Vector2 position)
        {

        }

        public void ReceivePosition(byte[] data)
        {
            string playerName = string.Empty;
            Vector2 position = new Vector2();

            Main.Multiplayer.PlayerManager.ReceivePosition(playerName, position);
        }
        
        // Animation

        public void SendAnimation(byte animation)
        {

        }

        public void ReceiveAnimation(byte[] data)
        {
            string playerName = string.Empty;
            byte animation = 0;

            Main.Multiplayer.PlayerManager.ReceiveAnimation(playerName, animation);
        }

        // Direction

        public void SendDirection(bool direction)
        {

        }

        public void ReceiveDirection(byte[] data)
        {
            string playerName = string.Empty;
            bool directon = false;

            Main.Multiplayer.PlayerManager.ReceiveDirection(playerName, directon);
        }

        // Skin

        public void SendSkin(string skinName)
        {

        }

        public void ReceiveSkin(byte[] data)
        {
            string playerName = string.Empty;
            byte[] skinData = new byte[0];

            Main.Multiplayer.UpdateSkinData(playerName, skinData);
        }

        public void SendProgress(ProgressUpdate progress)
        {

        }

        public void ReceiveProgress(byte[] data)
        {
            string playerName = string.Empty;
            string progressId = string.Empty;
            ProgressType progressType = ProgressType.Bead;
            byte progressValue = 0;

            ProgressUpdate progress = new ProgressUpdate(progressId, progressType, progressValue);
            Main.Multiplayer.ProgressManager.ReceiveProgress(progress);
            Main.Multiplayer.ProcessRecievedStat(progress);

            if (playerName != "*")
                Main.Multiplayer.NotificationManager.DisplayProgressNotification(playerName, progress);

        }

        public void SendAttack(string hitPlayerName, AttackType attackType)
        {

        }

        public void ReceiveAttack(byte[] data)
        {
            string attackerName = string.Empty;
            string receiverName = string.Empty;
            AttackType attackType = AttackType.Aubade;

            Main.Multiplayer.AttackManager.ReceiveAttack(attackerName, receiverName, attackType);
        }

        public void SendEffect(EffectType effectType)
        {

        }

        public void ReceiveEffect(byte[] data)
        {
            string playerName = string.Empty;
            EffectType effectType = EffectType.Crouch;

            Main.Multiplayer.AttackManager.ReceiveEffect(playerName, effectType);
        }
    }
}
