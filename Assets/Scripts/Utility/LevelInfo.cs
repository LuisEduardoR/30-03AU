using UnityEngine;

[AddComponentMenu("Scripts/Utility/Level Info")]
// Esse código contêm as informações sobre um determinado nível.
public class LevelInfo : MonoBehaviour {

    public string nextLevel;

    [System.Serializable]
    public class WinCondition {

        [Header("Time:")]
        [Space(3)]
        // Tempo até causar a vitória, -1 = não ganha o nível por tempo.
        public float timeToWin = 5;
        [HideInInspector]
        public float _time = 0;

        [Header("Progress:")]
        [Space(3)]
        // Progresso até causar a vitória, -1 = não ganha o nível por progresso.
        public float progressToWin = -1;

        [Header("Target:")]
        [Space(3)]
        // Ganha o jogo quando certo objeto for destruído.
        public bool winByTarget;
        public GameObject target;
        // Delay entre destruir o objeto e ganhar o jogo.
        public float delay = 4.0f;
        [HideInInspector]
        public float _delayCounter = 0;

    }
    [Space(10)]
    public WinCondition winCondition;

    [Space(10)]
    public Color startSkyColor;
    public Color endSkyColor;

    public float colorTransitionDuration = 200;

    [Space(10)]
    public AudioClip soundtrack;

    void FixedUpdate() {

        if (winCondition.timeToWin >= 0) {

            if (winCondition._time >= winCondition.timeToWin)
                Win();

            winCondition._time += Time.fixedDeltaTime; 
        }

        if (winCondition.progressToWin >= 0) {

            if (GameController.gameController.level_progress >= winCondition.progressToWin)
                Win();

        }

        if (winCondition.winByTarget && winCondition.target == null) {

            winCondition._delayCounter += Time.fixedDeltaTime;

            if (winCondition._delayCounter >= winCondition.delay)
                Win();

        }
    }

    public void Win() {
        GameController.gameController.WinLevel();
    }
}
