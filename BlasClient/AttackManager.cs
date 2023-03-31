using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        private PenitentAttack attacker;

        private void OnPlayerAttack(PenitentAttack attacker)
        {
            //Main.Multiplayer.LogError("Attacking");
            //Main.Multiplayer.SendNewAttack(0);
        }

        public void sceneLoaded()
        {
            attacker = Core.Logic.Penitent.EntityAttack as PenitentAttack;
            attacker.OnAttackTriggered += OnPlayerAttack;
        }
        public void sceneUnloaded()
        {
            if (attacker != null)
                attacker.OnAttackTriggered -= OnPlayerAttack;
        }

        public void SwordAttack(byte type)
        {
            Main.Multiplayer.LogError("Attacking");
            Main.Multiplayer.SendNewAttack(0);
        }

        public void TakeHit(byte attack)
        {
            Penitent penitent = Core.Logic.Penitent;
            if (penitent == null) return;

            Hit hit = new Hit()
            {
                AttackingEntity = penitent.gameObject,
                DamageAmount = 20,
                //Force = 1,
                DamageElement = DamageArea.DamageElement.Normal,
                DamageType = DamageArea.DamageType.Normal,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                // Sound
            };

            penitent.Damage(hit);
        }
    }
}
