using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TD.Core
{
    /// <summary>
    /// A simple class that handles a fade in or out of scenes. Relies on the function of LevelLoader to actually load scenes.
    /// Create a new empty game object and child a canvas with an image to it. Attach this script to the empty game object and setup the image to connect
    /// to the script as the image to fade.
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        public Image imageToFade; //the screen size image to fade in or fade out
        public AnimationCurve fadeCurve; //customizable curve to control fade rate
        public static SceneFader instance; //a quick reference to this control

        private void Start()
        {
            if(instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            //when a scene starts, we should fade in
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// A helper function to fade out to a scene by name
        /// The scene to transition to MUST be in the build order
        /// </summary>
        /// <param name="scene">The string containing the name of the scene</param>
        public void FadeOutTo(string scene)
        {
            StartCoroutine(FadeOut(scene));
        }

        /// <summary>
        /// A helper function to fade out to a scene by name
        /// The scene to transition to MUST be in the build order
        /// </summary>
        /// <param name="scene">The int containing the index of the scene</param>
        public void FadeOutTo(int scene)
        {
            StartCoroutine(FadeOut(scene));
        }

        /// <summary>
        /// A helper function to fade out and reload the current scene
        /// The scene to transition to MUST be in the build order
        /// </summary>
        public void FadeOut_RestartCurrent()
        {
            StartCoroutine(FadeOut_Restart());
        }

        /// <summary>
        /// A helper function to fade out to a scene by name
        /// The scene to transition to MUST be in the build order
        /// This version will NOT trigger the loading screen
        /// </summary>
        /// <param name="scene">The string containing the name of the scene</param>
        public void FadeOutTo_NoLS(string scene)
        {
            StartCoroutine(FadeOut_NoLS(scene));
        }

        /// <summary>
        /// A helper function to fade out to a scene by name
        /// The scene to transition to MUST be in the build order
        /// This version will NOT trigger the loading screen
        /// </summary>
        /// <param name="scene">The int containing the index of the scene</param>
        public void FadeOutTo_NoLS(int scene)
        {
            StartCoroutine(FadeOut_NoLS(scene));
        }

        //fades in to the scene, lowering the alpha of the imageToFade based on the fadeCurve
        IEnumerator FadeIn()
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }
        }

        //fades out of the current scene, triggering the loading of the next scene via LevelLoader
        IEnumerator FadeOut(string scene)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }

            imageToFade.color = Color.clear;
            yield return null;
            LevelLoader.instance.LoadScene(scene);
        }

        //fades out of the current scene, triggering the loading of the next scene via LevelLoader
        IEnumerator FadeOut(int scene)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }

            imageToFade.color = Color.clear;
            yield return null;
            LevelLoader.instance.LoadScene(scene);
        }

        //fades out of the current scene, triggering the loading of the next scene via LevelLoader
        IEnumerator FadeOut_NoLS(string scene)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }
            LevelLoader.instance.LoadScene_NoLS(scene);
        }

        //fades out of the current scene, triggering the loading of the next scene via LevelLoader
        IEnumerator FadeOut_NoLS(int scene)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }
            LevelLoader.instance.LoadScene_NoLS(scene);
        }

        //fades out of the current scene, triggering the reloading of the current scene via LevelLoader
        IEnumerator FadeOut_Restart()
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Color col = imageToFade.color;
                float alpha = fadeCurve.Evaluate(t);
                col.a = alpha;
                imageToFade.color = col;
                yield return null;
            }

            imageToFade.color = Color.clear;
            yield return null;
            LevelLoader.instance.ReloadCurrentLevel();
        }

    }
}