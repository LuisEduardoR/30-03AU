using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

// Esse código controla os powerUps.
[AddComponentMenu("Scripts/Entities/Power Up")]
public class PowerUp : EntityBase {

    [Space(5)]
    // Pontos recebidos por pegar o powerUp.
    public int points = 0;
    // Créditos recebidos por pegar o powerUp.
    public int bonus_credits = 0;

    [Space(5)]
    // Usado para determinar o tipo do powerUp, "hull" sempre será usado para a vida e "credits" para créditos, o resto será tratado, por padrão, como um tipo de munição.
    public string typeName = "hull";
    // Quantidade da "coisa" a ser dada ao player.
    public int amount = 25;

    public override void Start() {

        // Guarda a cor do inimigo no começo do jogo.
        _defaultColor = _renderer.color;

    }

    public override void Update() {

        base.Update();

        // Move o powerUp
        transform.Translate(transform.InverseTransformDirection(0, -velocity * Time.deltaTime, 0));

    }
    
    // Função de dano que também faz o powerUp piscar.
    public override void Damage(int amount, float paralyse) {

        // Chama função padrão.
        base.Damage(amount, 0);

    }

    public override void Die() {

        for (int i = 0; i < deathEffect.Length; i++)
            Instantiate(deathEffect[i], transform.position, transform.rotation);
        Destroy(this.gameObject);

        GameController.gameController.AddPoints(points);
        GameController.gameController.AddCredits((points / 10) + bonus_credits);

    }

    public virtual void OnCollisionStay2D(Collision2D collision) {

        if (ReferenceEquals(GameController.player.gameObject, collision.collider.gameObject)) {
            EntityBase entity = collision.collider.GetComponent<EntityBase>();
            if (entity != null)
                PickPowerUp();

        }
    }

    public void PickPowerUp () {
        if (typeName == "hull") {
            if (GameController.player.Heal(amount)) {
                Destroy(this.gameObject);
            }
        }
        else if (typeName == "credits") {
            GameController.gameController.AddCredits(amount);
            Destroy(this.gameObject);
        } else {
            if (GameController.player.AddAmmo(amount, typeName)) {
                Destroy(this.gameObject);
            }
        }
    }
}
