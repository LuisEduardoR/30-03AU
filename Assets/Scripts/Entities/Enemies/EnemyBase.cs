using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

// Serve como base para inimigos e alguns outros objetos que compartilham alguns compartamentos básicos, como os objetos que podem ser destruídos e caixas.
[AddComponentMenu("Scripts/Entities/Enemies/Enemy Base")]
public class EnemyBase : EntityBase {

    [Space(10)]
    // Controla sé o inimigo já entrou em tela.
    public bool _activated = false;

    // Faz com que o inimigo seja ativado somente por um script externo.
    public bool specialActivation;

    // Faz com que o inimigo seja ativado mais para a frente ou para trás doque o normal.
    public float activationPosition;

    [Space(10)]
    public int points = 100;
    public int credits = 10;

    public override void Start() {

        base.Start();

    }

    public override void Update() {

        base.Update();

        // Para de executar o código do inimigo caso o jogo esteja pausado.
        if (GameController.gameController._currentState == GameController.GameState.Pause)
            return;

        // Ativa o inimigo depois de um certo progresso no nível, quanto mais para trás o inimigo estiver em relação à origem, maior a demora.
        if (transform.position.y - GameController.gameController.level_progress < 6.0f && !_activated && !specialActivation)
            ActivateEnemy();

    }

    public virtual void ActivateEnemy() {

        transform.position = new Vector3(transform.position.x, 6 + activationPosition, 0);
        _activated = true;
    }

    // Usada para detectar colisões com o player.
    public virtual void OnCollisionEnter2D(Collision2D collision) {

        // Checa se a colisão realmente foi com o player.
        if (GameController.player != null && ReferenceEquals(collision.collider.gameObject, GameController.player.gameObject)) {

            int playerHull = GameController.player.hull;

            GameController.player.Damage(hull, 0);
            Damage(playerHull, 0);

        }
    }

    public override void Die() {

        base.Die();

        GameController.gameController.AddPoints(points);
        GameController.gameController.AddCredits(credits);

    }
}
