using UnityEngine;

// Esse script ativa um ou mais inimigos/objetos depois de um certo progresso no nível.
[AddComponentMenu("Scripts/Utility/Enemy Activation")]
public class EnemyActivation : MonoBehaviour {

    public bool activateByCommand;

    [Space(10)]
    public bool autoDetectProgress = true;
    public float progressToTrigger = 0;

    public EnemyBase[] targets;

    void Start() {

        if (autoDetectProgress) // Seta o progresso de ativação para o transform desse script.
            progressToTrigger = transform.position.y;

    }

    void FixedUpdate () {

        if (GameController.gameController.level_progress >= progressToTrigger && !activateByCommand)
            Activate();

	}

    // Ativa os inimigos/objetos.
    public void Activate() {

        for (int i = 0; i < targets.Length; i++) {

            targets[i].ActivateEnemy();

        }

        Destroy(this);

    }
}
