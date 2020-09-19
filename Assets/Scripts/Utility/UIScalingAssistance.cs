using UnityEngine;

// Script usado para ajudar no escalonamento da UI durante o jogo.
[AddComponentMenu("Scripts/Utility/UI Scaling Assistance")]
public class UIScalingAssistance : MonoBehaviour {

    // Deve conter um RectTransform que seja "pai" de todos os elementos de UI que queira afetar.
    public RectTransform UIScreen;

	void Awake () {

        AdjustUIScale();

    }

    void AdjustUIScale () {

        int width = Screen.width;
        int height = Screen.height;

        int delta = width - height;

        UIScreen.offsetMin = new Vector2((delta / 2f) - (100 * height / 800), 0);
        UIScreen.offsetMax = new Vector2(-((delta / 2f) - (100 * height / 800)), 0);

    }
}
