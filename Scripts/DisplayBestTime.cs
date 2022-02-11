using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DisplayBestTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex.ToString();
        var wasLevelCompleted = PlayerPrefs.HasKey(currentSceneIndex);
        if (wasLevelCompleted)
        {
            var time = PlayerPrefs.GetFloat(currentSceneIndex);
            var roundedTime = Mathf.Round(time * 10f) / 10f;
            if (roundedTime % Mathf.RoundToInt(roundedTime) == 0)
            {
                GetComponent<TextMeshProUGUI>().text = "Best " + roundedTime.ToString() + ".0s";
            }
            else
            {
                GetComponent<TextMeshProUGUI>().text = "Best " + roundedTime.ToString() + "s";
            }
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = "No Best";
        }
    }
}
