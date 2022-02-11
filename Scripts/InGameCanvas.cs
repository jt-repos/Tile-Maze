using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        int numberLevels = FindObjectsOfType<InGameCanvas>().Length;
        if (numberLevels > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ButtonEnabled(bool isEnabled)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = isEnabled;
        }
    }

    public void HideCanvas()
    {
        ButtonEnabled(false);
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ShowCanvas()
    {
        ButtonEnabled(true);
        GetComponent<CanvasGroup>().alpha = 1;
    }
}
