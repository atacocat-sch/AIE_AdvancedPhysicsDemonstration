using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Player;

namespace BoschingMachine
{
    public class EnemyEyes
    {
        [SerializeField] float range;
        [SerializeField] float angle;
        [SerializeField] int xItterations;
        [SerializeField] int yItterations;
        [SerializeField] float cycleSpeed;

        public bool CanSeePlayer (Transform head, out PlayerBiped player)
        {
            foreach (var ray in GetRays(head))
            {
                    if (Physics.Raycast(ray, out var hit, range))
                    {
                        if (hit.transform.TryGetComponent(out player))
                        {
                            return true;
                        }
                    }
            }

            player = null;
            return false;
        }

        public List<Ray> GetRays (Transform head)
        {
            List<Ray> rays = new List<Ray>();

            for (int x = 0; x < xItterations; x++)
            {
                for (int y = 0; y < yItterations; y++)
                {
                    float xAngle = x / (float)xItterations * angle;
                    float yAngle = y / (float)yItterations * Mathf.PI;
                    Quaternion rotation = head.rotation * Quaternion.Euler(xAngle, 0.0f, yAngle);

                    Ray ray = new Ray(head.position, rotation * Vector3.forward);
                    rays.Add(ray);
                }
            }

            return rays;
        }
    }
}
