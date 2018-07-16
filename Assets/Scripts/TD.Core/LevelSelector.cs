using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TD.Core
{
    /// <summary>
    /// A class to manage the level select menu. 
    /// Create your level select button layout, then setup your reference for this object.
    /// This script should live on your LevelSelect scene Manager object.
    /// Content should be the object containing all the buttons.
    /// </summary>
    public class LevelSelector : MonoBehaviour
    {

        public GameObject content; //the root object of all our buttons
        Button[] levelButtons; //all of our level select buttons
        

        private void Start()
        {
            //get all our buttons from their root content object
            levelButtons = content.GetComponentsInChildren<Button>();

            //load our highest level unlocked from PlayerPrefs
            //disable any level button above this level
            int levelReached = PlayerPrefs.GetInt("levelReached",1);
            //Debug.Log("Level Reached: " + levelReached);
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (i+1 > levelReached)
                {
                    levelButtons[i].interactable = false;
                }

            }
        }

        /// <summary>
        /// Select a level by name and pass it to the SceneFader to transition to
        /// </summary>
        /// <param name="levelName">The name of the level to transition to. SCENE MUST BE IN THE BUILD ORDER.</param>
        public void Select(string levelName)
        {
            SceneFader.instance.FadeOutTo(levelName);
        }

        /// <summary>
        /// Select a level by index and pass it to the SceneFader to transition to
        /// </summary>
        /// <param name="levelName">The index of the level to transition to. SCENE MUST BE IN THE BUILD ORDER.</param>
        public void Select(int levelIndex)
        {
            SceneFader.instance.FadeOutTo(levelIndex);
        }

    }
}