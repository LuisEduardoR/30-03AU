using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

// Esse código controla os projéteis
[AddComponentMenu("Scripts/Weapons/Projectile")]
public class Projectile : MonoBehaviour {

    public int damage = 10;
    // Tempo que o projétil deixa o alvo paralisado, 0 = sem paralisia.
    public float paralyse = 0;
    
    public float velocity = 5.0f;
        
    public GameObject[] effectsLeftOnHit;

    void Update () {

        Vector3 projectile_velocity = (new Vector3( 0, velocity * Time.deltaTime, 0));
        transform.Translate(projectile_velocity);

        if (Mathf.Abs(transform.position.y) >= GameController.gameController.screenSize.y * 1.5f || Mathf.Abs(transform.position.x) >= GameController.gameController.screenSize.x * 1.5f)
            Destroy(gameObject);

	}

    void OnCollisionEnter2D(Collision2D collision) {

        // Faz com que os efeitos do projétil sejam aplicados somente se ele estiver na tela. Usado para impedir que inimigos sejam mortos fora dela.
        if (transform.position.y < GameController.gameController.screenSize.y + 0.5f || transform.position.y > -GameController.gameController.screenSize.y - 0.5f) {

            if (collision.collider.tag == "Entity") {

                EntityBase entity = collision.collider.GetComponent<EntityBase>();

                if (entity != null) {

                    entity.Damage(damage, paralyse);

                    for (int i = 0; i < effectsLeftOnHit.Length; i++)
                        Instantiate(effectsLeftOnHit[i], transform.position, transform.rotation);
                }
            }
        }

        Destroy(gameObject);
    }
}
