using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Core
{
    [System.Serializable]
    /// <summary>
    /// A class to hold all the stats for the laser part of a tower. 
    /// Not used in Unity, but rather a cleaner way to implement stats for Tower.
    /// </summary>
    public class LaserStats
    {
        public float damageOverTime = 30; //the lasers damage per second
        public bool moreDamageOvertime = false; //whether the tower does more laser damage over time
        [HideInInspector]
        public float laserEngagedTimer = 0f; //how long the laser has currently been attack; buffs the damage being done
        public float maxEngagedTimer = 5f; //the current time the tower has engaged targets; the max time bonus a turret can get from engaging targets
        public LineRenderer lineRenderer; //the linerenderer to make the laser effect
        public ParticleSystem laserImpactEffect; //part of the system to show the effect of the laser on the target
        public Light impactLight; //part of the system to show the effect of the laser on the target
        public float slowPercent = .5f; //the amount to slow the target down by
    }
}
