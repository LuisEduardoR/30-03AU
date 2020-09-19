using UnityEngine;
using UnityEngine.UI;

// Código que controla o paínel de um tipo de munição na loja.
[AddComponentMenu("Scripts/Utility/Ammo Type Panel")]
public class AmmoTypePanel : MonoBehaviour {

    // Loja que é "dona" deste tipo de munição.
    [HideInInspector]
    public Store store;
    // Número do tipo de munição. Começa no 0.
    [HideInInspector]
    public int code;

    public GameObject _gameObject;

    public string id;
    public int max;
    public int cost;

    public Sprite icon;

    public Button buy_button;
    public Text amountText;

    public Text costText;

    public Image iconImage;

    public void Buy() {

        store.BuyAmmo(id, code);

    }
}
