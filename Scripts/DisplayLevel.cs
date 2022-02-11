using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DisplayLevel : MonoBehaviour
{
    int currentSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GetComponent<TextMeshProUGUI>().text = "LVL " + (currentSceneIndex + 1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        var newSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(newSceneIndex != currentSceneIndex)
        {
            currentSceneIndex = newSceneIndex;
            GetComponent<TextMeshProUGUI>().text = "LVL " + (currentSceneIndex + 1).ToString();
        }
    }
}
