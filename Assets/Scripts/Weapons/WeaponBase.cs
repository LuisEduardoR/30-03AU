using UnityEngine;

// Esse código serve como base para todas as armas.
[AddComponentMenu("Scripts/Weapons/Weapon Base")]
public class WeaponBase : MonoBehaviour {

    // Projétil disparado pela arma.
    public GameObject projectile;
    // Locais onde esse projétil aparecera. 
    public Transform[] projectile_spawn;

    // Quando verdadeiro alterna entre os spaws de projéteis, quando falso atira com todos os spawns ao mesmo tempo.
    public bool alternateSpawns = true;

    // Guarda o spawn de projéteis atual;
    [HideInInspector]
    public int current_projectile_spawn = 0;

    // Delay entre os tiros.
    public float delayBetweenShots;
    // Conta o tempo do delay.
    [HideInInspector]
    public float delayShots = 0;

    // Faz com que essa arma ignore a munição munição.
    public bool infinite_ammo;
    // Guarda a quantidade máxima de munição dessa arma.
    public int maxAmmo = 10;
    // Guarda a munição atual.
    public int currentAmmo = 10;

    // As fontes em que o som do tiro será tocado, elas já devem estar pré-preparadas com o clip de áudio, são usadas várias para que armas que disparam muito rápido não 
    // sejam cortados no meio de um som. Elas são tocadas em sequência. Também podem ser usadas para várias os clips de áudio.
    public AudioSource[] _audio;
    // A fonte de áudio a ser tocada em seguida.
    [HideInInspector]
    public int nextAudioSource = 0;

    public virtual void Update() {

        // Reseta o delay para disparar.
        if (delayShots > delayBetweenShots)
            delayShots = 0;

        // Conta o tempo do delay para disparar.
        if(delayShots != 0)
            delayShots += Time.deltaTime;

        // Garante que nunca haja mais munição que o máximo.
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

    }

    // Faz o disparo.
	public virtual void Fire () {

        // Dispara caso o delay já tenha passado, e haja  munição/ela seja infinita.
        if(delayShots == 0 && currentAmmo > 0 || delayShots == 0 && infinite_ammo) {

            // Retira a munição caso ela não seja infinita.
            if(!infinite_ammo)
                currentAmmo--; 

            // Toca o áudio na fonte atual e passa para a próxima, dando loop ao chegar ao fim da lista.
            if (nextAudioSource < _audio.Length) {
                _audio[nextAudioSource].Play();
            } else if(_audio.Length > 0) {
                nextAudioSource = 0;
                _audio[nextAudioSource].Play();
            }

            if(!alternateSpawns) // Atira com projéteis de todos os spawns.
                for (int i = 0; i < projectile_spawn.Length; i++)
                    Instantiate( projectile, projectile_spawn[i].position, projectile_spawn[i].rotation);
            else { // Alterna entre os spawns de projéteis.

                Instantiate(projectile, projectile_spawn[current_projectile_spawn].position, projectile_spawn[current_projectile_spawn].rotation);

                current_projectile_spawn++;
                if (current_projectile_spawn >= projectile_spawn.Length)
                    current_projectile_spawn = 0;

            }

            // Começa a contar o delay entre os tiros.
            delayShots += Time.deltaTime;
        }
	}

    // Pega o tipo e a quantidade de munição da arma.
    public void InitializeAmmo(string _type) {

        if (PlayerPrefs.HasKey("ammo_" + _type))
            currentAmmo = PlayerPrefs.GetInt("ammo_" + _type);

    }
}
