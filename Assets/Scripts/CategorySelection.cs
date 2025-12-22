using UnityEngine;
using UnityEngine.UI;

public class CategorySelection : MonoBehaviour
{
    private static GameObject current;

    public Button button;
    public GameObject menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public static void SetCurrent(GameObject menu){ current = menu;}

    void OnButtonClick()
    {
        if(current != null){
            current.SetActive(false);
        }
        menu.SetActive(true);
        SetCurrent(menu);
    }
}
