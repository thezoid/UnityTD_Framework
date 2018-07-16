using UnityEngine;
using UnityEngine.EventSystems;

namespace TD.Core
{
    /// <summary>
    /// A class to handle a blank spot that can eventually hold a tower.
    /// Create an object to represent a spot in the world and attach this script to it.
    /// </summary>
    public class TowerNode : MonoBehaviour
    {

        public Color hoverColor; //the color to change to when the mouse is over this spot
        public Color defaultColor; //the color to change to when not being hovered over
        public Color cantBuyColor; //the color to change to when the mouse is over this spot and they cant afford the selected tower
        public GameObject buildEffect, sellEffect;


        GameObject currentTower; //this spots tower
        Renderer thisRenderer; //cache of the renderer

        /// <summary>
        /// Nulls out the reference to the tower placed on this node and spawns the sell effect when unlinking the tower
        /// </summary>
        public void unlinkTurret()
        {
            currentTower = null;
            GameObject be = Instantiate(sellEffect, transform.parent.position, transform.parent.rotation);
            Destroy(be, 2f);
        }

        private void Start()
        {
            thisRenderer = GetComponent<Renderer>();
            defaultColor = thisRenderer.material.color;
        }

        private void OnMouseEnter()
        {
            //handles shading of selected tower node
            //if we cant afford the selected tower or there is no selected tower to build, make the node the cantBuyColor
            //else make it the color when hovering
            if (!BuildingManager.instance.hasMoney && currentTower == null)
            {
                thisRenderer.material.color = cantBuyColor;
            }
            else
            {
                thisRenderer.material.color = hoverColor;
            }
            
        }

        private void OnMouseExit()
        {
            //handles reseting the node color when mouse unselects
            if (UpgradeManager.instance.selectedTower != null && currentTower == UpgradeManager.instance.selectedTower.gameObject)
            {
                thisRenderer.material.color = UpgradeManager.instance.selectedColor;
            }
            else
            {
                thisRenderer.material.color = defaultColor;
            }
        }

        private void OnMouseUp()
        {
            //make sure the player cant click on a node through the ui
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("TowerNode clicked.");

                //select the current tower on the node for upgrade
                if (currentTower != null)
                {
                    //Debug.Log("Can't build here! Turret already exists! Selecting turret instead!");
                    UpgradeManager.instance.SetSelectedTower(currentTower.GetComponent<Tower>());
                    if (!UpgradeManager.instance.menuRoot.gameObject.activeSelf)
                    {
                        UpgradeManager.instance.ToggleMenu();
                    }
                }
                //if we cant select the current tower, try to build a the selected tower to build
                else
                {
                    //if there is a selected tower
                    if (BuildingManager.instance.canBuild)
                    {
                        //if the player doesnt have enough money to build
                        if (!BuildingManager.instance.hasMoney)
                        {
                            Debug.Log("Not enough money!");
                            return;
                        }

                        //spawn a particle for feedback on building
                        GameObject be = Instantiate(buildEffect, transform.parent.position, transform.parent.rotation);
                        Destroy(be, 2f);

                        //build the tower and link its towerspot to the node
                        GameObject t = Instantiate(BuildingManager.instance.selectedTower, transform.parent.position, transform.parent.rotation);
                        t.GetComponent<Tower>().SetID();
                        t.GetComponent<Tower>().towerSpot = gameObject;

                        //pay for the tower
                        PlayerStats.instance.SpendMoney(BuildingManager.instance.selectedTower.GetComponent<Tower>().stats.cost);

                        //select the tower for upgrade
                        UpgradeManager.instance.SetSelectedTower(t.GetComponent<Tower>());
                        if (!UpgradeManager.instance.menuRoot.gameObject.activeSelf)
                        {
                            UpgradeManager.instance.ToggleMenu();
                        }

                        //reference the tower on its node
                        currentTower = t;
                    }
                }
            }
        }

    }
}
