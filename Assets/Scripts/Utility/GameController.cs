using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Game Controller")]
public class GameController : MonoBehaviour {

    public static GameController gameController = null;
    public static Player player;

    public enum GameState {
        MainMenu, Play, Pause, Win, Lose, Cutscene, Store, Load
    }
    public GameState _currentState = GameState.Play;

    public string mainMenuLevelName = "_MAIN_MENU_";
    public string storeLevelName = "_STORE_";
    public string firstLevelName = "_LEVEL1_";

    [HideInInspector]
    public float level_progress = 0;

    [Space(10)]
    public Camera _mainCamera;

    [Space(10)]
    // Tamanho da área jogável, para um determinado lado a partir da origem.
    public Vector2 screenSize = new Vector2(5.6f, 4.4f);

    [Space(10)]
    public GameObject playerObject;

    [Header("Score and credits:")]
    [Space(3)]
    public int points;
    public int run_points;
    public int earned_cr;


    [System.Serializable]
    public class MainMenuUI {

        public GameObject UIObject;

        public Button continueButton;

        public Slider masterVolume;
        public Slider musicVolume;

    }
    [Header("UI:")]
    [Space(3)]
    public MainMenuUI mainMenuUI;

    [System.Serializable]
    public class PlayerUI {

        public GameObject UIObject;

        public Text _points;
        public Text _run_points;
        public Text hull_text;
        public Image hull_bar;
        
        [System.Serializable]
        public struct WeaponUI {

            public Image weaponIcon;

            public Text keyUI;
            public Text ammoAmount;
            public Image delayTimer;

        }
        public WeaponUI priWeaponUI;
        public WeaponUI[] secWeaponUI;

    }
    public PlayerUI playerUI;

    public GameObject pauseUI;

    [System.Serializable]
    public class DeathUI {

        public GameObject UIObject;

        public Text _points;
        public Text _run_points;

        public Text _points_record;
        public Text _run_points_record;

    }
    public DeathUI deathUI;

    [System.Serializable]
    public class WinUI {

        public GameObject UIObject;

        public Text _points;
        public Text _run_points;

        public Text _credits;

        public Text _points_record;
        public Text _run_points_record;

    }
    public WinUI winUI;
    
    public GameObject storeUI;

    public GameObject loadUI;

    [Header("Weapons:")]
    [Space(3)]
    // Deve conter ScriptableObjects contendo todas as armas que o player pode utilizar.
    public Weapon[] avaliableWeapons;

    // Deve conter ScriptableObjects contendo a arma primária padrão do começo do jogo.
    public Weapon defaultPrimaryWeapon;

    private List<AudioSource> levelAudioSources;

    private float _deltaTime;

    [Header("Level:")]
    [Space(3)]
    public LevelInfo levelInfo;

    public BackgroundControl background;

    [Header("Music:")]
    [Space(3)]
    public AudioSource music;

    void Awake() {

        InitializeAudioVolume();

        // Cria uma referência estática a este GameController e faz com que ele persista ao carregar uma nova cena. Caso está referência já exista, destrói este objeto.
        if (gameController == null) {
            gameController = this;

            SceneManager.sceneLoaded += OnSceneLoaded;

            DontDestroyOnLoad(gameObject);

        } else
            Destroy(gameObject);

        if (_mainCamera == null)
            _mainCamera = Camera.main;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        loadUI.SetActive(false);

        // Reinicia valores necessários ao entrar em uma cena.
        points = 0;
        if (PlayerPrefs.HasKey("currentRunPoints"))
            run_points = PlayerPrefs.GetInt("currentRunPoints");
        else
            run_points = 0;
       
        earned_cr = 0;

        level_progress = 0;

        Time.timeScale = 1;

        if(background != null)
            background.ResetBackground();

        // Obtem informações sobre o nível.
        levelInfo = FindObjectOfType<LevelInfo>();

        if (levelInfo != null) {
            _mainCamera.backgroundColor = levelInfo.startSkyColor;
        } else {
            Debug.LogWarning("(Game Controller) No level info found on " + SceneManager.GetActiveScene().name + "!");
        }
        
        if (scene.name == mainMenuLevelName) {

            _currentState = GameState.MainMenu;

            if (PlayerPrefs.HasKey("currentLevel"))
                mainMenuUI.continueButton.interactable = true;
            else
                mainMenuUI.continueButton.interactable = false;

            mainMenuUI.UIObject.SetActive(true);
            pauseUI.SetActive(false);
            playerUI.UIObject.SetActive(false);
            deathUI.UIObject.SetActive(false);
            winUI.UIObject.SetActive(false);
            storeUI.SetActive(false);
            loadUI.SetActive(false);

            if (player != null)
                Destroy(player.gameObject);

        } else if (scene.name == storeLevelName) {

            _currentState = GameState.Store;

            mainMenuUI.UIObject.SetActive(false);
            pauseUI.SetActive(false);
            playerUI.UIObject.SetActive(false);
            deathUI.UIObject.SetActive(false);
            winUI.UIObject.SetActive(false);
            storeUI.SetActive(true);
            loadUI.SetActive(false);

        } else {
            _currentState = GameState.Play;
        }

        if (_currentState == GameState.Play) {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mainMenuUI.UIObject.SetActive(false);
            pauseUI.SetActive(false);
            playerUI.UIObject.SetActive(true);
            deathUI.UIObject.SetActive(false);
            winUI.UIObject.SetActive(false);
            storeUI.SetActive(false);
            loadUI.SetActive(false);

            if (player == null) {
                player = Instantiate(playerObject, new Vector3(0, -3, 0), new Quaternion()).GetComponent<Player>();

                for (int i = 0; i < avaliableWeapons.Length; i++)
                    if (avaliableWeapons[i].mainWeapon && PlayerPrefs.HasKey(avaliableWeapons[i].idName) && PlayerPrefs.GetInt(avaliableWeapons[i].idName) == 2) {
                            if (player.primaryWeapon == null)
                                player.primaryWeapon = avaliableWeapons[i];
                            else
                                Debug.LogWarning("(GameController) More than one main weapon is present! Using the first found on the list...");
                    } else if(!avaliableWeapons[i].mainWeapon)
                        player.secondaryWeapons.Add(avaliableWeapons[i]);

                // Caso o player não tenha uma arma primária, dá a ele a arma padrão e gera um aviso.
                if (player.primaryWeapon == null) {

                    Debug.LogWarning("(GameController) No primary weapon detected on PlayerPrefs save! Giving player the default primary weapon...");

                    player.primaryWeapon = defaultPrimaryWeapon;

                }

                // Cria as armas do player.
                player.CreateWeapons();

            }

            player.transform.position = new Vector3(0, -3.5f, 0);

            UpdateWeaponsUI();

            AddPoints(0);

            UpdatePointsUI();

            UpdateHealthBar();
        }
    }

    void Update() {

        if (_currentState == GameState.Play) {

            UpdateWeaponsUI();

            // Faz com que o level avance.
            level_progress += Time.deltaTime;

        }
        else if (_currentState == GameState.Lose) {
            
            // Guarda uma referência a um deltaTime normal para quando o jogo começar a ficar em câmera lenta.
            if (_deltaTime == 0)
                _deltaTime = Time.deltaTime;

            // Faz com que o tempo fique mais lento até o jogo pausar por completo e garante que ele nunca fique negativo.
            if (Time.timeScale > 0)
                Time.timeScale = Mathf.Clamp(Time.timeScale - _deltaTime * 0.2f, 0, 100);

            // Vai para o menu quando o jogo finalmente para.
            if (Time.timeScale == 0 && !deathUI.UIObject.activeSelf) {

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                deathUI.UIObject.SetActive(true);

            }
        }
    }

    public void AddPoints(int amount) {

        points += amount;
        run_points += amount;

        UpdatePointsUI();

    }

    public void AddCredits(int amount) {

        earned_cr += amount;

    }

    public void UpdateWeaponsUI() {

        // Para a arma primária:
        playerUI.priWeaponUI.weaponIcon.sprite = player.primaryWeapon._ui.inGameUISprite;

        if (player != null && !player.primaryWeaponScript.infinite_ammo) {

            playerUI.priWeaponUI.ammoAmount.text = player.primaryWeaponScript.currentAmmo + "x";
            playerUI.priWeaponUI.keyUI.text = player.primaryWeapon.fireKey;

            if (player.primaryWeaponScript.delayShots != 0 && player.primaryWeaponScript.currentAmmo > 0 || player.primaryWeaponScript.delayShots != 0 && player.primaryWeaponScript.infinite_ammo)
                playerUI.priWeaponUI.delayTimer.fillAmount = 1 - player.primaryWeaponScript.delayShots / player.primaryWeaponScript.delayBetweenShots;
            else if (player.primaryWeaponScript.currentAmmo > 0 || player.primaryWeaponScript.infinite_ammo)
                playerUI.priWeaponUI.delayTimer.fillAmount = 0;
            else
                playerUI.priWeaponUI.delayTimer.fillAmount = 1;

        }
        else if (player != null) {

            playerUI.priWeaponUI.ammoAmount.text = "inf";
            playerUI.priWeaponUI.keyUI.text = player.primaryWeapon.fireKey;

            if (player.primaryWeaponScript.delayShots != 0)
                playerUI.priWeaponUI.delayTimer.fillAmount = 1 - player.primaryWeaponScript.delayShots / player.primaryWeaponScript.delayBetweenShots;
            else
                playerUI.priWeaponUI.delayTimer.fillAmount = 0;

        }

        // Para as armas secundárias:
        for (int i = 0; i < player.secondaryWeapons.Count; i++) {

            playerUI.secWeaponUI[i].weaponIcon.sprite = player.secondaryWeapons[i]._ui.inGameUISprite;

            if (player != null && !player.secondaryWeaponScripts[i].infinite_ammo) {

                playerUI.secWeaponUI[i].ammoAmount.text = player.secondaryWeaponScripts[i].currentAmmo + "x";
                playerUI.secWeaponUI[i].keyUI.text = player.secondaryWeapons[i].fireKey;

                if (player.secondaryWeaponScripts[i].delayShots != 0 && player.secondaryWeaponScripts[i].currentAmmo > 0 || player.secondaryWeaponScripts[i].delayShots != 0 && player.secondaryWeaponScripts[i].infinite_ammo)
                    playerUI.secWeaponUI[i].delayTimer.fillAmount = 1 - player.secondaryWeaponScripts[i].delayShots / player.secondaryWeaponScripts[i].delayBetweenShots;
                else if (player.secondaryWeaponScripts[i].currentAmmo > 0 || player.secondaryWeaponScripts[i].infinite_ammo)
                    playerUI.secWeaponUI[i].delayTimer.fillAmount = 0;
                else
                    playerUI.secWeaponUI[i].delayTimer.fillAmount = 1;

            }
            else if (player != null) {

                playerUI.secWeaponUI[i].ammoAmount.text = "inf";
                playerUI.secWeaponUI[i].keyUI.text = player.secondaryWeapons[i].fireKey;

                if (player.secondaryWeaponScripts[i].delayShots != 0)
                    playerUI.secWeaponUI[i].delayTimer.fillAmount = 1 - player.secondaryWeaponScripts[i].delayShots / player.secondaryWeaponScripts[i].delayBetweenShots;
                else
                    playerUI.secWeaponUI[i].delayTimer.fillAmount = 0;

            }

        }
    }

    public void UpdatePointsUI() {

        playerUI._points.text = "" + points;
        playerUI._run_points.text = "" + run_points;

    }

    public void UpdateHealthBar() {

        if ((float)player.hull / (float)player.maxHull <= 0.5f && (float)player.hull / (float)player.maxHull > 0.25f) {
            playerUI.hull_bar.color = Color.yellow;
            playerUI.hull_text.color = Color.yellow;
        } else if ((float)player.hull / (float)player.maxHull <= 0.25f) {
            playerUI.hull_bar.color = Color.red;
            playerUI.hull_text.color = Color.red;
        } else {
            playerUI.hull_bar.color = Color.green;
            playerUI.hull_text.color = Color.green;
        }

        playerUI.hull_text.text = player.hull + "/" + player.maxHull;
        playerUI.hull_bar.fillAmount = (float)player.hull / (float)player.maxHull;

    }

    public void PauseGame() {

        _currentState = GameState.Pause;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        // Pausa todos os áudios da cena, com excessão das músicas (que devêm ser marcadas com a tag "Music") e cria uma lista com aqueles que deve ser despausados depois.
        levelAudioSources = new List<AudioSource>();
        AudioSource[] _aSources = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < _aSources.Length; i++) {
            if (_aSources[i].gameObject.tag != "Music" && _aSources[i].isPlaying) {
                levelAudioSources.Add(_aSources[i]);
                _aSources[i].Pause();
            }
        }
        
        // Mostra os menus apropriados.
        pauseUI.SetActive(true);
        playerUI.UIObject.SetActive(false);

    }

    public void UnpauseGame() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currentState = GameState.Play;

        Time.timeScale = 1;

        // Despausa todos os áudios que foram pausados previamente.
        for (int i = 0; i < levelAudioSources.Count; i++)
            if (levelAudioSources[i] != null)
                levelAudioSources[i].UnPause();

        // Mostra os menus apropriados.
        pauseUI.SetActive(false);
        playerUI.UIObject.SetActive(true);

    }


    public void QuitToMainMenu() {

        _currentState = GameState.Load;

        mainMenuUI.UIObject.SetActive(false);
        pauseUI.SetActive(false);
        playerUI.UIObject.SetActive(false);
        deathUI.UIObject.SetActive(false);
        winUI.UIObject.SetActive(false);
        storeUI.SetActive(false);
        loadUI.SetActive(true);

        SceneManager.LoadScene(mainMenuLevelName);

    }

    public void LoseLevel() {

        playerUI.UIObject.SetActive(false);

        _currentState = GameState.Lose;

        UpdateFinalScoreUI();

    }

    public void WinLevel() {
        
        Time.timeScale = 0;
        player.enabled = false;

        playerUI.UIObject.SetActive(false);
        winUI.UIObject.SetActive(true);

        _currentState = GameState.Win;

        UpdateFinalScoreUI();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void UpdateFinalScoreUI() {

        // Aualiza a UI de pontuação na tela de morte.
        if (_currentState == GameState.Lose) {

            deathUI._points.text = "Pontuação Nível: " + points;
            deathUI._run_points.text = "Pontuação Total: " + run_points;

            if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_record")) {

                int level_record = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_record");

                if (points > level_record)
                    deathUI._points_record.text = " <color=#00ff00ff>Novo Recorde!</color>";
                else
                    deathUI._points_record.text = "Recorde Nível: " + level_record;
            }
            else {
                deathUI._points_record.text = " <color=#00ff00ff>Novo Recorde!</color>";
            }

            if (PlayerPrefs.HasKey("run_record")) {

                int run_record = PlayerPrefs.GetInt("run_record");

                if (run_points > run_record)
                    deathUI._run_points_record.text = " <color=#00ff00a0>Novo Recorde!</color>";
                else
                    deathUI._run_points_record.text = "Recorde Total: " + run_record;
            }
            else {
                deathUI._run_points_record.text = " <color=#00ff00a0>Novo Recorde!</color>";
            }
        }

        // Atualiza a UI de pontuação na tela de vitória.
        if (_currentState == GameState.Win) {

            winUI._points.text = "Pontuação Nível: " + points;
            winUI._run_points.text = "Pontuação Total: " + run_points;

            winUI._credits.text = "+ Cr$: " + earned_cr;

            if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_record")) {

                int level_record = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_record");

                if (points > level_record)
                    winUI._points_record.text = " <color=#00ff00ff>Novo Recorde!</color>";
                else
                    winUI._points_record.text = "Recorde Nível: " + level_record;
            }
            else {
                winUI._points_record.text = " <color=#00ff00ff>Novo Recorde!</color>";
            }

            if (PlayerPrefs.HasKey("run_record")) {

                int run_record = PlayerPrefs.GetInt("run_record");

                if (run_points > run_record)
                    winUI._run_points_record.text = " <color=#00ff00a0>Novo Recorde!</color>";
                else
                    winUI._run_points_record.text = "Recorde Total: " + run_record;
            }
            else {
                winUI._run_points_record.text = " <color=#00ff00a0>Novo Recorde!</color>";
            }
        }

    }

    public void CompleteLevel() {

        // Verifica se o jogador quebrou um recorde e salva sua pontuação.
        SaveMaxScore();

        // Adiciona a quantidade de créditos ganha pelo jogador. 
        int credits = 0;

        if (PlayerPrefs.HasKey("credits"))
            credits += PlayerPrefs.GetInt("credits");

        credits += earned_cr;

        PlayerPrefs.SetInt("credits", credits);

        // Salva a vida atual do jogador.
        PlayerPrefs.SetInt("currentHull", player.hull);

        // Salva a ponutação atual nesse jogo.
        PlayerPrefs.SetInt("currentRunPoints", run_points);

        // Salva a munição do jogador.
        for (int i = 0; i < player.secondaryWeaponScripts.Length; i++) {

            if (player.secondaryWeapons[i].ammoName != "none")
                PlayerPrefs.SetInt("ammo_" + player.secondaryWeapons[i].ammoName, player.secondaryWeaponScripts[i].currentAmmo);

        }
    }

    // Salva o recorde do jogador caso esse tenha sido quebrado.
    public void SaveMaxScore() {

        int level_record = 0;
        int run_record = 0;

        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_record"))
            level_record = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_record");

        if (points > level_record)
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_record", points);


        if (PlayerPrefs.HasKey("run_record"))
            run_record = PlayerPrefs.GetInt("run_record");

        if (run_points > run_record)
            PlayerPrefs.SetInt("run_record", run_points);

    }

    public void NextLevel() {

        if (SceneManager.GetActiveScene().name != storeLevelName) {

            _currentState = GameState.Load;

            winUI.UIObject.SetActive(false);
            loadUI.SetActive(true);

            PlayerPrefs.SetString("currentLevel", levelInfo.nextLevel);
            SceneManager.LoadSceneAsync(storeLevelName);
        } else {
            storeUI.SetActive(false);
            loadUI.SetActive(true);
            SceneManager.LoadSceneAsync(PlayerPrefs.GetString("currentLevel"));
        }
        

    }

    public void RestartLevel() {

        _currentState = GameState.Load;

        deathUI.UIObject.SetActive(false);
        loadUI.SetActive(true);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

    }

    public void RestartLevelFromStore() {

        _currentState = GameState.Load;

        deathUI.UIObject.SetActive(false);
        loadUI.SetActive(true);
        SceneManager.LoadSceneAsync(storeLevelName);

    }

    public void ContinueGame() {

        _currentState = GameState.Load;

        mainMenuUI.UIObject.SetActive(false);
        loadUI.SetActive(true);

        SceneManager.LoadSceneAsync(PlayerPrefs.GetString("currentLevel"));

    }

    public void StartNewGame() {

        _currentState = GameState.Load;

        mainMenuUI.UIObject.SetActive(false);
        loadUI.SetActive(true);

        PlayerPrefs.SetString("currentLevel", firstLevelName);

        PlayerPrefs.SetInt("currentHull", 100);
        PlayerPrefs.SetInt("maxHull", 100);

        PlayerPrefs.SetInt("credits", 0);

        PlayerPrefs.SetInt("currentRunPoints", 0);

        // Seta as PlayerPrefs relacionadas às arma para o padrão.
        for (int i = 0; i < gameController.avaliableWeapons.Length; i++) {

            if (avaliableWeapons[i].mainWeapon)
                PlayerPrefs.SetInt(gameController.avaliableWeapons[i].idName, 0);

            if (avaliableWeapons[i].ammoName != "none")
                PlayerPrefs.SetInt("ammo_" + avaliableWeapons[i].ammoName, avaliableWeapons[i].defaultAmmo);
        }

        if (PlayerPrefs.HasKey(gameController.defaultPrimaryWeapon.idName)) {
            PlayerPrefs.SetInt(gameController.defaultPrimaryWeapon.idName, 2);
            if (defaultPrimaryWeapon.ammoName != "none")
                PlayerPrefs.SetInt("ammo_" + defaultPrimaryWeapon.ammoName, defaultPrimaryWeapon.defaultAmmo);
        } else
            Debug.LogError("(MainMenu) defaultPrimaryWeapon <" + gameController.defaultPrimaryWeapon.idName + "> isn't also in avaliableWeapons[]!");

        SceneManager.LoadSceneAsync(firstLevelName);

    }

    public void InitializeAudioVolume() {

        int masterVol = 10;
        if (PlayerPrefs.HasKey("masterVolume"))
            masterVol = PlayerPrefs.GetInt("masterVolume");
        else
            PlayerPrefs.SetInt("masterVolume", masterVol);

        AudioListener.volume = masterVol / 10.0f;
        mainMenuUI.masterVolume.value = masterVol;


        int musicVol = 10;
        if (PlayerPrefs.HasKey("musicVolume"))
            musicVol = PlayerPrefs.GetInt("musicVolume");
        else
            PlayerPrefs.SetInt("musicVolume", musicVol);

        music.volume = musicVol / 100.0f;
        mainMenuUI.musicVolume.value = musicVol;

    }

    public void UpdateAudioVolume() {

        PlayerPrefs.SetInt("masterVolume", (int)mainMenuUI.masterVolume.value);
        AudioListener.volume = mainMenuUI.masterVolume.value / 10.0f;
        PlayerPrefs.SetInt("musicVolume", (int)mainMenuUI.masterVolume.value);
        music.volume = mainMenuUI.musicVolume.value / 100.0f;

    }

    public void OpenInternetLink(string link) {

        Application.OpenURL(link);

    }

    public void QuitGame() {

        PlayerPrefs.Save(); // Garante que as PlayerPrefs sejam salvas antes de sair.

        Debug.Log("Application.Quit()");
        Application.Quit();

    }

    public void ResetGame() {

        Debug.Log("All PlayerPrefs deleted!");
        PlayerPrefs.DeleteAll();

        Debug.Log("Application.Quit()");
        Application.Quit();

    }
}
