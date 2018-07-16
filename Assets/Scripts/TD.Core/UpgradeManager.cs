using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TD.Core
{
    /// <summary>
    /// A class that handles the upgrades of towers. Attach this to your GameManager object in the scene.
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        //class reference so we can more easily reference the object
        public static UpgradeManager instance;
        //the currently selected tower
        public Tower selectedTower;
        //all our text elements
        public TMP_Text towerName, range, fireRate, damage, radius, rangeCostTxt, fireRateCostTxt, damageCostTxt, radiusCostTxt, sellTxt;
        //all our upgrade buttons, use this to modify interactability
        public Button rangeBTN, fireRateBTN, damageBTN, radiusBTN, sellBTN;
        //a reference to the top container of the shop to let us enable/disable the display
        public Transform menuRoot;

        public Color selectedColor; //the color when a tower is selected
        Color defaultColor; //the color to return to



        //modifiers for the scaling of the cost and stat when upgrading
        public float costMultiplier = 1.25f, upgradeMultiplier = 1.05f;
        //the return on an investment to a towers worth
        public float worthModifier = 2f / 3f;

        private void Awake()
        {
            //establish class reference
            if (instance != null)
            {
                Debug.Log("There is already an UpgradeManager!");
                Destroy(gameObject);
                return;
            }
            instance = this;

            //initialize default values into display
            UpdateText();

            //on start hide the upgrade menu
            if (menuRoot.gameObject.activeSelf)
            {
                menuRoot.gameObject.SetActive(false);
            }
        }

        void UpdateText()
        {
            //show default values if there is no tower selected
            if (selectedTower == null)
            {
                //stats display text
                towerName.text = "No tower selected";
                range.text = "Range: " + "N/A";
                fireRate.text = "Fire Rate: " + "N/A";
                damage.text = "Damage: " + "N/A";
                radius.text = "Radius: " + "N/A";

                //upgrade button text
                rangeCostTxt.text = "N/A";
                fireRateCostTxt.text = "N/A";
                damageCostTxt.text = "N/A";
                radiusCostTxt.text = "N/A";
                sellTxt.text = "N/A";

                //upgrade button interactability
                rangeBTN.interactable = false;
                fireRateBTN.interactable = false;
                damageBTN.interactable = false;
                radiusBTN.interactable = false;
                sellBTN.interactable = false;
            }
            else
            {
                //update the tower name
                towerName.text = selectedTower.stats.displayName; //+ " ("+selectedTower.idNum+")";

                //stats display text
                range.text = "Range: " + selectedTower.stats.range.ToString("0.00");
                fireRate.text = "Fire Rate: " + selectedTower.bulletStats.fireCooldown.ToString("0.00");
                damage.text = selectedTower.isLaser ? "DPS: " + selectedTower.laserStats.damageOverTime.ToString("0.00") : "Damage: " + selectedTower.bulletStats.damage.ToString("0.00");
                radius.text = "Radius: " + selectedTower.bulletStats.radius.ToString("0.00");

                //enable the sell button
                sellBTN.interactable = true;

                //update button text
                rangeCostTxt.text = "$" + selectedTower.stats.rangeCost.ToString("0.00");
                fireRateCostTxt.text = "$" + selectedTower.stats.fireRateCost.ToString("0.00"); ;
                damageCostTxt.text = "$" + selectedTower.stats.damageCost.ToString("0.00"); ;
                radiusCostTxt.text = "$" + selectedTower.stats.radiusCost.ToString("0.00");
                sellTxt.text = "Sell Tower: $" + selectedTower.stats.sellValue.ToString("0.00");

                //disable the range button if we upgrade to above our max range
                if (selectedTower.stats.range < selectedTower.stats.MAX_RANGE)
                {
                    rangeBTN.interactable = true;
                }

                //disable fire rate if the tower is a laser
                if (selectedTower.isLaser)
                {
                    fireRateBTN.interactable = false;
                    fireRate.text = "Fire Rate: " + "N/A";
                    fireRateCostTxt.text = "N/A";
                }
                //disable the fire rate button if the firerate is at or below the minimum
                else if (selectedTower.bulletStats.fireCooldown > selectedTower.stats.MIN_FIRERATE)
                {
                    fireRateBTN.interactable = true;
                }
                //disable the damage button if damage is at or above the max
                if (selectedTower.bulletStats.damage < selectedTower.stats.MAX_DAMAGE)
                {
                    damageBTN.interactable = true;
                }

                //disables the radius button and updates text if the tower has no radius
                if (selectedTower.bulletStats.radius <= 0)
                {
                    radiusCostTxt.text = "N/A";
                    radius.text = "Radius: " + "N/A";
                    radiusBTN.interactable = false;
                }
                //disables the button if the radius is max
                else if (selectedTower.bulletStats.radius < selectedTower.stats.MAX_RADIUS)
                {
                    radiusBTN.interactable = true;
                }

            }
        }

        /// <summary>
        /// Upgrade the towers damage based on its current stats
        /// </summary>
        public void UpgradeDamage()
        {
            //lasers use a different stat class, so upgrading requires different calls
            if (selectedTower.isLaser)
            {
                //if we can afford the upgrade
                if (PlayerStats.instance.GetMoney() >= selectedTower.stats.damageCost)
                {
                    //pay for the upgrade and update the towers sell value
                    PlayerStats.instance.SpendMoney((int)selectedTower.stats.damageCost);
                    selectedTower.GetComponent<Tower>().addWorth(selectedTower.stats.damageCost * worthModifier);

                    //update the lasers damage over time by our multiplier
                    selectedTower.laserStats.damageOverTime *= upgradeMultiplier;

                    //dont let the damage go over max
                    if (selectedTower.laserStats.damageOverTime >= selectedTower.stats.MAX_DAMAGE)
                    {
                        selectedTower.laserStats.damageOverTime = selectedTower.stats.MAX_DAMAGE;
                        damageBTN.interactable = false;
                    }

                    //update the cost of this towers damage upgrades
                    selectedTower.stats.damageCost *= costMultiplier;

                    //update the display text
                    UpdateText();
                }

            }
            else
            {
                //if we can afford the upgrade
                if (PlayerStats.instance.GetMoney() >= selectedTower.stats.damageCost)
                {
                    //pay for the upgrade and update the sell value
                    PlayerStats.instance.SpendMoney((int)selectedTower.stats.damageCost);
                    selectedTower.GetComponent<Tower>().addWorth(selectedTower.stats.damageCost * worthModifier);

                    //update the damage based on our multiplier
                    selectedTower.bulletStats.damage *= upgradeMultiplier;

                    //dont allow the tower damage to go above the max damage stat
                    if (selectedTower.bulletStats.damage >= selectedTower.stats.MAX_DAMAGE)
                    {
                        selectedTower.bulletStats.damage = selectedTower.stats.MAX_DAMAGE;
                        damageBTN.interactable = false;
                    }

                    //update the damage upgrade cost
                    selectedTower.stats.damageCost *= costMultiplier;

                    //update display text
                    UpdateText();
                }
            }
        }

        /// <summary>
        /// Upgrade the towers range based on its current stats
        /// </summary>
        public void UpgradeRange()
        {
            //if we can afford the upgrade
            if (PlayerStats.instance.GetMoney() >= selectedTower.stats.rangeCost)
            {
                //pay for the upgrade and update sell value
                PlayerStats.instance.SpendMoney((int)selectedTower.stats.rangeCost);
                selectedTower.GetComponent<Tower>().addWorth(selectedTower.stats.rangeCost * worthModifier);

                //upgrade the range and make sure it doesnt go over max
                selectedTower.stats.range *= upgradeMultiplier;
                if (selectedTower.stats.range >= selectedTower.stats.MAX_RANGE)
                {
                    selectedTower.stats.range = selectedTower.stats.MAX_RANGE;
                    rangeBTN.interactable = false;
                }

                //update the cost
                selectedTower.stats.rangeCost *= costMultiplier;

                //uppdate the display
                UpdateText();
            }
        }

        /// <summary>
        /// Upgrade the towers radius based on its current stats
        /// </summary>
        public void UpgradeRadius()
        {
            //if we can afford the upgrade
            if (PlayerStats.instance.GetMoney() >= selectedTower.stats.radiusCost)
            {
                //pay for the upgrade and update the sell value
                PlayerStats.instance.SpendMoney((int)selectedTower.stats.radiusCost);
                selectedTower.GetComponent<Tower>().addWorth(selectedTower.stats.radiusCost * worthModifier);

                //upgrade the radius and make sure it doesnt go above max
                selectedTower.bulletStats.radius *= upgradeMultiplier;
                if (selectedTower.bulletStats.radius >= selectedTower.stats.MAX_RADIUS)
                {
                    selectedTower.bulletStats.radius = selectedTower.stats.MAX_RADIUS;
                    radiusBTN.interactable = false;
                }

                //update the cost of radius upgrades
                selectedTower.stats.radiusCost *= costMultiplier;

                //upgrade the display
                UpdateText();
            }
        }

        /// <summary>
        /// Upgrade the towers rate of fire based on its current stats
        /// </summary>
        public void UpgradeFireRate()
        {
            //if we can afford the upgrade
            if (PlayerStats.instance.GetMoney() >= selectedTower.stats.fireRateCost)
            {
                //pay for the upgrade and update the sell value
                PlayerStats.instance.SpendMoney((int)selectedTower.stats.fireRateCost);
                selectedTower.GetComponent<Tower>().addWorth(selectedTower.stats.fireRateCost * worthModifier);

                //upgrade the stat and make sure it doesnt go below minimum fire rate
                selectedTower.bulletStats.fireCooldown *= (1 / upgradeMultiplier);
                if (selectedTower.bulletStats.fireCooldown <= selectedTower.stats.MIN_FIRERATE)
                {
                    selectedTower.bulletStats.fireCooldown = selectedTower.stats.MIN_FIRERATE;
                    fireRateBTN.interactable = false;
                }

                //update the upgrade cost
                selectedTower.stats.fireRateCost *= costMultiplier;

                //update the display
                UpdateText();
            }
        }


        /// <summary>
        /// trigger on the sell button to start the tower selling process
        /// </summary>
        public void SellTower()
        {
            //call on the tower to sell itself
            selectedTower.Sell();
            //break the link to the selected tower
            selectedTower = null;
            //update display text to defaults
            UpdateText();
            //disable the menu
            ToggleMenu();
        }

        /// <summary>
        /// Handles selecting a new tower for upgrade
        /// </summary>
        /// <param name="t">The tower to select</param>
        public void SetSelectedTower(Tower t)
        {
            if (t != null)
            {
                //update default to new towers color
                defaultColor = t.towerSpot.GetComponent<TowerNode>().defaultColor;

                //update selected tower to new tower and update base to show selected
                if (selectedTower != null)
                {
                    selectedTower.towerSpot.GetComponent<Renderer>().material.color = defaultColor;
                }
                this.selectedTower = t;
                selectedTower.towerSpot.GetComponent<Renderer>().material.color = selectedColor;

                //update stat text
                UpdateText();
            }
        }

        /// <summary>
        /// Toggle the upgrade menu display state
        /// </summary>
        public void ToggleMenu()
        {
            menuRoot.gameObject.SetActive(!menuRoot.gameObject.activeSelf);
        }
    }
}