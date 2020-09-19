using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerInput))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

// Código básico para o player.
[AddComponentMenu("Scripts/Entities/Player/Main")]
public class Player : EntityBase {

    // Usado para facilitar a implementação de controles via interfaces touch.
    public struct P_Input {

        public float x_axis, y_axis;
        public bool pauseKey;

    }
    public P_Input p_input;

    // Guarda o ScriptableObject com informações sobre s arma primária do player.
    public Weapon primaryWeapon;
    // Guarda o script da arma primária do player após ela ser instanciada.
    [HideInInspector]
    public WeaponBase primaryWeaponScript;

    // Guarda os ScriptableObjects com informações sobre as armas secundárias do player.
    public List<Weapon> secondaryWeapons;
    // Guarda os scripts das armas secundárias do player após elas serem instanciadas.
    [HideInInspector]
    public WeaponBase[] secondaryWeaponScripts;

    // Guarda a vida máxima do player no momento.
    [HideInInspector]
    public int maxHull;

	public override void Start () {

        base.Start();

        maxHull = PlayerPrefs.GetInt("maxHull");

        hull = Mathf.Clamp(PlayerPrefs.GetInt("currentHull"), 0, maxHull);
        GameController.gameController.UpdateHealthBar();

    }

    public override void Update () {

        base.Update();

        // Para de executar o código do player caso o jogo esteja pausado.
        if (GameController.gameController._currentState == GameController.GameState.Pause)
            return;


        if (p_input.pauseKey) {
            GameController.gameController.PauseGame();
        }

        
        if (paralisedTime <= 0) { // Impede que o controle do player se ele estiver paralisado

            // Movimenta o player, mantendo-o dentro dos limites da tela.
            Vector3 new_pos = transform.position;
            if (p_input.x_axis != 0 || p_input.y_axis != 0)
                new_pos += new Vector3(p_input.x_axis * Time.deltaTime * velocity, p_input.y_axis * Time.deltaTime * velocity, 0);

            transform.position = new Vector3(Mathf.Clamp(new_pos.x, -GameController.gameController.screenSize.x, GameController.gameController.screenSize.x), Mathf.Clamp(new_pos.y, -GameController.gameController.screenSize.y, GameController.gameController.screenSize.y), 0);

            // Verifica se alguma das armas está sendo atirada.
            if (Input.GetKey(primaryWeapon.fireKey))
                primaryWeaponScript.Fire();
            

            for (int i = 0; i < secondaryWeapons.Count; i++)
                if (Input.GetKey(secondaryWeapons[i].fireKey)) {
                    secondaryWeaponScripts[i].Fire();
                }
        }
    }

    // (Chamado pelo GameController).
    public void CreateWeapons() {

        Transform new_pri_weapon = Instantiate(primaryWeapon.weaponObject, transform.position + primaryWeapon.position, new Quaternion(transform.rotation.x + primaryWeapon.rotation.x, transform.rotation.y + primaryWeapon.rotation.y, transform.rotation.z + primaryWeapon.rotation.z, transform.rotation.z + primaryWeapon.rotation.z)).transform;
        new_pri_weapon.parent = this.transform;

        primaryWeaponScript = new_pri_weapon.GetComponent<WeaponBase>();

        if (primaryWeapon.ammoName != "none")
            primaryWeaponScript.InitializeAmmo(primaryWeapon.ammoName);

        secondaryWeaponScripts = new WeaponBase[secondaryWeapons.Count];
        for (int i = 0; i < secondaryWeapons.Count; i++) {

            Transform new_sec_weapon = Instantiate(secondaryWeapons[i].weaponObject, transform.position + secondaryWeapons[i].position, new Quaternion(transform.rotation.x + secondaryWeapons[i].rotation.x, transform.rotation.y + secondaryWeapons[i].rotation.y, transform.rotation.z + secondaryWeapons[i].rotation.z, transform.rotation.z + secondaryWeapons[i].rotation.z)).transform;
            new_sec_weapon.parent = this.transform;

            secondaryWeaponScripts[i] = new_sec_weapon.GetComponent<WeaponBase>();

            if(secondaryWeapons[i].ammoName != "none")
                secondaryWeaponScripts[i].InitializeAmmo(secondaryWeapons[i].ammoName);
        }

    }

    public bool Heal(int amount) {

        // Valor de retorno usado pelos PowerUp's.
        if (hull >= maxHull) {
            return false;
        } else {
            hull += amount;
            if (hull >= maxHull)
                hull = maxHull;

            GameController.gameController.UpdateHealthBar();

            return true;
        }

    }

    public bool AddAmmo(int ammo, string ammoType) {

        // Encontra a arma apropriada para a munição.
        for (int i = 0; i < secondaryWeapons.Count; i++)
            if(secondaryWeapons[i].ammoName == ammoType) {

                // Valor de retorno usado pelos PowerUp's.
                if (secondaryWeaponScripts[i].currentAmmo >= secondaryWeaponScripts[i].maxAmmo)
                    return false;
                else {

                    secondaryWeaponScripts[i].currentAmmo += ammo;

                    if (secondaryWeaponScripts[i].currentAmmo >= secondaryWeaponScripts[i].maxAmmo)
                        secondaryWeaponScripts[i].currentAmmo = secondaryWeaponScripts[i].maxAmmo;

                    return true;
                }
            }

        // Retorna falso caso nenhuma arma com essa munição seja encontrada e gera um aviso.
        Debug.LogWarning("(Player) No weapon using <" + ammoType + "> as ammo!");
        return false;
    }

    public override void Damage(int amount, float paralyse) {

        base.Damage(amount, paralyse);

        GameController.gameController.UpdateHealthBar();
    }

    public override void Die() {

        GameController.gameController.LoseLevel();

        base.Die();

    }
}
