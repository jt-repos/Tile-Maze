using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        GetComponent<TextMeshProUGUI>().text = "Coins: " + PlayerPrefs.GetInt("coins").ToString();
    }
}
