using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Este código controla a loja.
[AddComponentMenu("Scripts/Utility/Store")]
public class Store : MonoBehaviour {

    [Header("Health:")]
    public Button maxHealButton;
    public Button heal10Button;

    public Text healthAmount;
    public Image healthBar;

    public Text fullHealText;
    public Text heal10Text;

    // Custo para uma unidade de vida.
    public int hullCost = 10;
    // Custo para aumentar a vida do player em 10.
    public int hullUpgradeCost = 1000;

    public Button upgradeHealthButton;
    public Text upgradeHealthText;

    [Header("Ammo:")]
    [Space(3)]
    // Prefab padrão do paínel de um tipo de munição.
    public GameObject ammoTypePanelObj;
    // Posição base para o posicionamento dos paíneis de munição, também serve como "pai" para o transform delas.
    public Transform ammoTypePosition;
    private List<AmmoTypePanel> ammoTypePanels;

    [Header("Weapons:")]
    [Space(3)]
    // Prefab padrão do paínel de uma arma.
    public GameObject weaponPanelObj;
    // Posição base para o posicionamento dos paíneis das armas, também serve como "pai" para o transform delas.
    public Transform weaponPanelPosition;
    private List<WeaponPanel> weaponPanels;

    public Scrollbar weaponScrollbar;

    [Header("Credits:")]
    [Space(3)]
    public Text creditsText;
    
    void OnEnable () {

        // Gera os paineis com as munições e as armas caso eles ainda não existam.
        if (weaponPanels == null || ammoTypePanels == null || weaponPanels.Count == 0 || ammoTypePanels.Count == 0) {

            ammoTypePanels = new List<AmmoTypePanel>();
            weaponPanels = new List<WeaponPanel>();

            for (int i = 0; i < GameController.gameController.avaliableWeapons.Length; i++) {

                // Configura as munições que serão vendiadas pela loja.
                if (GameController.gameController.avaliableWeapons[i].ammoName != "none") {

                    GameObject ammoTypeObj = Instantiate(ammoTypePanelObj);
                    AmmoTypePanel ammoTypeScript = ammoTypeObj.GetComponent<AmmoTypePanel>();
                    RectTransform ammoTransform = ammoTypeObj.GetComponent<RectTransform>();

                    ammoTransform.SetParent(ammoTypePosition);
                    ammoTransform.localPosition = new Vector3(0, -65 * ammoTypePanels.Count, 0);

                    ammoTypeScript.code = ammoTypePanels.Count;

                    ammoTypeScript.id = GameController.gameController.avaliableWeapons[i].ammoName;
                    ammoTypeScript.cost = GameController.gameController.avaliableWeapons[i].ammoCost;
                    ammoTypeScript.max = GameController.gameController.avaliableWeapons[i].weaponObject.GetComponent<WeaponBase>().maxAmmo;
                    ammoTypeScript.icon = GameController.gameController.avaliableWeapons[i]._ui.ammoUISprite;

                    ammoTypeScript.store = this;

                    ammoTypePanels.Add(ammoTypeScript);

                }

                // Configura as armas que serão vendiadas pela loja.
                if (GameController.gameController.avaliableWeapons[i].mainWeapon) {


                    GameObject panelObj = Instantiate(weaponPanelObj);
                    WeaponPanel panelScript = panelObj.GetComponent<WeaponPanel>();
                    RectTransform panelTransform = panelObj.GetComponent<RectTransform>();

                    panelTransform.SetParent(weaponPanelPosition);
                    panelTransform.localPosition = new Vector3(-440 + (440 * (weaponPanels.Count)), -20, 0);

                    panelScript.icon.sprite = GameController.gameController.avaliableWeapons[i]._ui.storeUISprite;
                    panelScript.displayName.text = GameController.gameController.avaliableWeapons[i].displayName;

                    panelScript.weapon = GameController.gameController.avaliableWeapons[i];
                    panelScript.code = weaponPanels.Count;

                    panelScript.stats_0.text = "<color=red>Dano:</color> " + GameController.gameController.avaliableWeapons[i].displayInfo.damage +
                                               "\n<color=cyan>Cadência:</color> " + (1 / GameController.gameController.avaliableWeapons[i].displayInfo.delay).ToString("F2");

                    panelScript.stats_1.text = "<color=green>Nº de Projéteis:</color> " + GameController.gameController.avaliableWeapons[i].displayInfo.projectileAmount +
                                               "\n<color=orange>DPS:</color> " + ((GameController.gameController.avaliableWeapons[i].displayInfo.damage *
                                               GameController.gameController.avaliableWeapons[i].displayInfo.projectileAmount) / GameController.gameController.avaliableWeapons[i].displayInfo.delay).ToString("F2");

                    panelScript.store = this;

                    weaponPanels.Add(panelScript);
                    
                }
            }
        }

        UpdateStoreUI();

        // Seta a scrollbar baseado na quantidade de armas a venda na loja.
        weaponScrollbar.value = 0;
        if (weaponPanels.Count > 2) {

            weaponScrollbar.size = 1.0f / (weaponPanels.Count - 1);
            weaponScrollbar.numberOfSteps = weaponPanels.Count - 1;

            weaponScrollbar.gameObject.SetActive(true);
        }
        else {
            weaponScrollbar.gameObject.SetActive(false);
        }
        

    }

    public void MaxHeal() {

        int cr = 0;
        int hull = 0;
        int maxHull = 100;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        if (PlayerPrefs.HasKey("currentHull"))
            hull = PlayerPrefs.GetInt("currentHull");

        if (PlayerPrefs.HasKey("maxHull"))
            maxHull = PlayerPrefs.GetInt("maxHull");

        if (maxHull - hull > 0 && cr >=  (maxHull - hull) * hullCost) {

            PlayerPrefs.SetInt("currentHull", maxHull);
            cr -= (maxHull - hull) * hullCost;

            PlayerPrefs.SetInt("credits", cr);

            UpdateStoreUI();
        }

    }

    public void Heal10() {

        int cr = 0;
        int hull = 0;
        int maxHull = 100;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        if (PlayerPrefs.HasKey("currentHull"))
            hull = PlayerPrefs.GetInt("currentHull");

        if (PlayerPrefs.HasKey("maxHull"))
            maxHull = PlayerPrefs.GetInt("maxHull");

        if (maxHull - hull >= 10 && cr >= 10 * hullCost) {

            PlayerPrefs.SetInt("currentHull", hull + 10);
            cr -= 10 * hullCost;

            PlayerPrefs.SetInt("credits", cr);

            UpdateStoreUI();
        }

    }

    public void UpgradeHealth() {

        int cr = 0;
        int hull = 0;
        int maxHull = 100;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        if (PlayerPrefs.HasKey("currentHull"))
            hull = PlayerPrefs.GetInt("currentHull");

        if (PlayerPrefs.HasKey("maxHull"))
            maxHull = PlayerPrefs.GetInt("maxHull");

        if (cr >= hullUpgradeCost) {

            PlayerPrefs.SetInt("currentHull", hull + 10);

            PlayerPrefs.SetInt("maxHull", maxHull + 10);
            cr -= hullUpgradeCost;

            PlayerPrefs.SetInt("credits", cr);

            UpdateStoreUI();
        }

    }

    public void BuyAmmo(string id, int code) {

        int cr = 0;
        int ammo = 0;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        if (PlayerPrefs.HasKey("ammo_" + id))
            ammo = PlayerPrefs.GetInt("ammo_" + id);

        if (cr >= ammoTypePanels[code].cost && ammo < ammoTypePanels[code].max) {

            PlayerPrefs.SetInt("ammo_" + id, ammo + 1);
            cr -= ammoTypePanels[code].cost;

            PlayerPrefs.SetInt("credits", cr);

            UpdateStoreUI();
        }

    }

    public void BuyWeapon (string id, int code) {

        int cr = 0;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        if(cr >= weaponPanels[code].weapon.cost) {

            PlayerPrefs.SetInt(id, 1);
            cr -= weaponPanels[code].weapon.cost;

            PlayerPrefs.SetInt("credits", cr);

            UpdateStoreUI();
        }

    }

    public void EquipWeapon (string id, int code) {

        if (PlayerPrefs.HasKey(id))
            if(PlayerPrefs.GetInt(id) == 1) {

                for (int i = 0; i < weaponPanels.Count; i++) {

                    if (PlayerPrefs.HasKey(weaponPanels[i].weapon.idName))
                        if (PlayerPrefs.GetInt(weaponPanels[i].weapon.idName) == 2) {
                            PlayerPrefs.SetInt(weaponPanels[i].weapon.idName, 1);
                        }
                }

                PlayerPrefs.SetInt(id, 2);

                UpdateStoreUI();
            }

    }

    public void SellWeapon (string id, int code) {

        if (PlayerPrefs.HasKey(id))
            if (PlayerPrefs.GetInt(id) == 1) {

                PlayerPrefs.SetInt(id, 0);

                int cr = 0;
                if (PlayerPrefs.HasKey("credits"))
                    cr = PlayerPrefs.GetInt("credits");

                cr += weaponPanels[code].weapon.cost;
                PlayerPrefs.SetInt("credits", cr);

                UpdateStoreUI();
            }

    }

    public void UpdateScrollBar () {

        for (int i = 0; i < weaponPanels.Count; i++) {

            weaponPanels[i]._gameObject.transform.localPosition = new Vector3(-440 + (440 * i) - (440 * (weaponPanels.Count - 2) * weaponScrollbar.value), -20, 0);

        }

    }

    public void UpdateStoreUI() {

        // Créditos:
        int cr = 0;

        if (PlayerPrefs.HasKey("credits"))
            cr = PlayerPrefs.GetInt("credits");

        creditsText.text = "Cr$ " + cr;

        // Itens relacionados à vida:
        int hull = 0;
        int maxHull = 100;

        if (PlayerPrefs.HasKey("currentHull"))
            hull = PlayerPrefs.GetInt("currentHull");

        if (PlayerPrefs.HasKey("maxHull"))
            maxHull = PlayerPrefs.GetInt("maxHull");

        // - Atualiza os botões relacionados à vida.
        fullHealText.text = "Cr$ " + (maxHull - hull) * hullCost;
        heal10Text.text = "Cr$ " + 10 * hullCost;

        if (hull < maxHull)
            maxHealButton.interactable = true;
        else
            maxHealButton.interactable = false;

        if (maxHull - hull >= 10)
            heal10Button.interactable = true;
        else
            heal10Button.interactable = false;

        upgradeHealthText.text = "Cr$ " + hullUpgradeCost;

        if (cr >= 10 * hullCost)
            upgradeHealthButton.interactable = true;
        else
            upgradeHealthButton.interactable = false;

        // - Atualiza a barra de vida.
        healthAmount.text = hull + "/" + maxHull;
        healthBar.fillAmount = (float)hull / (float)maxHull;

        if ((float)hull / (float)maxHull <= 0.5f && (float)hull / (float)maxHull > 0.25f) {
            healthAmount.color = Color.yellow;
            healthBar.color = Color.yellow;
        }
        else if ((float)hull / (float)maxHull <= 0.25f) {
            healthAmount.color = Color.red;
            healthBar.color = Color.red;
        }
        else {
            healthAmount.color = Color.green;
            healthBar.color = Color.green;
        }

        // Tipos de munições:
        for (int j = 0; j < ammoTypePanels.Count; j++) {

            int ammo = 0;

            if (PlayerPrefs.HasKey("ammo_" + ammoTypePanels[j].id)) {

                ammo = PlayerPrefs.GetInt("ammo_" + ammoTypePanels[j].id);

                if (ammo == 0)
                    ammoTypePanels[j].amountText.text = "<color=red>" + ammo + "/" + ammoTypePanels[j].max + "</color>";
                else if (ammo >= ammoTypePanels[j].max)
                    ammoTypePanels[j].amountText.text = "<color=lime>" + ammo + "/" + ammoTypePanels[j].max + "</color>";
                else
                    ammoTypePanels[j].amountText.text = ammo + "/" + ammoTypePanels[j].max;

                if (cr >= ammoTypePanels[j].cost && ammo < ammoTypePanels[j].max)
                    ammoTypePanels[j].buy_button.interactable = true;
                else
                    ammoTypePanels[j].buy_button.interactable = false;

            }
            else {
                Debug.Log("(Store) No PlayerPrefs entry found for ammo_" + ammoTypePanels[j].id + "!");
            }

            ammoTypePanels[j].costText.text = "Cr$ " + ammoTypePanels[j].cost;
            ammoTypePanels[j].iconImage.sprite = ammoTypePanels[j].icon;

        }

        // Armas:
        for (int i = 0; i < weaponPanels.Count; i++) {

            if (PlayerPrefs.HasKey(weaponPanels[i].weapon.idName)) {

                // 0 = não comprado, 1 = comprado, 2 = equipado.
                int status = PlayerPrefs.GetInt(weaponPanels[i].weapon.idName);

                if (status == 0) {

                    weaponPanels[i].cost.text = "Cr$ " + weaponPanels[i].weapon.cost;

                    weaponPanels[i].buyButton.gameObject.SetActive(true);

                    if (cr >= weaponPanels[i].weapon.cost)
                        weaponPanels[i].buyButton.interactable = true;
                    else
                        weaponPanels[i].buyButton.interactable = false;

                    weaponPanels[i].equipButton.gameObject.SetActive(false);
                    weaponPanels[i].sellButton.gameObject.SetActive(false);

                } else if (status == 1) {

                    weaponPanels[i].cost.text = "<color=cyan>Comprado</color>";

                    weaponPanels[i].buyButton.gameObject.SetActive(false);
                    weaponPanels[i].equipButton.gameObject.SetActive(true);
                    weaponPanels[i].equipButton.interactable = true;
                    weaponPanels[i].sellButton.gameObject.SetActive(true);
                    weaponPanels[i].sellButton.interactable = true;

                } else {

                    weaponPanels[i].cost.text = "<color=magenta>Equipado</color>";

                    weaponPanels[i].buyButton.gameObject.SetActive(false);
                    weaponPanels[i].equipButton.gameObject.SetActive(true);
                    weaponPanels[i].equipButton.interactable = false;
                    weaponPanels[i].sellButton.gameObject.SetActive(true);
                    weaponPanels[i].sellButton.interactable = false;

                }

            }
            else {

                Debug.Log("(Store) No PlayerPrefs entry detected for " + weaponPanels[i].weapon.idName + "!");

            }
        }
    }
}
