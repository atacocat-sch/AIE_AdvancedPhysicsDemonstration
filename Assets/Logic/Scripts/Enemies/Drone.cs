using UnityEngine;
using BoschingMachine.Enemies.Behaviours;
using BoschingMachine.Player;

namespace BoschingMachine.Enemies
{
    public class Drone : Enemy
    {
        [SerializeField] PatrolBehaviour patrol;
        [SerializeField] InvestigateBehaviour investigate;
        [SerializeField] AttackBehaviour attack;
        [SerializeField] EnemyVision vision;
        [SerializeField] float timerSpeed;
        [SerializeField] string state;

        float seenTimer;

        PlayerBiped player;
        bool canSeePlayer;

        Interest interest;

        protected override void FixedUpdate()
        {
            vision.FixedUpdate(Head);

            canSeePlayer = vision.CanSeePlayer(Head, ref player);
            if (!canSeePlayer) vision.CanSeeInterest(Head, ref interest);

            base.FixedUpdate();
        }

        protected override void Update()
        {
            bool seen = false;
            if (canSeePlayer)
            {
                seen = true;
                seenTimer += Time.deltaTime * timerSpeed;
            }
            else
            {
                seenTimer -= Time.deltaTime * timerSpeed;
            }

            state = "Invalid";
            if (seenTimer > 1.0f)
            {
                attack.Update(this, player?.gameObject);
                state = "Attacking";
            }
            else if (seenTimer > 0.01f)
            {
                investigate.Update(this, player?.gameObject, seen);
                state = "Investigating (Player)";
            }
            else if (interest)
            {
                investigate.Update(this, interest.gameObject, true);
                state = "Investigating (Interest)";
            }
            else
            {
                patrol.Update(this);
                state = "Patroling";
            }

            seenTimer = Mathf.Clamp(seenTimer, 0.0f, 2.0f);

            base.Update();
        }

        private void OnDrawGizmosSelected()
        {
            vision.Gizmos(Head);
        }
    }
}