using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Core
{
    /// <summary>
    /// A class that handles showing the pause menu and pausing and unpausing of time.
    /// Place on your GameManager object and hook up the reference to your Pause Menu root item.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenuUI; //the pause menu root object

        private void OnEnable()
        {
            //make sure we unpause time if it is paused
            if(Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
        }

        private void Update()
        {
            //looks for input for the menu
            //its better to use an input axis, but looking for a specific keypress is ok in this instance
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }


        /// <summary>
        /// Toggles the current state of the pause menu
        /// Pauses or unpauses time based on the menu state
        /// </summary>
        public void Toggle()
        {
            //toggle display state
            pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);

            if (pauseMenuUI.activeSelf)
            {
                //pause time
                Time.timeScale = 0;
            }
            else
            {
                //unpause time
                Time.timeScale = 1;
            }
        }
    }
}