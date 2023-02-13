using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Enemies;

namespace BoschingMachine.Enemies.Behaviours
{
    [System.Serializable]
    public class PatrolBehaviour 
    {
        [SerializeField] Transform path;
        [SerializeField] float softDistance;
        [SerializeField] float waitTime;

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

            Debug.Log(moveDir);

            enemy.SetMoveDirection(moveDir);
            enemy.SetLookDirection(enemy.Rigidbody.velocity);
        }
    }
}
