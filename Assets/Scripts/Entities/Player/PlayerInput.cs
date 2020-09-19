using UnityEngine;

// Usado para facilitar o input em outras plataformas.
[AddComponentMenu("Scripts/Entities/Player/Input")]
public class PlayerInput : MonoBehaviour {

    public Player _player;

	void Start () {

        if (_player == null) {
            Debug.LogWarning("No Player assigned by default, this script will try to find one, but it is recommended to assign it here. If no reference is found an error will occur!");
            _player = GetComponent<Player>();
        }

    }
	
	void Update () {

        // Detecta os inputs do teclado e passa eles ao player.
        _player.p_input.x_axis = Input.GetAxis("Horizontal");
        _player.p_input.y_axis = Input.GetAxis("Vertical");
        _player.p_input.pauseKey = Input.GetButton("Cancel");

    }
}
