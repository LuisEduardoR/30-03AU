using UnityEngine;

// Script usado para adicionar uma força ou toque a um rigidibody.
[AddComponentMenu("Scripts/Utility/Add Physics Force")]
public class AddPhysicsForce : MonoBehaviour {

    public Rigidbody2D _rigidbody;

    public bool continous;


    public Vector3 force;
    public float torque;

	void Start () {

        if (_rigidbody == null) {
            Debug.LogWarning("No Rigidbody2D assigned by default, this script will try to find one, but it is recommended to assign it here. If no reference is found an error will occur!");
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        _rigidbody.AddForce(force);
        _rigidbody.AddTorque(torque);
    }
	
	void FixedUpdate () {

        if (continous) {
            _rigidbody.AddForce(force);
            _rigidbody.AddTorque(torque);
        }
        else
            Destroy(this);

	}
}
