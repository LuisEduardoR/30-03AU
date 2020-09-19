using UnityEngine;

// Usado por triggers para causar dano.
[AddComponentMenu("Scripts/Utility/Damage Trigger")]
public class DamageTrigger : MonoBehaviour {


    public int damage = 25;
    // Tempo para para causar paralisia no alvo (0 = sem paralisia).
    public float paralyse = 0;

    // Essa função é chamada pelo objeto que entrou no trigger.
    public void TakeDamageFromTrigger (EntityBase target) {

        target.Damage(damage, paralyse);
        
    }
}