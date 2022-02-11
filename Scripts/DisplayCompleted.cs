using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayCompleted : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var levelsInChapter = FindObjectOfType<SelectButtons>().GetNumberOfLevelsInChapter();
        var chapterIndex = PlayerPrefs.GetInt("chapterIndexKey");
        var completedLevelsInChapter = FindObjectOfType<Level>().GetLevelsCompletedInChapter(chapterIndex);
        GetComponent<TextMeshProUGUI>().text = completedLevelsInChapter.ToString() + "/" + levelsInChapter.ToString() + " COMPLETED";
    }
}
