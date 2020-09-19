using UnityEngine;

// Esse código move o background.
[AddComponentMenu("Scripts/Utility/Background Control")]
public class BackgroundControl : MonoBehaviour {

    public Renderer background;

    public float backgroundScrollVelocity = 1.0f;
    
    private Material _material;

    private float textureOffset;

    void Start () {

        // Cria e começa a usar uma instância do material do background para evitar casos indesejados, como o material real ser alterado para sempre no editor.
        if (_material == null)
            _material = Instantiate(background.material);
        background.material = _material;

    }
	
	void Update () {

        if (GameController.gameController._currentState == GameController.GameState.Play || GameController.gameController._currentState == GameController.GameState.Lose || GameController.gameController._currentState == GameController.GameState.Cutscene) {
            textureOffset += 0.02f * Time.deltaTime * backgroundScrollVelocity;
            _material.SetTextureOffset("_MainTex", new Vector2(0, textureOffset));
        }

    }

    // Reseta o background para a posição inicial.
    public void ResetBackground() {

        if(_material == null)
            _material = Instantiate(background.material);
        background.material = _material;

        textureOffset = 0;
        _material.SetTextureOffset("_MainTex", new Vector2(0, textureOffset));

    }
}
