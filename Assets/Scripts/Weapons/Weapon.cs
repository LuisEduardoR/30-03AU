using UnityEngine;

// Objeto que guarda as informações sobre uma determinada arma.
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject {

    // Armas principais só podem ser equipadas uma de cada vez, armas secundárias estão equipadas o tempo todo.
    public bool mainWeapon = true;

    public string displayName = "New Weapon";

    // O nome da arma que será usado pelo código do jogo.
    public string idName = "NewWeapon";

    public int cost = 1000;
    public int ammoCost = 100;

    public string fireKey = "space";

    public GameObject weaponObject;

    // Posição da arma relativa ao player.
    public Vector3 position;
    public Vector3 rotation;

    // Nome utilizado para se referir ao tipo de munição desta arma ("none" quer dizer que a arma não usa munição).
    public string ammoName = "none";

    public int defaultAmmo = 5;

    [System.Serializable]
    public struct UI {

        public Sprite inGameUISprite;

        public Sprite storeUISprite;

        public Sprite ammoUISprite;

    }
    public UI _ui;

    // Informações demonstradas na loja.
    [System.Serializable]
    public struct DisplayInfo {

        public int damage;
        public float delay;
        public int projectileAmount;

    }
    public DisplayInfo displayInfo;
}
