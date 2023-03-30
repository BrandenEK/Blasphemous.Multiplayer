using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;

namespace BlasClient.Managers
{
    public class AttackManager
    {
        private PenitentAttack attacker;

        private void OnPlayerAttack(PenitentAttack attacker)
        {
            Main.Multiplayer.LogError("Attacking");
            Main.Multiplayer.SendNewAttack(0);
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
    }
}
