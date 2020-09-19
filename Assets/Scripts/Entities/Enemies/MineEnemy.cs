using System.Collections.Generic;
using UnityEngine;

// Código de um inimigo básico que anda e atira.
[AddComponentMenu("Scripts/Enemies/Mine Enemy")]
public class MineEnemy : EnemyBase {

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
    public List<MovementPattern> movementPattern;
    
    // Guardam o padrão de movimento atual e o tempo que o inimigo deve permanecer nesse padrão.
    private int mPstep;
    private float mPtime;

    // Minas que serão soltas pelo inimigo.
    [Header("Weapons:")]
    [Space(3)]
    public List<GameObject> mines;

    public float distanceToLayMine = 4.5f; // A partir do meio da tela.

    public float delayBetweenMines = 1.0f;
    private float mineTime = 0.0f;

    public override void Start() {

        base.Start();

        mineTime += Time.deltaTime;

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

                    // Passa para o próximo padrão, contando quantas vezes o padrão atual foi utilizado e o removendo caso seu número de repetições acabe.
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
            else // Executa o movimento sem suavização.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * velocity * Time.deltaTime));

            // Solta minas de tempos em tempos até elas acabarem.
            if(transform.position.y <= distanceToLayMine && mines.Count > 0) {

                if (mineTime != 0)
                    mineTime += Time.deltaTime;

                if(mineTime >= delayBetweenMines) {

                    mines[0].transform.parent = null;

                    mines[0].GetComponent<Collider2D>().enabled = true;
                    mines[0].GetComponent<Rigidbody2D>().simulated = true;
                    mines[0].GetComponent<EnemyActivation>().Activate();

                    mines.RemoveAt(0);

                    mineTime = Time.deltaTime;
                }

            }
        }
    }

    public override void Damage(int amount, float paralyse) {

        // Garante que o inimigo não leve dano antes de aparecer na tela.
        if (_activated) {

            base.Damage(amount, paralyse);
        }
    }
}
