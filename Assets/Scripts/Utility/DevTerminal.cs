using UnityEngine;

// Terminal de desenvolvedor.
[AddComponentMenu("Scripts/Utility/Developer Terminal")]
public class DevTerminal : MonoBehaviour {

    private bool showTerminal = false;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.F1))
            showTerminal =! showTerminal;

    }

    void OnGUI () {

        if (showTerminal) {

            GUI.Label(new Rect(10, 10, 100, 20), "Terminal:");

            if (GUI.Button(new Rect(10, 30, 100, 30), "+Cr$ (1)") || Input.GetKeyDown("1")) {

                int cr = 0;
                if (PlayerPrefs.HasKey("credits"))
                    cr = PlayerPrefs.GetInt("credits");

                PlayerPrefs.SetInt("credits", cr + 1000);

                if (GameController.gameController._currentState == GameController.GameState.Store)
                    GameController.gameController.storeUI.GetComponent<Store>().UpdateStoreUI();
            }

            if (GUI.Button(new Rect(10, 65, 100, 30), "Perder (2)") || Input.GetKeyDown("2")) {

                if (GameController.gameController._currentState == GameController.GameState.Play)
                    GameController.player.Damage(10000, 0);

            }

            if (GUI.Button(new Rect(10, 100, 100, 30), "Vencer (3)") || Input.GetKeyDown("3")) {

                if (GameController.gameController._currentState == GameController.GameState.Play)
                    GameController.gameController.levelInfo.Win();

            }
        }
    }
}
