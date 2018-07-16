using UnityEngine;
using UnityEngine.EventSystems;

namespace TD.Core
{
    /// <summary>
    /// A core script that handles the behavior of a tower.
    /// Create a new empty game object and attach this script to it and setup the graphics as a child object. 
    /// Then make that object a prefab and delete it from the scene. Add a button to let the BuildingManager select that tower type 
    /// and it should be able to spawn.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        [Header("General Stats")]
        static int nextIDNum = 1; //the next id number for a tower
        public int idNum; //the id of the tower
        public Transform turretTransform; //the part to rotate
        public float turretRotSpeed; //panning speed of turret rotation
        public Transform bulletSpawnPoint; //the point from which to fire
        public TowerStats stats; //primary stats for the tower
        
        //bullet vars
        [Header("Uses Bullets")]
        public BulletStats bulletStats; //holds all the stats needed for a tower that uses bullets
        float fireCooldownLeft = 0; //the time left until able to fire


        //laser
        [Header("Uses Laser")]
        public bool isLaser = false; //whether the tower is a laser
        public LaserStats laserStats; //holds all the stats needed for a laser tower

        //[Header("Tower Spot")]
        [HideInInspector]
        public GameObject towerSpot; //the tower plot this turret is built on
        Color hoverColor; //the color when the mouse hovers over a tower
        Color defaultColor; //the default color when a tower is not moused over
        Renderer thisRenderer; //the tower spot renderer


        private void Awake()
        {
            //provide additional tower sell value
            stats.sellValue = stats.cost;
        }

        void Start()
        {
            thisRenderer = towerSpot.GetComponent<Renderer>();
            defaultColor = towerSpot.GetComponent<TowerNode>().defaultColor;
            hoverColor = towerSpot.GetComponent<TowerNode>().hoverColor;
        }

        // Update is called once per frame
        void Update()
        {
            //get all our enemies
            Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();

            //find the closest enemy in range
            Enemy nearestEnemy = null;
            float dist = Mathf.Infinity;
            foreach (Enemy e in enemies)
            {
                float d = Vector3.Distance(this.transform.position, e.transform.position);
                if (nearestEnemy == null || d < dist)
                {
                    if (d < stats.range)
                    {
                        nearestEnemy = e;
                        dist = d;
                    }
                }
            }

            //handles when no enemy is in range
            if (nearestEnemy == null)
            {
                if (isLaser)
                {
                    //disable our laser effects if they are enabled
                    //due to how we handle all 3 effects together, we can assume if one is on, they are all on
                    if (laserStats.lineRenderer.enabled)
                    {
                        laserStats.lineRenderer.enabled = false;
                        laserStats.laserImpactEffect.Stop();
                        laserStats.impactLight.enabled = false;
                    }

                    //start to lose charge
                    if (laserStats.moreDamageOvertime)
                    {
                        laserStats.laserEngagedTimer -= Time.deltaTime;
                        laserStats.laserEngagedTimer = Mathf.Clamp(laserStats.laserEngagedTimer, 0f, laserStats.maxEngagedTimer);
                    }
                }
                return;
            }

            //point the turret at the closest enemy
            Vector3 dir = nearestEnemy.transform.position - this.transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            turretTransform.rotation = Quaternion.Slerp(turretTransform.rotation, lookRot, Time.deltaTime * turretRotSpeed);

            //handles engaging an enemy with an attack
            if (isLaser)
            {
                if (laserStats.moreDamageOvertime)
                {
                    laserStats.laserEngagedTimer += Time.deltaTime;
                    laserStats.laserEngagedTimer = Mathf.Clamp(laserStats.laserEngagedTimer, 0f, laserStats.maxEngagedTimer);
                }
                LaserAt(nearestEnemy);
            }
            else
            {
                fireCooldownLeft -= Time.deltaTime;
                if (fireCooldownLeft <= 0 && dir.magnitude <= stats.range)
                {
                    fireCooldownLeft = bulletStats.fireCooldown;
                    ShootAt(nearestEnemy);
                }
            }

        }

        /// <summary>
        /// A function to update the sell value of the turret
        /// </summary>
        /// <param name="amount">The amount to add to the turrets sell value</param>
        public void addWorth(float amount)
        {
            if (amount <= 0)
            {
                return;
            }
            stats.sellValue += amount;
        }

        /// <summary>
        /// Do a laser attack towards the given enemy
        /// </summary>
        /// <param name="e">The enemy to laser attack at</param>
        void LaserAt(Enemy e)
        {
            //enable the laser and effects
            if (!laserStats.lineRenderer.enabled)
            {
                laserStats.lineRenderer.enabled = true;
                laserStats.laserImpactEffect.Play();
                laserStats.impactLight.enabled = true;
            }

            //update the laser linerender positions
            laserStats.lineRenderer.SetPosition(0, bulletSpawnPoint.position);
            laserStats.lineRenderer.SetPosition(1, e.transform.position);

            //rotate to face tower (particles "blow back")
            Vector3 dir = bulletSpawnPoint.transform.position - e.transform.position;
            laserStats.laserImpactEffect.transform.rotation = Quaternion.LookRotation(dir);

            //move particles to targeted enemy
            //do math to move the impact away from the middle of the enemy transform
            laserStats.laserImpactEffect.transform.position = e.transform.position + dir.normalized * 0.5f;

            //charge laser
            if (laserStats.moreDamageOvertime)
            {
                laserStats.lineRenderer.endWidth += (laserStats.laserEngagedTimer * laserStats.lineRenderer.endWidth);
                laserStats.lineRenderer.endWidth = Mathf.Clamp(laserStats.lineRenderer.endWidth, 0.1f, .5f);
                float dam = Mathf.Clamp((laserStats.damageOverTime * Time.deltaTime), 0f, Mathf.Infinity);
                float bonusDam = Mathf.Clamp((dam * laserStats.laserEngagedTimer), 0f, Mathf.Infinity);
                e.takeDamage(dam + bonusDam);
                //Debug.Log("Charge laser damage: " + dam + "| Engaged time: " + laserStats.laserEngagedTimer + "| Enganged bonus damage: " + bonusDam + "|Total damage: " + (dam + bonusDam));
            }
            //regular (slowing) laser
            else
            {
                e.takeDamage(laserStats.damageOverTime * Time.deltaTime);
                e.Slow(laserStats.slowPercent);
            }
        }

        /// <summary>
        /// Do a projectile based attack at the given enemy.
        /// </summary>
        /// <param name="e">The enemy to shoot at</param>
        void ShootAt(Enemy e)
        {
            //spawn bullet and update its properties
            GameObject bulletGO = (GameObject)Instantiate(bulletStats.bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Bullet b = bulletGO.GetComponent<Bullet>();
            b.damage = bulletStats.damage;
            b.radius = bulletStats.radius;
            b.target = e.transform;
            b.chainAttackRadius = bulletStats.chainAttackRadius;
            b.chainAttackReductionPercent = bulletStats.chainAttackReductionPercent;
            b.chainAttack_MaxBounces = bulletStats.chainAttack_MaxBounces;
        }

        private void OnMouseEnter()
        {
            //update tower node color
            thisRenderer.material.color = hoverColor;
        }

        private void OnMouseExit()
        {
            //update tower node color
            if (UpgradeManager.instance.selectedTower != null && this.gameObject == UpgradeManager.instance.selectedTower.gameObject)
            {
                thisRenderer.material.color = UpgradeManager.instance.selectedColor;
            }
            else
            {
                thisRenderer.material.color = defaultColor;
            }
        }

        private void OnMouseUp()
        {
            //select the tower if it is clicked on
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Tower clicked.");
                if (!UpgradeManager.instance.menuRoot.gameObject.activeSelf)
                {
                    UpgradeManager.instance.ToggleMenu();
                }
                UpgradeManager.instance.SetSelectedTower(this);
            }
        }

        /// <summary>
        /// Set the turret's id to the next ID
        /// </summary>
        public void SetID()
        {
            idNum = nextIDNum;
            nextIDNum++;
        }

        /// <summary>
        /// Sell the tower for its sell value
        /// </summary>
        public void Sell()
        {
            //get rid of model
            Destroy(gameObject);
            //gain back the worth of the tower
            PlayerStats.instance.GainMoney(stats.sellValue);
            //unlink the tower from the node and update the tower node color
            towerSpot.GetComponent<TowerNode>().unlinkTurret();
            thisRenderer.material.color = defaultColor;
        }

        private void OnDrawGizmos()
        {
            //generate random color
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, stats.range);
        }
    }
}
