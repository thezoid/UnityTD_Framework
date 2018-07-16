using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Core
{
    [System.Serializable]
    /// <summary>
    /// A class to hold all the stats for a tower. 
    /// Not used in Unity, but rather a cleaner way to implement stats for Tower.
    /// </summary>
    public class TowerStats
    {

        [HideInInspector]
        public float sellValue; //the worth of the tower, used when selling a tower
        public float cost = 5; //the cost to purchase a tower
        public float range = 10; //the max range an enemy can be targeted at
        public string displayName = "Tower"; //the display name of the tower, used in the upgrade shop

        //All the constraints of our upgrades
        public float MAX_RANGE = 40f; //the maxmium range this tower can have
        public float MIN_FIRERATE = 0.01f; //the fastest fire rate this tower can have
        public float MAX_DAMAGE = 75f; //the most damage this tower can do
        public float MAX_RADIUS = 25f; //the furthest splash distance for this tower

        //the base costs of each stat upgrade
        public float rangeCost = 25f, fireRateCost = 5f, damageCost = 10f, radiusCost = 15f;

    }
}