using UnityEngine;

// Destroi o objeto quando a animação atinge o estado "Destroy" que deve ser adicionado dentro do AnimationController.
[AddComponentMenu("Scripts/Utility/Animation Destroy")]
public class AnimationDestroy : MonoBehaviour {

    public Animator _animator;

    void Awake() {

        if (_animator == null) {
            Debug.LogWarning("No Animator assigned by default, this script will try to find one, but it is recommended to assign it here. If no reference is found an error will occur!");
            _animator = GetComponent<Animator>();
        }

    }

    void Update () {

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
            Destroy(gameObject);

	}
}
