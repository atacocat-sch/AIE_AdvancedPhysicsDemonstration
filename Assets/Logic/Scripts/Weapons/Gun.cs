using UnityEngine;
using BoschingMachine.Vitality;
using System.Collections.Generic;

namespace BoschingMachine
{
    [System.Serializable]
    public class Gun
    {
        [SerializeField] Transform muzzle;
        [SerializeField] float damage;
        [SerializeField] float firerate;
        [SerializeField] float spray;
        [SerializeField] GameObject hitPrefab;

        [Space]
        [SerializeField] List<ParticleSystem> fx;

        float lastFireTime;

        public void Shoot (GameObject shooter)
        {
            if (Time.time > lastFireTime + 60.0f / firerate)
            {
                Ray ray = new Ray(muzzle.position, Quaternion.Euler(Random.insideUnitSphere * spray) * muzzle.forward);
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.transform.TryGetComponent(out Health health))
                    {
                        health.Damage(new Health.DamageArgs(shooter, damage));
                    }

                    Object.Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                }

                fx.ForEach(e => e.Play());

                lastFireTime = Time.time;
            }
        }
    }
}
