using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] int chapterIndex;
    [SerializeField] Sprite disabledSprite;
    [SerializeField] Color disabledTextColor;
      
    // Start is called before the first frame update
    void Start()
    {
        if (!FindObjectOfType<Level>().IsFullGameUnlocked() && chapterIndex != 0)
        {
            DisableButton();
        }
    }

    private void DisableButton()
    {
        GetComponent<Button>().enabled = false;
        GetComponent<Image>().sprite = disabledSprite;
        GetComponentInChildren<TextMeshProUGUI>().color = disabledTextColor;
    }

    public void PassAndLoadSelect()
    {
        PlayerPrefs.SetInt("chapterIndexKey", chapterIndex);
        FindObjectOfType<Level>().LoadLevelSelect();
    }
}
