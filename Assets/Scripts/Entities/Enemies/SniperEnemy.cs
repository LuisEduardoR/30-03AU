using UnityEngine;

// Código de um inimigo que persegue o player (no eixo-x) carrega o tiro e dispara.
[AddComponentMenu("Scripts/Enemies/Sniper Enemy")]
public class SniperEnemy : EnemyBase {

    // Grupos de armas, quando um inimigo atira ele atira com todas as armas de um grupo.
    [System.Serializable]
    public struct WeaponGroup {

        // As armas que serão disparadas.
        public WeaponBase[] weapons;

    }
    [Header("Weapons:")]
    [Space(3)]
    // Guarda os grupos de armas do inimigo.
    public WeaponGroup[] weaponGroups;

    private bool aimShot = false;

    public float chargingDuration = 1.0f;
    private float _chargingTime = 0;

    public float fireDuration = 1.0f;

    [Space(10)]
    public LineRenderer aimEffect;
    public Transform aimSource;

    [Space(10)]
    // Máscara usada para ignorar colisão do Raycast com objetos que não o player ao fazer o efeito de mira.
    public LayerMask raycastMask;

    [Space(10)]
    public AudioSource chargingSound;

    public override void Start() {

        base.Start();

    }

    public override void Update() {

        base.Update();

        // Para de executar o código do inimigo caso o jogo esteja pausado.
        if (GameController.gameController._currentState == GameController.GameState.Pause)
            return;

        if (_activated && paralisedTime <= 0) {

            if (GameController.player != null && !aimShot && (transform.position.y - GameController.player.transform.position.y) >= 0) {

                float deltaPos = transform.position.x - GameController.player.transform.position.x;

                transform.Translate(transform.InverseTransformDirection(new Vector3(-deltaPos * velocity * Time.deltaTime, -velocity * Time.deltaTime, 0)));

                if (Mathf.Abs(deltaPos) <= 0.2f) {

                    aimShot = true;
                    aimEffect.enabled = true;
                    chargingSound.Play();

                }
            } else if(!aimShot){

                transform.Translate(transform.InverseTransformDirection(new Vector3( 0, -velocity * Time.deltaTime, 0)));

            }

            if (aimShot) {

                _chargingTime += Time.deltaTime;

                if (_chargingTime >= chargingDuration) {

                    EnemyFire();
                    aimEffect.enabled = false;
                    chargingSound.Stop();

                }
                else {

                    RaycastHit2D hit = new RaycastHit2D();

                    hit = Physics2D.Raycast(aimSource.position, -Vector2.up, GameController.gameController.screenSize.y * 3, raycastMask);

                    if (hit.collider != null) {

                        // Posiciona o efeito gráfico do laser.
                        aimEffect.SetPosition(0, aimSource.position);
                        aimEffect.SetPosition(1, hit.point - new Vector2(0, 0.25f));

                    } else {

                        // Posiciona o efeito gráfico do laser.
                        aimEffect.SetPosition(0, aimSource.position);
                        aimEffect.SetPosition(1, aimSource.position - new Vector3(0, GameController.gameController.screenSize.y * 3, 0));

                    }
                }

                if (_chargingTime >= (chargingDuration + fireDuration)) {

                    _chargingTime = 0;
                    aimShot = false;

                }
            }
        }
    }

    public virtual void EnemyFire() {

        if (weaponGroups.Length != 0) {

            for (int i = 0; i < weaponGroups[0].weapons.Length; i++) {
                weaponGroups[0].weapons[i].Fire();
            }
        }
    }

    public override void Damage(int amount, float paralyse) {

        // Garante que o inimigo não leve dano antes de aparecer na tela.
        if (_activated)
            base.Damage(amount, paralyse);

    }
}
