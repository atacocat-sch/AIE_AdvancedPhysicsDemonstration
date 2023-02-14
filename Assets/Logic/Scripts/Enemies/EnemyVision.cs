using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Player;

namespace BoschingMachine
{
    [System.Serializable]
    public class EnemyVision
    {
        [SerializeField] float range;
        [SerializeField] float angle;
        [SerializeField] int xItterations;
        [SerializeField] int yItterations;
        [SerializeField] float cycleSpeed;
        [SerializeField] float visionRadius;

        [Space]
        [SerializeField] EnemySightVisuals visuals;

        public void FixedUpdate (Transform head)
        {
            List<Vector3> starts = new List<Vector3>();
            List<Vector3> ends = new List<Vector3>();

            foreach (var ray in GetRays(head))
            {
                starts.Add(ray.origin);
                if (Physics.SphereCast(ray, visionRadius, out var hit, range))
                {
                    ends.Add(hit.point);
                }
                else
                {
                    ends.Add(ray.GetPoint(range));
                }
            }

            visuals.Update(starts, ends);
        }

        public bool CanSeePlayer (Transform head, ref PlayerBiped player) => CanSeeGeneric(head, ref player);
        public bool CanSeeInterest(Transform head, ref Interest interest) => CanSeeGeneric(head, ref interest);
        private bool CanSeeGeneric <T> (Transform head, ref T val) where T : class
        {
            foreach (var ray in GetRays(head))
            {
                if (Physics.SphereCast(ray, visionRadius, out var hit, range))
                {
                    if (hit.transform.TryGetComponent(out T result))
                    {
                        val = result;
                        return true;
                    }
                }
            }

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
                    float yAngle = y / (float)yItterations * 360.0f + (Time.time * cycleSpeed * (x % 2 == 0 ? 1 : -1));
                    Quaternion rotation = head.rotation * Quaternion.Euler(0, 0, yAngle) * Quaternion.Euler(xAngle, 0, 0);

                    Ray ray = new Ray(head.position, rotation * Vector3.forward);
                    rays.Add(ray);
                }
            }

            return rays;
        }

        public void Gizmos(Transform head)
        {
            foreach (var ray in GetRays(head))
            {
                Debug.DrawRay(ray.origin, ray.direction * range);
            }
        }
    }

    [System.Serializable]
    public class EnemySightVisuals
    {
        [SerializeField] LineRenderer lines;

        public void Update(List<Vector3> starts, List<Vector3> ends)
        {
            lines.positionCount = starts.Count * 3;
            lines.useWorldSpace = false;

            for (int i = 0; i < starts.Count; i++)
            {
                Vector3 start = lines.transform.InverseTransformPoint(starts[i]);
                Vector3 end = lines.transform.InverseTransformPoint(ends[i]);

                lines.SetPosition(3 * i, start);
                lines.SetPosition(3 * i + 1, end);
                lines.SetPosition(3 * i + 2, start);
            }
        }
    }
}
