using UnityEngine;

// Código que no momento controla o inimigo suicida mas pode ser modificado para outros tipos de inimigos perseguidores.
[AddComponentMenu("Scripts/Entities/Enemies/Suicide Enemy")]
public class SuicideEnemy : EnemyBase {

    // Guarda o alvo do inimigo.
    [HideInInspector]
    public GameObject target;

    [Header("Target Seeking:")]
    [Space(3)]
    // Tempo entre o inimigo entrar na tela e começar a seguir o player.
    public float delayToSeek = 3.5f;
    // Conta o tempo até o inimigo começar a seguir.
    private float timeSeek = 0;

    // Distância em que o inimigo começa a seguir automaticamente.
    public float distanceToSeek = 1.5f;

    [Space(5)]
    // Bônus de velocidade ao começar a seguir.
    public float seekingVelocityBonus = 1.0f;
    // Usado para garantir que a velocidade bônus só seja usada no momento certo.
    private float velocityBonus = 0;

    // Velocidade com que o inimigo vira.
    public float turnVelocity = 1.0f;

    [Space(5)]
    // Animator do inimigo.
    public Animator _animator;
    // Guarda se a animação de seguir já foi ativada.
    private bool animatorActivated = false;

    public override void Update() {

            base.Update();

        // Para de executar o código do inimigo caso o jogo esteja de pausado.
        if (GameController.gameController._currentState == GameController.GameState.Pause)
            return;

        if (_activated && paralisedTime <= 0) {

            // Conta o tempo para o inimigo começar a seguir.
            if(target != null)
                timeSeek += Time.deltaTime;

            // Move o inimigo.
            transform.Translate(-Vector2.up * (velocity + velocityBonus) * Time.deltaTime);

            // Faz com que o inimigo comece a seguir automaticamente caso o player chegue muito perto.
            if(target != null)
                if(Vector3.Distance(transform.position, target.transform.position) < distanceToSeek)
                    timeSeek = delayToSeek;

            // Faz com que o inimigo comece a seguir o player.
            if (timeSeek >= delayToSeek && target != null) {

                // Ativa a animação de seguir caso ela não esteja ativada e caso ela exista também.
                if (_animator != null && !animatorActivated) {
                    _animator.SetBool("Danger", true);
                    animatorActivated = true;
                }

                // Aplica o bônus de velocidade.
                if (velocityBonus == 0)
                    velocityBonus = seekingVelocityBonus;

                // Detecta a rotação em que o inimigo tem que estar para olhar para o plyaer.
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - target.transform.position, -Vector3.forward);
                targetRotation.x = 0; // Garante que a rotação esteja em um espaço 2D.
                targetRotation.y = 0; //

                // Faz com que o inimigo vire lentamente até olhar o plyaer.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnVelocity);

            }
        
        }
    }

    // Função que ativa a entidade modificada.
    public override void ActivateEnemy() {

        // Chama a função padrão.
        base.ActivateEnemy();

        // Pega uma refrência ao player como alvo.
        if(GameController.player != null)
            target = GameController.player.gameObject;

    }

    // Função de dano modificada.
    public override void Damage(int amount, float paralyse) {

        // Garante que o inimigo não leve dano antes de aparecer na tela.
        if (_activated) {

            // Chama a função padrão.
            base.Damage(amount, paralyse);

            // Faz com que o inimigo comece a seguir ao levar dano.
            timeSeek = delayToSeek;
        }
    }
}
