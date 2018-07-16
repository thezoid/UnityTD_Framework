using UnityEngine;

namespace TD.Core
{
    /// <summary>
    /// A class to handle wheter a tower can currently be built, and handles the selected tower to be built.
    /// This class should live on your GameManager object.
    /// Referenced to see if there is a tower selected to be built, and if there is enough money to buy that tower.
    /// </summary>
    public class BuildingManager : MonoBehaviour
    {

        public GameObject selectedTower; //the currently selected tower, will be used when referncing what to build
        public static BuildingManager instance; //easier access reference

        /// <summary>
        /// A property that inspects if there is a valid tower reference
        /// </summary>
        public bool canBuild { get { return selectedTower != null;  } }

        /// <summary>
        /// A property to check if the player has enough money to purchase the currently selected tower
        /// </summary>
        public bool hasMoney { get { return PlayerStats.instance.GetMoney() >= selectedTower.GetComponent<Tower>().stats.cost; } }

        void Start()
        {
            if (instance != null)
            {
                Debug.Log("There is already an instance of BuildingManager!");
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Select the BuildingManager's tower prefab
        /// </summary>
        /// <param name="prefab">The tower prefab to select</param>
        public void SelectTowerType(GameObject prefab)
        {
            selectedTower = prefab;
        }
    }
}