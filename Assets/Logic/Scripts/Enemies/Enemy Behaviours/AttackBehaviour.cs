using BoschingMachine.Enemies;
using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class AttackBehaviour
    {
        [SerializeField] float holdDistance;
        [SerializeField] float reEvalTime;
        [SerializeField] float reEvalDistance;
        [SerializeField] float angleVariance;
        [SerializeField] Gun gun;
        [SerializeField][Range(0.0f, 1.0f)] float fireSplit;

        float timer;
        float angle;

        public AttackBehaviour Start (Enemy enemy, GameObject target)
        {
            GetNewAngle(enemy, target);
            return this;
        }

        private void GetNewAngle(Enemy enemy, GameObject target)
        {
            Vector3 direction = enemy.transform.position - target.transform.position;
            angle = Mathf.Atan2(direction.x, direction.z) + Random.Range(-angleVariance, angleVariance) * Mathf.Deg2Rad;
            timer %= reEvalTime;
        }

        public void Update (Enemy enemy, GameObject target)
        {
            if (!target) return;

            Vector3 dir = target.transform.position - enemy.transform.position;
            float distance = dir.magnitude;
            dir /= distance;
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * holdDistance;
            
            if (distance > reEvalDistance)
            {
                GetNewAngle(enemy, target);
            }
            if (timer > reEvalTime)
            {
                GetNewAngle(enemy, target);
            }

            enemy.SetLookDirection(target.transform.position - enemy.Head.position, 1.0f);
            enemy.MoveToPoint(target.transform.position + offset);

            if (timer / reEvalTime < fireSplit)
            {
                gun.Shoot(enemy.gameObject);
            }

            timer += Time.deltaTime;
        }
    }
}
