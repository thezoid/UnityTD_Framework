using UnityEngine;
using UnityEngine.UI;

namespace TD.Core
{
    /// <summary>
    /// The central controller for each individual enemy.
    /// You should make a new empty game object and attach this script.
    /// Setup the graphics as a child of that object.
    /// Be sure to modify the stats of the script, then create a prefab of the object.
    /// Inject that prefab as a WaveComponent of one of the EnemySpawner Waves.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        public Slider healthBar; //enemies healthbar slider
        public GameObject deathEffect; //particle effect to spawn
        Transform targetPathNode; //the next node to move towards
        public float defaultSpeed = 5; //normal move speed
        private float speed; //current move sppeed
        public float health = 1; //current health
        public float rotSpeed = 5; //how fast to rotate when moving towards node, make it larger for snapier movement
        public int moneyValue = 1; //money reward for killing enemy

        private bool isDead = false; //flag to make sure death function doesnt trigger more than once

        // Use this for initialization
        void Start()
        {
            targetPathNode = EnemySpawner.instance.spawnPoint.GetComponent<PathNode>().nextNode;
            //Debug.Log("Current Node: " + targetPathNode + "Next node: " + targetPathNode.GetComponent<PathNode>().nextNode);
            healthBar.maxValue = health;
            healthBar.value = health;
            speed = defaultSpeed;
        }

        /// <summary>
        /// Slows down the enemy by the given percent
        /// </summary>
        /// <param name="percentToSlow">The percent to slow. Clamped between 0 and 1</param>
        public void Slow(float percentToSlow)
        {
            if(percentToSlow > 1 || percentToSlow < 0)
            {
                Debug.LogWarning("Trying to slow enemy by negative or value greater than 1");
                percentToSlow = Mathf.Clamp01(percentToSlow);
            }
            speed = defaultSpeed * (1 - percentToSlow);
        } 

        /// <summary>
        /// Set our target node as the current nodes pointer. If the pointer is null, it should have reached the end of the path.
        /// </summary>
        void GetNextPathNode()
        {
            
            if (targetPathNode.GetComponent<PathNode>().nextNode == null)
            {
                //Debug.Log("GetNextPathNode next node is null");
                //targetPathNode = null;
                ReachedGoal();
                return;
            }
            //Debug.Log("Current Node: "+targetPathNode+"Next node: " + targetPathNode.GetComponent<PathNode>().nextNode);
            targetPathNode = targetPathNode.GetComponent<PathNode>().nextNode;

        }


        void Update()
        {
            //direction to the target node
            Vector3 dir = targetPathNode.position - transform.localPosition;

            //distance to move this frame
            float distThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distThisFrame)
            {
                //we've reached the node
                //targetPathNode = null;
                GetNextPathNode();
            }
            else
            {
                //move towards node
                transform.Translate(dir.normalized * distThisFrame, Space.World);
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
            }

            //resets the speed in the case that the enemy was slowed
            speed = defaultSpeed;
        }

        /// <summary>
        /// The enemy reached the goal!
        /// Destroy it and trigger life loss.
        /// </summary>
        void ReachedGoal()
        {
            Destroy(gameObject);
            PlayerStats.instance.LoseLife();
        }

        /// <summary>
        /// Take damage based on the amount given. If we go below 0 health, calls the die function.
        /// </summary>
        /// <param name="amount">The amount of damage to take. Must be greater than 0</param>
        public void takeDamage(float amount)
        {
            if (amount < 0f)
            {
                return;
            }
            health -= amount;
            healthBar.value = health;
            if (health <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Handle destroying the object, rewarding the player, and spawning death particles.
        /// </summary>
        void Die()
        {
            if (isDead)
            {
                return;
            }

            EnemySpawner.instance.enemiesOnLevel--;
            isDead = true;
            GameObject de = Instantiate(deathEffect, this.transform.position, this.transform.rotation);
            Destroy(de, 2f);
            Destroy(gameObject);
            PlayerStats.instance.GainMoney(this.moneyValue);
        }
    }
}

