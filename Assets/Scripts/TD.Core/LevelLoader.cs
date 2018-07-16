using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace TD.Core
{
    /// <summary>
    /// A useful class that handles level loading asynchronously.
    /// If prompted, it will display the loading screen and update the progress bar and text.
    /// It also rotates through background images and tip text on the loading screen, if provided.
    /// This script should live on your Manager game object.
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        //a quick reference to this control
        public static LevelLoader instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("More than one instance of LevelLoader found!");
                return;
            }
            instance = this;
        }

        public GameObject loadingScreen; //the root object of the loading screen
        public Slider loadingBar; //the progress bar to show loading
        public TMP_Text progressText; //displays the percent loaded
        public Image backgroundImage;
        public List<Sprite> galleryImages; //a list of images to cycle through
        public TMP_Text tipText;
        [TextArea(1, 5)]
        public List<string> tips; //a list of tips to cycle through
        public float timeBetweenImages = 2f; //time between images on the loading screen, set to <= 0 to disable changing
        public float timeBetweenTips = 5f; //time between tips changing, set to <= 0 to disable changing

        private void Start()
        {
            //disable the loading screen if it was left active
            if (loadingScreen.activeSelf)
            {
                loadingScreen.SetActive(false);
            }
        }

        /// <summary>
        /// A test function to allow testing of the image and tip changing 
        /// Use a button to trigger this function
        /// </summary>
        public void TEST()
        {
            Debug.Log("Starting loading screen test!");
            loadingScreen.SetActive(true);
            StartCoroutine(ChangeImage());
            StartCoroutine(ChangeTip());
        }

        /// <summary>
        /// Quit out of the application completely
        /// </summary>
        public void Quit()
        {
            Debug.Log("Exiting game...");
            Application.Quit();
        }

        /// <summary>
        /// Loads a scene based on the given index, if valid.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene. Must be between 0 and the number of scenes in the build settings.</param>
        public void LoadScene(int sceneIndex)
        {
            if (sceneIndex > SceneManager.sceneCountInBuildSettings || sceneIndex < 0)
            {
                return;
            }
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsynchronously(sceneIndex));
        }

        /// <summary>
        /// Loads a scene based on the given name.
        /// </summary>
        /// <param name="sceneName">Name of the scene. Cannot be null or empty string.</param>
        public void LoadScene(string sceneName)
        {
            if (sceneName == null || sceneName == "")
            {
                return;
            }
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsynchronously(sceneName));
        }

        /// <summary>
        /// Loads a scene based on the given index, if valid.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene. Must be between 0 and the number of scenes in the build settings.</param>
        public void LoadScene_NoLS(int sceneIndex)
        {
            if (sceneIndex > SceneManager.sceneCountInBuildSettings || sceneIndex < 0)
            {
                return;
            }
            StartCoroutine(LoadAsynchronously_NoLS(sceneIndex));
        }

        /// <summary>
        /// Loads a scene based on the given name.
        /// </summary>
        /// <param name="sceneName">Name of the scene. Cannot be null or empty string.</param>
        public void LoadScene_NoLS(string sceneName)
        {
            if (sceneName == null || sceneName == "")
            {
                return;
            }
            StartCoroutine(LoadAsynchronously_NoLS(sceneName));
        }

        /// <summary>
        /// Reloads the current level.
        /// </summary>
        public void ReloadCurrentLevel()
        {
            Debug.Log("Reloading current level");
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex));
        }

        /// <summary>
        /// A coroutine that asynchronously loads the given scene at the given index. Updates the loading screen UI as progress is made.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to load. Must be 0 or larger.</param>
        IEnumerator LoadAsynchronously_NoLS(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                loadingBar.value = progress;
                progressText.text = progress * 100 + "%";

                yield return null;
            }
        }

        /// <summary>
        /// A coroutine that asynchronously loads the given scene with the given name. Updates the loading screen UI as progress is made.
        /// </summary>
        /// <param name="sceneName">The index of the scene to load. Cannot be null</param>
        IEnumerator LoadAsynchronously_NoLS(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                loadingBar.value = progress;
                progressText.text = progress * 100 + "%";

                yield return null;
            }
        }

        /// <summary>
        /// A coroutine that asynchronously loads the given scene at the given index. Updates the loading screen UI as progress is made.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to load. Must be 0 or larger.</param>
        IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            loadingScreen.SetActive(true);
            StartCoroutine(ChangeImage());
            StartCoroutine(ChangeTip());

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                loadingBar.value = progress;
                progressText.text = progress * 100 + "%";

                yield return null;
            }
        }

        /// <summary>
        /// A coroutine that asynchronously loads the given scene with the given name. Updates the loading screen UI as progress is made.
        /// </summary>
        /// <param name="sceneName">The index of the scene to load. Cannot be null</param>
        IEnumerator LoadAsynchronously(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            loadingScreen.SetActive(true);
            StartCoroutine(ChangeImage());
            StartCoroutine(ChangeTip());

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                loadingBar.value = progress;
                progressText.text = progress * 100 + "%";

                yield return null;
            }
        }

        //called to continuously change the background image of the loading screen
        IEnumerator ChangeImage()
        {
            if (galleryImages.Count > 0)
            {
                backgroundImage.color = Color.white; //remove any tinting
                int r = Random.Range(0, galleryImages.Count);
                backgroundImage.sprite = galleryImages[r];
                if (timeBetweenImages > 0)
                {
                    yield return new WaitForSeconds(timeBetweenImages);
                    StartCoroutine(ChangeImage());
                }
            }
            else
            {
                Debug.Log("No images for LevelLoader!");
            }
        }

        //called to continuously change the tip text of the loading screen
        IEnumerator ChangeTip()
        {
            if (tips.Count > 0)
            {
                int r = Random.Range(0, tips.Count);
                tipText.text = tips[r];
                if (timeBetweenTips > 0)
                {
                    yield return new WaitForSeconds(timeBetweenTips);
                    StartCoroutine(ChangeTip());
                }
            }
            else
            {
                Debug.Log("No tips for LevelLoader!");
            }
        }

    }
}