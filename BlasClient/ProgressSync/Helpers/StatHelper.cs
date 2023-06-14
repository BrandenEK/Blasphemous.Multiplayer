using Framework.FrameworkCore.Attributes;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Managers;
using HarmonyLib;

namespace BlasClient.ProgressSync.Helpers
{
    public class StatHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Attribute attribute;
            switch (progress.Id)
            {
                case "LIFE":        attribute = Core.Logic.Penitent.Stats.Life; break;
                case "FERVOUR":     attribute = Core.Logic.Penitent.Stats.Fervour; break;
                case "STRENGTH":    attribute = Core.Logic.Penitent.Stats.Strength; break;
                case "MEACULPA":    attribute = Core.Logic.Penitent.Stats.MeaCulpa; break;
                case "BEADSLOTS":   attribute = Core.Logic.Penitent.Stats.BeadSlots; break;
                case "FLASKHEALTH": attribute = Core.Logic.Penitent.Stats.FlaskHealth; break;
                case "FLASK":
                    {
                        // Flask is handled specially
                        Core.Logic.Penitent.Stats.Flask.SetPermanentBonus(progress.Value == 255 ? -1 : progress.Value);
                        return;
                    }
                default:
                    Main.Multiplayer.Log("Error: Unknown stat received - " + progress.Id);
                    return;
            }

            while (attribute.GetUpgrades() < progress.Value)
            {
                attribute.Upgrade();
            }
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            string stat;
            switch (progress.Id)
            {
                case "LIFE":        stat = "stlife"; break;
                case "FERVOUR":     stat = "stferv"; break;
                case "STRENGTH":    stat = "stmeas"; break;
                case "MEACULPA":    stat = "stmeat"; break;
                case "BEADSLOTS":   stat = "stbead"; break;
                case "FLASK":       stat = "stfknm"; break;
                case "FLASKHEALTH": stat = "stfkhl"; break;
                default:
                    Main.Multiplayer.Log("Error: Unknown stat received - " + progress.Id);
                    return null;
            }
            
            return $"{Main.Multiplayer.Localize("stnot")} {Main.Multiplayer.Localize(stat)}";
        }

        public void SendAllProgress()
        {
            byte life = (byte)Core.Logic.Penitent.Stats.Life.GetUpgrades();
            if (life > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("LIFE", ProgressType.PlayerStat, life);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }

            byte fervour = (byte)Core.Logic.Penitent.Stats.Fervour.GetUpgrades();
            if (fervour > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("FERVOUR", ProgressType.PlayerStat, fervour);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }

            byte strength = (byte)Core.Logic.Penitent.Stats.Strength.GetUpgrades();
            if (strength > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("STRENGTH", ProgressType.PlayerStat, strength);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }

            byte meaculpa = (byte)Core.Logic.Penitent.Stats.MeaCulpa.GetUpgrades();
            if (meaculpa > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("MEACULPA", ProgressType.PlayerStat, meaculpa);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }

            byte beadslots = (byte)Core.Logic.Penitent.Stats.BeadSlots.GetUpgrades();
            if (beadslots > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("BEADSLOTS", ProgressType.PlayerStat, beadslots);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }

            byte flaskhealth = (byte)Core.Logic.Penitent.Stats.FlaskHealth.GetUpgrades();
            byte flask = (byte)(Core.Logic.Penitent.Stats.Flask.GetUpgrades() + flaskhealth);
            if (flask > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("FLASK", ProgressType.PlayerStat, flask);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            if (flaskhealth > 0)
            {
                ProgressUpdate progress = new ProgressUpdate("FLASKHEALTH", ProgressType.PlayerStat, flaskhealth);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(Attribute), "Upgrade")]
    public class AttributeUpgrade_Patch
    {
        public static bool Prefix(Attribute __instance)
        {
            // If receiving upgrade, then just upgrade and skip send
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return true;

            // If shouldn't upgrade because received upgrade from same room, skip upgrade and skip send
            if (!Main.Multiplayer.CanObtainStatUpgrades)
                return false;

            // If you can upgrade and found this naturally, upgrade and send
            string type = null;
            if (__instance.GetType() == typeof(Life))               type = "LIFE";
            else if (__instance.GetType() == typeof(Fervour))       type = "FERVOUR";
            else if (__instance.GetType() == typeof(Strength))      type = "STRENGTH";
            else if (__instance.GetType() == typeof(MeaCulpa))      type = "MEACULPA";
            else if (__instance.GetType() == typeof(BeadSlots))     type = "BEADSLOTS";
            else if (__instance.GetType() == typeof(Flask))         type = "FLASK";
            else if (__instance.GetType() == typeof(FlaskHealth))   type = "FLASKHEALTH";
            
            int upgradeLevel = __instance.GetUpgrades() + 1; // This is before the upgrade so increase by one
            if (type == "FLASK")
                upgradeLevel += Core.Logic.Penitent.Stats.FlaskHealth.GetUpgrades();

            if (type != null && upgradeLevel > 0)
            {
                ProgressUpdate progress = new ProgressUpdate(type, ProgressType.PlayerStat, (byte)upgradeLevel);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            return true;
        }
    }
}
