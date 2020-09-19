using UnityEngine;
using UnityEngine.UI;

// Código que controla o paínel de uma arma na loja.
[AddComponentMenu("Scripts/Utility/Weapon Panel")]
public class WeaponPanel : MonoBehaviour {

    // Loja que é "dona" deste paínel.
    [HideInInspector]
    public Store store;
    // Número do paínel. Começa no 0.
    [HideInInspector]
    public int code;

    public GameObject _gameObject;
    public Weapon weapon;

    public Image icon;

    public Text displayName;
    public Text cost;

    public Button buyButton;
    public Button equipButton;
    public Button sellButton;

    // Dano e delay.
    public Text stats_0;
    // Nº de projéteis e DPS.
    public Text stats_1;

    public void Buy() {

        store.BuyWeapon(weapon.idName, code);

    }

    public void Equip() {

        store.EquipWeapon(weapon.idName, code);

    }

    public void Sell() {

        store.SellWeapon(weapon.idName, code);

    }

}