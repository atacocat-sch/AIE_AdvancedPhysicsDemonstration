using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoschingMachine.Elevators
{
    public class ElevatorExtraSwitch : Interactable
    {
        [SerializeField] SwitchType type;

        Elevator elevator;

        public static readonly Dictionary<SwitchType, string> Labels = new()
        {
            { SwitchType.CloseDoors, "Close Doors" },
            { SwitchType.OpenDoors, "Open Doors" },
            { SwitchType.CallHelp, "Call For Help" },
        };

        private void Awake()
        {
            elevator = GetComponentInParent<Elevator>();
        }

#if !UNITY_EDITOR
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
#endif

        protected override void FinishInteract(Biped biped)
        {
            switch (type)
            {
                case SwitchType.CloseDoors:
                    elevator.CloseDoors();
                    break;
                case SwitchType.OpenDoors:
                    elevator.OpenDoors();
                    break;
                case SwitchType.CallHelp:
                    System.Diagnostics.Process.Start("https://www.wikihow.com/Survive-Being-Stuck-in-a-Lift");
#if !UNITY_EDITOR
                    ShowWindow(GetActiveWindow(), 2);
#endif
                    break;
            }
        }

        public override string BuildInteractString(string passthrough = "")
        {
            return Labels[type];
        }

        public enum SwitchType
        {
            CloseDoors,
            OpenDoors,
            CallHelp,
        }
    }
}
