using UnityEngine;

namespace TD.Core
{
    /// <summary>
    /// A simple doubly linked list to provide enemies with a way to follow waypoints.
    /// Place on you waypoint object (can be an empty object).
    /// </summary>
    public class PathNode : MonoBehaviour
    {
        public Transform nextNode;
        public Transform lastNode;
    }
}

