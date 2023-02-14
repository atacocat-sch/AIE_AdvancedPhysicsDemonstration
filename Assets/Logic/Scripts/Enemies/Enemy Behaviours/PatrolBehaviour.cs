using UnityEngine;

namespace BoschingMachine.Enemies.Behaviours
{
    [System.Serializable]
    public class PatrolBehaviour 
    {
        [SerializeField] Transform path;
        [SerializeField] float softDistance;
        [SerializeField] float waitTime;
        [SerializeField] float lookTilt;
        [SerializeField][Range(0.0f, 1.0f)] float speedScale;

        int pathIndex;
        float waitTimer;

        public void Update (Enemy enemy)
        {
            Vector3 moveDir;

            if (path.childCount == 0) return;
            if (path.childCount == 1) 
            {
                moveDir = path.GetChild(0).position - enemy.transform.position;
            }
            else 
            {
                Vector3 vec = path.GetChild(pathIndex).position - enemy.transform.position;
                vec.y = 0.0f;
                if (vec.sqrMagnitude < softDistance * softDistance)
                {
                    if (waitTimer < waitTime)
                    {
                        enemy.SetMoveDirection(Vector3.zero);
                        waitTimer += Time.deltaTime;
                        return;
                    }

                    waitTimer = 0.0f;
                    pathIndex = (pathIndex + 1) % path.childCount;
                }
                moveDir = vec;
            }

            moveDir.y = 0.0f;
            moveDir = Vector3.ClampMagnitude(moveDir, speedScale);

            enemy.SetMoveDirection(moveDir);

            float a = lookTilt * Mathf.Deg2Rad;
            enemy.SetLookDirection(enemy.Rigidbody.velocity * Mathf.Cos(a) + Vector3.up * Mathf.Sin(a), 0.1f);
        }
    }
}
