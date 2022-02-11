using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCanvasButton : MonoBehaviour
{
    [SerializeField] Color lockedTextColor;
    [SerializeField] Sprite lockedSprite;
    [SerializeField] Color completedTextColor;
    [SerializeField] Sprite completedSprite;
    TextMeshProUGUI childText;
    int buttonIndex;
    float scaleFactor;
    Level level;

    // Start is called before the first frame update
    void Start()
    {
        childText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        scaleFactor = FindObjectOfType<Canvas>().scaleFactor;
        var buttonScale = gameObject.GetComponent<RectTransform>().localScale;
        buttonScale.x = scaleFactor;
        buttonScale.y = scaleFactor;
    }

    public void SetLevelIndex(int index)
    {
        childText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        level = FindObjectOfType<Level>();
        buttonIndex = index; //bo 0 to main menu 
        childText.text = (index + 1).ToString(); //child 0 since buttons have only one child
        if (level.IsLevelUnlocked(buttonIndex))
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Image>().sprite = lockedSprite;
            childText.color = lockedTextColor;
        }
        if (PlayerPrefs.HasKey(index.ToString()))
        {
            GetComponent<Image>().sprite = completedSprite;
            childText.color = completedTextColor;
        }
    }

    public void LoadLevelOnPress()
    {
        FindObjectOfType<Level>().LoadLevelByIndex(buttonIndex);
    }
}
