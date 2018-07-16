using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

namespace TD.Core
{
    /// <summary>
    /// A general class that contains player stats for the current level. Provides analytical stats (total earned, total spent), as well as practical stats (current money, lives).
    /// Provides functionality of all the stats as well. Works mostly as a general GameManager. Place it on your GameManager object.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        //the most lives a player can have
        const int MAXLIVES = 99;
        //current lives
        int lives;
        //lives given at the start of the level
        public int startingLives = 20;
        //current money
        float money;
        //total money made on a level
        float totalMoneyGained = 0;
        //total money spent on a level
        float totalMoneySpent = 0;
        //number of waves beaten
        int wavesCompleted;
        //money given at the start of the level
        public float startingMoney = 400; 
        public static PlayerStats instance;
        //all the text elements for displaying info on UI
        public TMP_Text moneyText, livesText, wavesText, gameOverWavesText, victoryWavesText;
        //the root object of the game over screen
        public GameObject gameOverScreen;
        //the root object of the victory screen
        public GameObject victoryScreen;
        //the name of the next scene to unlock
        public string nextLevelName = "Level2";
        //the number of the next level to unlock; will affect unlocked buttons in the level select
        public int levelToUnlock = 2; 

        [HideInInspector]
        //flag to allow other scripts to detect if the game is over
        public bool gameIsOver = false; 
        [HideInInspector]
        //flag to see if the player beat the level
        public bool gameWon = false; 

        private void Awake()
        {
            if (instance != null)
            {
                Debug.Log("PlayerStats already exists!");
                Destroy(gameObject);
                return;
            }
            instance = this;

            //setup initial lives and money
            lives = startingLives;
            money = startingMoney;

            //setup display text
            UpdateLivesText();
            UpdateMoneyText();
            UpdateWaveText();

        }

        void UpdateLivesText()
        {
            livesText.text = "Lives: " + GetLives().ToString();
        }

        void UpdateMoneyText()
        {
            moneyText.text = "Money: $" + GetMoney().ToString("0.00");
        }

        void UpdateWaveText()
        {
            wavesText.text = "Wave: " + (wavesCompleted+1).ToString("000");
        }

        /// <summary>
        /// Get the number of lives
        /// </summary>
        /// <returns>Retuns the number of lives as an int</returns>
        public int GetLives()
        {
            return lives;
        }

        /// <summary>
        /// Lose an amount of lives that is greater than 0
        /// If the number of lives runs below 0, end the game
        /// Updates the UI after removing lives
        /// </summary>
        /// <param name="l">The number of lives to lose. Defaults to 1.</param>
        public void LoseLife(int l = 1)
        {
            //make sure we dont lose negative lives (would make us gain a life)
            if (l <= 0)
            {
                return;
            }
            lives -= l;
            //end the game if we run out of lives
            if (lives <= 0)
            {
                gameWon = false;
                EndGame();
            }
            UpdateLivesText();
        }

        /// <summary>
        /// Gain an amount of lives that is greater than 0
        /// If the number of lives is greater than the max, sets the number of lives to the max
        /// Updates the UI after adding lives
        /// </summary>
        /// <param name="l">The number of lives to gain. Defaults to 1.</param>
        public void GainLife(int l = 1)
        {
            //return if given negative value or value puts us over max lives
            if (l <= 0)
            {
                return;
            }

            lives += l;
            
            //should never happen, but check just in case
            if (lives > MAXLIVES)
            {
                lives = MAXLIVES;
            }

            UpdateLivesText();
        }

        /// <summary>
        /// Get the current amount of money
        /// </summary>
        /// <returns>The current amount of money</returns>
        public float GetMoney()
        {
            return money;
        }

        /// <summary>
        /// Adds money to the total money, as long as the given value is greater than 0
        /// Updates the UI after adding money
        /// Updates the stat tracking of the total money gained
        /// </summary>
        /// <param name="m">The amount of money to gain. Defaults to 1</param>
        public void GainMoney(float m = 1)
        {
            //prevent gaining negative money (would lose money)
            if (m <= 0)
            {
                return;
            }
            money += m;
            UpdateMoneyText();
            //update overall stat tracking
            totalMoneyGained += m;
        }

        /// <summary>
        /// Removes money from the total money, as long as the given value is greater than 0
        /// Updates the UI after adding money
        /// Updates the stat tracking of the total money spent
        /// </summary>
        /// <param name="m">The amount of money to spend. Defaults to 1</param>
        public void SpendMoney(float m = 1)
        {
            //prevent spending negative money (would gain money)
            if (m <= 0)
            {
                return;
            }
            money -= m;
            UpdateMoneyText();
            //update overall stat tracking
            totalMoneySpent += m;
        }


        /// <summary>
        /// Handles the player side of ending a wave
        /// Increments the waves completed tracker
        /// Updates the wave tracker text
        /// Gains bonus money based on the wave bonus
        /// </summary>
        /// <param name="bonus">The bonus money to gain from the wave ending</param>
        public void WaveCompleted(float bonus = 0)
        {
            wavesCompleted++;
            UpdateWaveText();

            //todo: add bonus money? interest or bank?
            GainMoney(bonus); //the bonus is passed through based on the wave's wave bonus from the enemy spawner
        }

        /// <summary>
        /// Fade to our next level via SceneFader
        /// </summary>
        public void FadeToNextLevel()
        {
            SceneFader.instance.FadeOutTo(nextLevelName);
        }

        /// <summary>
        /// End the game and trigger events based on whether the player won
        /// </summary>
        public void EndGame()
        {
            //make sure this triggers once
            if (gameIsOver)
            {
                return;
            }

            Debug.Log("Game Over");
            gameIsOver = true;

            //if victory
            if (gameWon)
            {
                //update game over screen text
                //victoryWavesText.text = wavesCompleted.ToString();

                //show victory screen
                if (!victoryScreen.activeSelf)
                {
                    victoryScreen.SetActive(true);
                }

                //update our levelsReached record for the level select
                int levelReached = PlayerPrefs.GetInt("levelReached", 1);
                if(levelReached < levelToUnlock)
                {
                    PlayerPrefs.SetInt("levelReached", levelToUnlock);
                }
            }
            //if loss
            else
            {
                //update game over screen text
                //gameOverWavesText.text = wavesCompleted.ToString();
                //show defeat screen
                if (!gameOverScreen.activeSelf)
                {
                    gameOverScreen.SetActive(true);
                }
            }

            StartCoroutine(AnimateText()); //animate the waves cleared to count up

            //disable camera controls
            Camera.main.GetComponent<CameraController>().ResetToStartingPosition();
            Camera.main.GetComponent<CameraController>().enabled = false;
        }

        //animates the wave text on the victory or game over screens to count up the waves beaten
        IEnumerator AnimateText()
        {
            int round = 0;
            while(round < wavesCompleted)
            {
                round++;
                if (gameWon)
                {
                    victoryWavesText.text = round.ToString();
                }
                else
                {
                    gameOverWavesText.text = round.ToString();
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}