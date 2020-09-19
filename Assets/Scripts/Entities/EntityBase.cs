using UnityEngine;

// Serve como base para as entidades do jogo.
[AddComponentMenu("Scripts/Entities/Entity Base")]
public class EntityBase : MonoBehaviour {

    public int hull = 20;

    public float velocity = 1.5f;

    [Space(10)]
    public SpriteRenderer _renderer;
    [HideInInspector]
    public Color _defaultColor;

    // Guarda o tempo que se passou desde o último dano levado pelo inimigo para fazê-lo ficar de cor diferente.
    [HideInInspector]
    public float damagedTime;
    // Guarda o tempo pelo qual o inimigo ficará paralisado, também usado para aplicar a cor de paralisia.
    [HideInInspector]
    public float paralisedTime;

    public GameObject[] deathEffect;

    public virtual void Start () {

        _defaultColor = _renderer.color;

    }

    public virtual void Update () {

        // Destrói a entidade após ela passar pela tela.
        if (transform.position.y <= - GameController.gameController.screenSize.y * 1.5)
            Destroy(this.gameObject);

        // Faz com que a entidade volte ao normal após piscar por causa de um dano.
        if (damagedTime >= 0.5f) {
            damagedTime = 0;
            if (paralisedTime > 0)
                _renderer.color = Color.cyan;
            else
                _renderer.color = _defaultColor;
        }

        // Conta o tempo para a entidade voltar à sua cor normal.
        if (damagedTime != 0)
            damagedTime += Time.deltaTime;

        // Faz com que a entidade volte ao normal após piscar por causa de uma paralisia.
        if (paralisedTime <= 0) {
            paralisedTime = 0;
            if (damagedTime > 0 && damagedTime < 0.25f)
                _renderer.color = Color.red;
            else
                _renderer.color = _defaultColor;
        }

        // Conta o tempo pelo qual a entidade ficara paralizado.
        if (paralisedTime > 0)
            paralisedTime -= Time.deltaTime;

    }

    public virtual void Damage(int amount, float paralyse) {

        hull -= amount;

        if (hull <= 0)
            Die();

        // Começa a contar o tempo pelo qual a entidade ficará com a cor de dano.
        damagedTime += Time.deltaTime;

        // Aplica a cor de dano, de paralisia, ou ambas.
        if (paralisedTime + paralyse > 0 && amount > 0)
            _renderer.color = Color.blue;
        else if (amount > 0)
            _renderer.color = Color.red;
        else if (paralisedTime > 0)
            _renderer.color = Color.cyan;

        // Paraliza a entidade e começa a contar o tempo.
        if (paralyse > paralisedTime)
            paralisedTime = paralyse;
    }

    public virtual void Die() {

        for (int i = 0; i < deathEffect.Length; i++)
            Instantiate(deathEffect[i], transform.position, transform.rotation);

        Destroy(this.gameObject);

    }

    // Usada por triggers, principalmente explosões, para causar dano na entidade.
    public void OnTriggerEnter2D(Collider2D collision) {
       
        // Detecta se a entidade entrou em um trigger de dano e o "avisa" sobre isso.
        if (collision.tag == "TriggerDamage") {

            DamageTrigger trigger;
            trigger = collision.GetComponent<DamageTrigger>();

            trigger.TakeDamageFromTrigger(this);
        }

    }
}
