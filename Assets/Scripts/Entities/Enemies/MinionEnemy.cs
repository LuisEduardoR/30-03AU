using UnityEngine;

// Código que controla pequenas naves que perseguem e se jogam contra o player. Igual ao inimigo suicida, porém as naves com esse script já começam ativadas e não são colocadas no início da tela.
[AddComponentMenu("Scripts/Entities/Enemies/Minion Enemy")]
public class MinionEnemy : SuicideEnemy {

    public override void Start() {

        if(GameController.player != null)
            target = GameController.player.gameObject;

        base.Start();

        ActivateEnemy();

    }

    // Função que ativa a entidade modificada.
    public override void ActivateEnemy() {

        _activated = true;

    }
}
