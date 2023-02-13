using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Enemies.Behaviours;

namespace BoschingMachine.Enemies
{

    public class Drone : Enemy
    {
        [SerializeField] PatrolBehaviour patrol;

        protected override void Update()
        {
            patrol.Update(this);

            base.Update();
        }
    }
}