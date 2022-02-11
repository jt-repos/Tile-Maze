using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayWinTitle : MonoBehaviour
{
    [SerializeField] Sprite newHighScoreImage;
    [SerializeField] Sprite standardImage;

    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<Level>().GetIsNewHighScore())
        {
            GetComponent<TextMeshProUGUI>().text = "NEW HIGH SCORE";
            var image = transform.parent.GetComponentInChildren<Image>();
            image.sprite = newHighScoreImage;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = "CONGRATS!";
            transform.parent.GetComponentInChildren<Image>().sprite = standardImage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
