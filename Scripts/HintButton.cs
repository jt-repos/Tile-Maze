using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HintButton : MonoBehaviour
{
    TextMeshProUGUI buttonText;
    bool isCurrentHintPurchased;
    [SerializeField] string notPurchasedButtonText = "GET HINT?";
    [SerializeField] string purchasedButtonText = "PLAY HINT?";

    public void SetActive(bool setActive)
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        var boolToAlpha = Convert.ToInt32(setActive); //0 if false, 1 if true
        GetComponent<Button>().interactable = setActive;
        Color imageColor = GetComponent<Image>().color;
        Color textColor = buttonText.color;
        imageColor.a = boolToAlpha;
        textColor.a = boolToAlpha;
        GetComponent<Image>().color = imageColor;
        buttonText.color = textColor;
    }

    public void LoadHintCanvas()
    {
        if(!isCurrentHintPurchased)
        {
            FindObjectOfType<GameSession>().SpawnBuyHintCanvas();
        }
        else
        {
            FindObjectOfType<GameSession>().PlayHint();
        }
    }

    public void SetIsCurrentHintPurchased(bool boolVar) //called by player
    {
        isCurrentHintPurchased = boolVar;
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(isCurrentHintPurchased)
        {
            buttonText.text = purchasedButtonText;
        }
        else
        {
            buttonText.text = notPurchasedButtonText;
        }
    }
}

