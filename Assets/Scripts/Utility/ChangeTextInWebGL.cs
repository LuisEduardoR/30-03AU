using UnityEngine;
using UnityEngine.UI;

// Muda um texto em uma build Web GL
[AddComponentMenu("Scripts/Utility/Change Text In Web GL")]
public class ChangeTextInWebGL : MonoBehaviour {

	public Text target;

	public string textToReplace;

	void Start () {
		# if UNITY_WEBGL
			target.text = textToReplace;
		# endif
	}
}
