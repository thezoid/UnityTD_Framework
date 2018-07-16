using UnityEngine;

namespace TD.Core
{

    /// <summary>
    /// A class that controls camera movement.
    /// Should be attached to the main camera object.
    /// IMPORTANT: 
    /// Setup your boundaries per level. 
    /// The way a level is layed out will change what the boundaries should be.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public float panSpeed = 30f; //x and z axis speed modifier
        public float panBorderThickness = 10f; //edge of screen zone to trigger scrolling
        public float scrollSpeed = 15f; //mousewheel zoom speed
        public float maxZoom = 80f, minZoom = 10f; //the max in and out to zoom
        public float maxXpos = 100f, maxXneg = -100f, maxZpos = 100f, maxZneg = -100f; //the boundaries of moving the camera on the x and z axis
        Vector3 startingPosition; //return position for when the game ends

        private void Start()
        {
            //setup the return position
            startingPosition = this.transform.position;

            //error check and report of camera boundaries
            if (maxXpos < 0)
            {
                maxXpos = 0;
                Debug.LogWarning("camera maxXpos was negative, now 0");
            }
            if (maxXneg > 0)
            {
                maxXneg = 0;
                Debug.LogWarning("camera maxXneg was positive, now 0");
            }
            if (maxZpos < 0)
            {
                maxZpos = 0;
                Debug.LogWarning("camera maxZpos was negative, now 0");
            }
            if (maxZneg > 0)
            {
                maxZneg = 0;
                Debug.LogWarning("camera maxZneg was positive, now 0");
            }
        }

        /// <summary>
        /// Return the camera to its starting position.
        /// </summary>
        public void ResetToStartingPosition()
        {
            this.transform.position = startingPosition;
        }

        void Update()
        {
         
            //mouse on defined edge border of screen
            //top
            if (Input.mousePosition.y >= (Screen.height - panBorderThickness))
            {
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
                Vector3 pos = transform.position;
                pos.z = Mathf.Clamp(pos.z, maxZneg, maxZpos);
                transform.position = pos;
            }
            //bottom
            else if (Input.mousePosition.y <= panBorderThickness)
            {
                transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
                Vector3 pos = transform.position;
                pos.z = Mathf.Clamp(pos.z, maxZneg, maxZpos);
                transform.position = pos;
            }
            //right
            else if (Input.mousePosition.x >= (Screen.width - panBorderThickness))
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, maxXneg, maxXpos);
                transform.position = pos;
            }
            //left
            else if (Input.mousePosition.x <= panBorderThickness)
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, maxXneg, maxXpos);
                transform.position = pos;
            }

            //key camera movement
            //a,d,left, right
            if (Input.GetAxis("Horizontal") != 0)
            {
                float horizontal = Input.GetAxis("Horizontal");
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime * horizontal, Space.World);
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, maxXneg, maxXpos);
                transform.position = pos;
            }

            //w,s,up,down
            if (Input.GetAxis("Vertical") != 0)
            {
                float vertical = Input.GetAxis("Vertical");
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime * vertical, Space.World);
                Vector3 pos = transform.position;
                pos.z = Mathf.Clamp(pos.z, maxZneg, maxZpos);
                transform.position = pos;
            }

            //handle zooming in and out with the scroll wheel
            //very basic movement currently
            //implement standard rts camera in the future
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                //Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Vector3 pos = transform.position;
                pos.y -= scroll * scrollSpeed * Time.deltaTime;
                pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
                transform.position = pos;
            }

        }
    }
}