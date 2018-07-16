using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Core
{
    [System.Serializable]
    /// <summary>
    /// A class to hold all the stats for the bullet part of a tower. 
    /// Not used in Unity, but rather a cleaner way to implement stats for Tower.
    /// </summary>
    public class BulletStats
    {
        public GameObject bulletPrefab; //prefab of the bullet to be fired
        public float fireCooldown = 0.5f; //the time between shots
        public float damage = 1; //damage done on impact
        public float radius = 0; //the area the damage is applied to on impact, if it is 0, then no splash damage is applied
        public float chainAttackRadius = 0; //the radius to check for enemies to chain attacks to
        public float chainAttackReductionPercent = (1f / 3f); //the amount the damage is reduced to for each chained attack
        public int chainAttack_MaxBounces = 2; //the max amount of times to allow damage to bounce from the first target hit
    }
}
