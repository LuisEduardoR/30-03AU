using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

// Código usado por objetos que aparecem na tela e podem ser destruídos.
[AddComponentMenu("Scripts/Entities/Destructible Prop")]
public class DestructibleProp : EnemyBase {

    // Padrões de movimento do objeto.
    [System.Serializable]
    public class MovementPattern {

        // Duração do movimento 0 = executar o padrão para sempre.
        public float duration = 0.0f;
        [Space(5)]
        // Direção do movimento.
        public Vector3 movDirection = new Vector3(0, -1, 0);
        // Rotação do objeto.
        public float rotation = 0.0f;
        [Space(5)]
        // Permite ou não suavização do movimento.
        public bool smoothMovement = false;
        [Space(5)]
        // Permite ou não que o inimigo atire durante o movimento.
        public bool allowFire = true;
        [Space(5)]
        // Guarda quantas vezes esse movimento deve se repetir.
        public bool repeatAlways = true; // Faz com que o movimento sempre se repita, ignorando a variável de baixo, pelo menos uma padrão por objeto deve ter essa variável marcada, ou ter uma de suas
                                         // "durations" = 0.
        public int repeatNTimes = 0; // Quantas vezes o movimento vai se repetir antes de sair da lista.
    }
    [Space(5)]
    public List<MovementPattern> movementPattern;

    // Guardam o padrão de movimento atual e o tempo que o objeto deve permanecer nesse padrão.
    private int mPstep;
    private float mPtime;

    // Objetos que devêm ser dropados quando esse objeto for destruído.
    [System.Serializable]
    public struct Drops {

        public GameObject _object;

        public Vector3 position;

    }
    [Space(5)]
    public Drops[] drop;

    public override void Update() {

        base.Update();

        if (_activated) {

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

                    }
                    else
                        movementPattern.RemoveAt(mPstep);

                    // Volta para o primeiro padrão da lista caso eles já tenham acabado.
                    if (mPstep >= movementPattern.Count)
                        mPstep = 0;
                }
            }

            // Move o objeto baseado o padrão de movimento atual.

            // Este "if" e o próximo fazem o movimento suavizando-o.
            if (movementPattern[mPstep].smoothMovement && mPtime < movementPattern[mPstep].duration / 2) // Executa a suavização na primeira metade do movimento.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * Mathf.Clamp(mPtime / (movementPattern[mPstep].duration / 2), 0, 1) * velocity * Time.deltaTime));
            else if (movementPattern[mPstep].smoothMovement && mPtime > movementPattern[mPstep].duration / 2) // Executa a suavização na última metade do movimento.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * Mathf.Clamp(1 - (mPtime - (movementPattern[mPstep].duration / 2)) / (movementPattern[mPstep].duration / 2), 0, 1) * velocity * Time.deltaTime));
            else // Executa o movimento caso não haja suavização.
                transform.Translate(transform.InverseTransformDirection(movementPattern[mPstep].movDirection * velocity * Time.deltaTime));

            if (movementPattern[mPstep].rotation != 0) // Executa o movimento de rotação.
                transform.Rotate( 0, 0, Time.deltaTime * movementPattern[mPstep].rotation);
        }


    }

    public override void Damage(int amount, float paralyse) {

        if (_activated) {
            base.Damage(amount, paralyse);
        }

    }

    public override void Die() {

        for (int i = 0; i < drop.Length; i++) {
            Instantiate(drop[i]._object, transform.position + drop[i].position, transform.rotation);
        }

        base.Die();

    }

}
