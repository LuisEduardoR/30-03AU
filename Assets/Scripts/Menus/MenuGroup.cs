using UnityEngine;

[AddComponentMenu("Scripts/UI/Sub Menu Group")]
public class MenuGroup : MonoBehaviour {

    public GameObject[] menuSubGroups;

    // Muda entre os sub-grupos de menus.
    public void ChangeMenuGroup(int group) {

        // Deixa somente o menu desejado ativo.
        for (int i = 0; i < menuSubGroups.Length; i++) {
            if (i != group)
                menuSubGroups[i].SetActive(false);
            else
                menuSubGroups[i].SetActive(true);
        }
    }
}
