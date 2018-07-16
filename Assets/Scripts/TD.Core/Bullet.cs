using System.Collections.Generic;
using UnityEngine;

namespace TD.Core
{
    /// <summary>
    /// A class the handles the control of all bullet types
    /// Currently supports standard, splash, and chain bullets
    /// </summary>
    public class Bullet : MonoBehaviour
    {

        public float speed = 15; //movement speed of the bullet
        public float rotSpeed = 5; //sppeed of the bullet rotating
        public Transform target; //the target to travel towards
        public float damage = 1f; //damage done on hit
        public float radius = 0; //if > 0, creates fall off splash damage from initial attack point
        public float chainAttackRadius = 0; //the radius to check for enemies to chain attacks to
        public float chainAttackReductionPercent = (1f / 3f); //the amount the damage is reduced to for each chained attack
        public int chainAttack_MaxBounces = 2; //the max amount of times to allow damage to bounce from the first target hit
        public GameObject impactEffect; //the particle prefab to spawn when the bullet hits its target

        public GameObject bulletPrefab; //the bullet prefab to pass on for chain attack spawns
        
        void Start()
        {
            //alert that the tower may be setup as a chain and splash tower
            if (chainAttackRadius > 0 && radius > 0)
            {
                Debug.LogError(this.name + "has a nonzero radius and chainAttackRadius\nIf radius is nonzero, tower defaults to splash tower");
            }

            chainAttackReductionPercent = Mathf.Clamp01(chainAttackReductionPercent);
        }
        
        void Update()
        {
            if (target == null)
            {
                //enemy is dead/deleted
                Destroy(gameObject);
                return;
            }

            //get the direction to the target
            Vector3 dir = target.position - this.transform.localPosition;

            //figure out how much to move this frame
            float distThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distThisFrame)
            {
                //we've reached the target
                DoBulletHit();
            }
            else
            {
                //move towards node
                transform.Translate(dir.normalized * distThisFrame, Space.World);
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
            }
        }

        /// <summary>
        /// Handle the bullet reaching its target based on its stats.
        /// If radius is greater than 0, consider the bullet to be doing splash damage
        /// If radius == 0 and chainAttackRadius less than or equal to 0 , consider the bullet to do regular damage
        /// If chainAttackRadius is greater than 0, chain more attacks up to chainAttack_MaxBounces (defaults to 2)
        /// </summary>
        void DoBulletHit()
        {
            //destroy first to prevent multiple calls
            Destroy(gameObject);

            GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectInstance, 1.9f);
            //regular or chain attack
            if (radius == 0)
            {
                //do regular damage
                target.GetComponent<Enemy>().takeDamage(damage);

                //do chain attack
                if (chainAttackRadius > 0)
                {
                    //Debug.Log("Starting chain attacks");
                    Collider[] cols = Physics.OverlapSphere(this.transform.position, chainAttackRadius);
                    Enemy closest = null;
                    List<Enemy> enemiesHit = new List<Enemy>();
                    enemiesHit.Add(target.GetComponent<Enemy>());
                    for (int i = 0; i < chainAttack_MaxBounces; i++)
                    {
                        float distance = chainAttackRadius;
                        //find closest
                        foreach (Collider c in cols)
                        {
                            Enemy e = c.GetComponent<Enemy>();
                            if (e != null)
                            {
                                //prevent hitting the same object twice
                                if (enemiesHit.Contains(e))
                                {
                                    continue;
                                }
                                float dist = Mathf.Infinity;

                                //if its the first iteration, find the closest to the origin
                                //if its not, find the closest to the last hit target
                                if (i == 0)
                                {
                                    if (target != null && c != null)
                                    {
                                        dist = Vector3.Distance(target.transform.position, c.transform.position);
                                    }
                                }
                                else
                                {
                                    if (closest != null && c != null)
                                    {
                                        dist = Vector3.Distance(closest.transform.position, c.transform.position);
                                    }
                                }

                                if (dist < distance)
                                {
                                    closest = e;
                                }
                            }
                        }

                        //if we found an enemy to chain to damage them based on which bounce number it is, reduced by our reduction percent
                        if (closest != null)
                        {
                            //Debug.Log(closest.name + "taking damage (" + (damage * (chainAttackReductionPercent / (i + 1))) + ")");
                            //closest.takeDamage(damage * (chainAttackReductionPercent / (i + 1)));
                            ChainShootAt(closest, (damage * (chainAttackReductionPercent / (i + 1))));
                            enemiesHit.Add(closest);
                        }
                    }
                }
            }
            //do splash attack
            else
            {
                Collider[] cols = Physics.OverlapSphere(this.transform.position, radius);
                foreach (Collider c in cols)
                {
                    Enemy e = c.GetComponent<Enemy>();
                    if (e != null)
                    {
                        float dist = Vector3.Distance(e.transform.position, this.transform.position);
                        e.GetComponent<Enemy>().takeDamage(damage / dist);
                    }

                }
            }
        }

        /// <summary>
        /// A function to allow chain type bullets to spread to/shoot at their next target
        /// </summary>
        /// <param name="e">The enemy to chain to</param>
        /// <param name="damage">The reduced damage to deal to the next bounce</param>
        void ChainShootAt(Enemy e, float damage)
        {
            //todo spawn at tip
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, transform.position, transform.rotation);
            Bullet b = bulletGO.GetComponent<Bullet>();
            b.damage = damage;
            b.radius = radius;
            b.target = e.transform;
            b.chainAttackRadius = 0;
            b.chainAttackReductionPercent = chainAttackReductionPercent;
            b.chainAttack_MaxBounces = chainAttack_MaxBounces;
        }

        private void OnDrawGizmos()
        {
            if (radius > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(this.transform.position, radius);
            }

            if (chainAttackRadius > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(this.transform.position, chainAttackRadius);
            }
        }
    }
}
