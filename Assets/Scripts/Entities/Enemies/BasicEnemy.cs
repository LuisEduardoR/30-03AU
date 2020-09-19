using System.Collections.Generic;
using UnityEngine;

// Código de um inimigo básico que anda e atira.
[AddComponentMenu("Scripts/Enemies/Basic Enemy")]
public class BasicEnemy : EnemyBase {

    private float maxHull;

    // Padrões de movimento do inimigo.
    [System.Serializable]
    public class MovementPattern {

        // 0 = executar o padrão para sempre.
        public float duration = 0.0f;

        [Space(5)]
        public Vector3 movDirection = new Vector3( 0, -1, 0);

        [Space(5)]
        public bool smoothMovement = false;

        [Space(5)]
        public bool allowFire = true;

        [Space(5)]
        public bool repeatAlways = true; // Faz com que o movimento sempre se repita, ignorando a variável de baixo, pelo menos uma padrão por objeto deve ter essa variável marcada, ou ter uma de suas
                                         // "durations" = 0.
        public int repeatNTimes = 0; // Quantas vezes o movimento vai se repetir antes de sair da lista.
    }

    [Header("Movimentation:")]
    [Space(3)]
    // Lista dos movimentos que devêm ser realizados.
    public List<MovementPattern> movementPattern;

    private bool movementAllowFire = true;
    
    // Guardam o padrão de movimento atual e o tempo que o inimigo deve permanecer nesse padrão.
    private int mPstep;
    private float mPtime;

    // Modos em que o inimigo pode revidar um tiro: Só no primeiro dano; Aleatoriamente; Sempre; Nunca, respectivamente.
    public enum FireBackMode {
        FireBackOnlyFirst, RandomFireBack, AlwaysFireback, DontFireBack
    }
    [Header("Weapons:")]
    [Space(3)]
    public FireBackMode fireBackMode = FireBackMode.RandomFireBack;

    [Range(0, 1)]
    public float randomFirebackChance = 0.5f;

    // Modos em que o inimigo pode atirar normalmente: Aleatoriamente com mais chance quando o player está em frente; Aleatoriamente; Nunca, respectivamente.
    public enum RegularFireMode {
        TargetRandomAndRandomFire, OnlyRandomFire, DontRegularFire
    }
    [Space(5)]
    public RegularFireMode regularFireMode = RegularFireMode.TargetRandomAndRandomFire;

    // Distância máxima para que o player seja considerado em frente.
    public float distanceToTargetShoot = 4.0f;

    [Range(0, 1)]
    public float randomTargetFireChance = 0.75f;
    [Range(0, 1)]
    public float randomRegFireChance = 0.5f;

    // Grupos de armas, quando um inimigo atira ele atira com todas as armas de um grupo.
    [System.Serializable]
    public struct WeaponGroup {

        // As armas que serão disparadas.
        public WeaponBase[] weapons;

    }
    [Space(5)]
    public WeaponGroup[] weaponGroups;


    private bool playerIsInSight = false;

    [Space(10)]
    // Máscara usada para ignorar colisão do Raycast com objetos que não o player ao checar se ele está em frente ao inimigo.
    public LayerMask raycastMask;

    public override void Start() {

        base.Start();

        movementAllowFire = movementPattern[mPstep].allowFire;

        maxHull = hull;

    }

    public override void Update() {

        base.Update();

        // Para de executar o código do inimigo caso o jogo esteja pausado.
        if (GameController.gameController._currentState == GameController.GameState.Pause)
            return;

        if (_activated && paralisedTime <= 0) {

            // Circula pelos vários padrões de movimento.

            if (movementPattern[mPstep].duration != 0) {
                // Conta o tempo para o trocar de padrão.
                mPtime += Time.deltaTime;

                // Troca o padrão.
                if (mPtime > movementPattern[mPstep].duration) {

                    mPtime = 0;

                    movementAllowFire = movementPattern[mPstep].allowFire;

                    if (movementPattern[mPstep].repeatAlways || movementPattern[mPstep].repeatNTimes >= 0) {
                        movementPattern[mPstep].repeatNTimes--;
                        mPstep++;

                    } else
                        movementPattern.RemoveAt(mPstep);

                    // Volta para o primeiro padrão da lista caso eles tenham acabado.
                    if (mPstep >= movementPattern.Count)
                        mPstep = 0;
                }
            }

            // Move o objeto baseado o padrão de movimento atual.

            // Este "if" e o próximo fazem o movimento suavizando-o.
            if (movementPattern[mPstep].smoothMovement && mPtime < movementPattern[mPstep].duration / 2) // Executa a suavização na primeira metade do movimento.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * Mathf.Clamp(mPtime / (movementPattern[mPstep].duration / 2), 0, 1) * velocity * Time.deltaTime));
            else if (movementPattern[mPstep].smoothMovement && mPtime > movementPattern[mPstep].duration / 2) // Executa a suavização na primeira última do movimento.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * Mathf.Clamp(1 - (mPtime - (movementPattern[mPstep].duration / 2)) / (movementPattern[mPstep].duration / 2), 0, 1) * velocity * Time.deltaTime));
            else // Executa o movimento caso não haja suavização.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * velocity * Time.deltaTime));

            if (paralisedTime <= 0 && movementAllowFire) {

                // Checa se o player está na frente do inimigo.
                if (regularFireMode != RegularFireMode.DontRegularFire) {

                    RaycastHit2D hit = new RaycastHit2D();
                    hit = Physics2D.Raycast(transform.position, -Vector2.up, 10, raycastMask);

                    if (hit.collider != null) {
                        if (ReferenceEquals(GameController.player.gameObject, hit.collider.gameObject))
                            playerIsInSight = true;
                        else
                            playerIsInSight = false;
                    }
                    else
                        playerIsInSight = false;
                }

                switch (regularFireMode) {
                    case RegularFireMode.TargetRandomAndRandomFire: // Atira quando o player está na frente do inimigo e também aleatoriamente.
                        // Executa os disparos direcionados ao player.
                        if (playerIsInSight && transform.position.y - GameController.player.transform.position.y < distanceToTargetShoot && Random.value <= randomTargetFireChance * Time.deltaTime * 10)
                            EnemyFire();
                        else if (Random.value <= randomRegFireChance * Time.deltaTime) // Executa os disparos aleatórios.
                            EnemyFire();
                        break;
                    case RegularFireMode.OnlyRandomFire: // Atira somente aleatoriamente.
                        if (Random.value <= randomRegFireChance * Time.deltaTime)
                            EnemyFire();
                        break;
                    case RegularFireMode.DontRegularFire: // Não atira.
                        break;
                }
            }
            else
                playerIsInSight = false;
        }
    }

    public virtual void EnemyFire() {
        
        if (weaponGroups.Length != 0) {

            // Atira com todas as armas do grupo.
            for (int i = 0; i < weaponGroups[0].weapons.Length; i++) {
                weaponGroups[0].weapons[i].Fire();
            }
        }
    }

    public override void Damage(int amount, float paralyse) {

        // Garante que o inimigo não leve dano antes de aparecer na tela.
        if (_activated) {

            base.Damage(amount, paralyse);

            // Faz o inimigo revidar tiros.
            if (paralisedTime <= 0 && movementAllowFire)
                switch (fireBackMode) {
                    case FireBackMode.AlwaysFireback: // Sempre.
                        EnemyFire();
                        break;
                    case FireBackMode.FireBackOnlyFirst: // Só na primeira vez.
                        if (maxHull == hull)
                            EnemyFire();
                        break;
                    case FireBackMode.RandomFireBack: // Aleatóriamente.
                        if (Random.value <= randomFirebackChance)
                            EnemyFire();
                        break;
                    case FireBackMode.DontFireBack: // Nunca.
                        break;
                }
        }
    }
}
