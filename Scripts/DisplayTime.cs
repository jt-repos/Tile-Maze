using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTime : MonoBehaviour
{
    TextMeshProUGUI textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(FindObjectsOfType<GameSession>().Length > 0)
        {
       
            var time = FindObjectOfType<GameSession>().GetCurrentTime();
            var roundedTime = Mathf.Round(time*10f) / 10f;
            if(time > 100)
            {
                time = 99.9f;
            }
            if(roundedTime % Mathf.RoundToInt(roundedTime) == 0)
            {
                textComponent.text = roundedTime.ToString() + ".0s";
            }
            else
            {
                textComponent.text = roundedTime.ToString() + "s";
            }
        }
    }
}
