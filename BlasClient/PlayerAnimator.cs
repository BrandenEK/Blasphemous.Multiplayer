using UnityEngine;
using System.Collections.Generic;

namespace BlasClient
{
    public class PlayerAnimator : MonoBehaviour
    {
        void SetDashInvulnerable() { }
        void SetDashVulnerable() { }

        void RaiseStopDust() { }

        void PlayFootStep() { }
        void GetStepDust() { }

        public static PlayerAnimState[] animations = new PlayerAnimState[]
        {
            new PlayerAnimState("Idle", new string[] { "GROUNDED", "RUN_STEP", "IS_CLIMBING_LADDER", "IS_IDLE_MODE", "IS_DIALOGUE_MODE" }, new bool[] { true, false, false, false, false }),
            new PlayerAnimState("Run", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("RunStep", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("RunStart", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("RunStop", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, false })
            //new PlayerAnimState("", new string[] { }, new bool[] { }),
        };
    }

    public class PlayerAnimState
    {
        public string name;
        public string[] parameterNames;
        public bool[] parameterValues;

        public PlayerAnimState(string name, string[] parameterNames, bool[] parameterValues)
        {
            this.name = name;
            this.parameterNames = parameterNames;
            this.parameterValues = parameterValues;
        }
    }
}
