using UnityEngine;

namespace BoschingMachine.Enemies.Behaviours
{
    [System.Serializable]
    public class InvestigateBehaviour
    {
        [SerializeField] float waitTime;
        [SerializeField] float holdDistance;

        float timer;
        Vector3 tPos;

        public InvestigateBehaviour Reset ()
        {
            timer = 0.0f;
            return this;
        }

        public void Update (Enemy enemy, GameObject source, bool canSenseSource)
        {
            if (canSenseSource) tPos = source.transform.position;

            Vector3 lookVec = tPos - enemy.Head.position;
            enemy.SetLookDirection(lookVec, 1.0f);

            if (timer < waitTime)
            {
                timer += Time.deltaTime;
                enemy.SetMoveDirection(Vector3.zero);
                return;
            }

            enemy.MoveToPoint(tPos, holdDistance);
        }
    }
}
