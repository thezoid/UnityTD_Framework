using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TD.Core
{

    /// <summary>
    /// This class is mostly responsible for holding and handling wave spawns.
    /// This script should live on your GameManager object.
    /// Contains the WaveComponent and Wave classes to allow for easy customization of waves on the Manager object.
    /// Enemies spawned by this need a system of PathNodes setup in order to allow them to path properly.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner instance; //reference to the manager object
        public GameObject spawnPoint; //the node to spawn the enemies at
        float spawnCDremaining = 0; //timer for between spawns
        float waveTimer = 5; //timer for between waves
        public float timeBetweenWaves = 60f; //time between waves
        public float initialWaitTime = 90f; //time before first wave spawns
        public TMP_Text waveCountdownText; //the text to show the wave ooldown
        int waveIndex = 0; //the current wave we're on
        public int enemiesOnLevel = 0; //the enemies remaining on the level

        [Header("DEBUG -- DO NOT USE FOR MAIN GAME")]
        [SerializeField]
        bool shouldLoopWaves = false;

        [System.Serializable]
        /// <summary>
        /// A wave component contains the prefab to spawn, the number to spawn, the number that have spawned, and the time between spawns.
        /// Many of these make up one wave, each providing one enemy type to spawn
        /// </summary>
        public class WaveComponent
        {
            public GameObject enemyPrefab;
            public int num;
            [HideInInspector]
            public int spawned = 0;
            public float spawnCD = 1f; //the time between spawns
        }

        //helper class to hold a collection of waveComps
        [System.Serializable]
        /// <summary>
        /// A classic approach to sending enemies.
        /// Contains many WaveComponents to comprise up spawn waves.
        /// Optionally allows for a wave bonus money to be rewarded upon wave completion
        /// </summary>
        public class Wave
        {
            public WaveComponent[] waveComps;
            [Header("Optional")]
            public int waveBonusMoney = 0; //extra money to give the user when a wave finishes spawning
        }

        //a collection of all our waves
        public List<Wave> waves;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                Debug.LogWarning("There is more than one instance of EnemySpawner2!");
                Destroy(this);
                return;
            }
            instance = this;

            waveTimer = initialWaitTime;
        }

        /// <summary>
        /// Lets the wave count down button skip to spawn the wave
        /// </summary>
        public void SkipWaveTimer()
        {
            waveTimer = 0f;
        }

        void Update()
        {
            //not at end of waves
            if (waveIndex < waves.Count)
            {
                //update wave timer
                waveTimer -= Time.deltaTime;
                waveTimer = Mathf.Clamp(waveTimer, 0f, Mathf.Infinity);
                if (waveTimer > 0)
                {
                    //update timer text
                    float minutes = Mathf.Floor(waveTimer / 60);
                    float seconds = (waveTimer % 60);
                    waveCountdownText.text = "Wave " + (waveIndex + 1).ToString("0") + ":\n" + minutes.ToString("00") + ":" + seconds.ToString("00.00");
                }
                else
                {
                    //inform the user that a wave is spawning
                    waveCountdownText.text = "Wave spawning!";
                }
                //spawn a wave
                if (waveTimer <= 0)
                {
                    bool didSpawn = false;
                    spawnCDremaining -= Time.deltaTime;
                    spawnCDremaining = Mathf.Clamp(spawnCDremaining, 0f, Mathf.Infinity);
                    if (spawnCDremaining <= 0)
                    {
                        //go through wave comp until we find a spawn
                        foreach (WaveComponent wc in waves[waveIndex].waveComps)
                        {
                            if (wc.spawned < wc.num)
                            {
                                spawnCDremaining = wc.spawnCD;
                                //spawn it
                                wc.spawned++;
                                //GameObject e = 
                                Instantiate(wc.enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                                didSpawn = true;
                                enemiesOnLevel++;
                                break;
                            }

                        }

                        if (!didSpawn)
                        {
                            //only advance the wave if there are no enemies left
                            if (enemiesOnLevel <= 0)
                            {
                                //end of current wave
                                PlayerStats.instance.WaveCompleted(waves[waveIndex].waveBonusMoney);
                                waveTimer = timeBetweenWaves;
                                waveIndex++;
                            }
                        }
                    }

                }
            }
            else
            {
                //waves are over, end the game
                //Debug.Log("waves are over, end the game");
                waveCountdownText.text = "Level complete!";
                if (enemiesOnLevel <= 0)
                {
                    if (shouldLoopWaves)
                    {
                        loopWaves();
                    }
                    else
                    {
                        PlayerStats.instance.gameWon = true;
                        PlayerStats.instance.EndGame();
                    }
                }
            }
        }


        //debug method to allow waves to continually loop
        //comment out the code to end the level and replace it with this method to loop a level's waves
        void loopWaves()
        {

            foreach (Wave w in waves)
            {
                Debug.Log("RESETING AND LOOPING WAVES - CHANGE THIS AFTER TESTING");
                foreach (WaveComponent wc in w.waveComps)
                {
                    wc.spawned = 0;
                }
            }
            waveIndex = 0;
        }
    }


}