using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TD.Core
{
    /// <summary>
    /// A really simple class that forces the attached canvas to point towards the main camera.
    /// Used for enemy world space canvases in order to keep them readable to the camera.
    /// </summary>
    public class CanvasToCamera : MonoBehaviour
    {
        private void Update()
        {
            //point the attached canvas to the camera
            transform.LookAt(Camera.main.transform);
        }
    }
}