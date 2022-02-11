using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtons : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;
    int chapterIndex;
    int levelsInChapter;
    int startLevelIndex;

    [Header("Level Buttons")]
    [SerializeField] int numberOfButtonsInRow;
    [SerializeField] float screenSideOffset;
    float scrollbarOffset;
    float screenTopOffset;

    // Start is called before the first frame update
    void Start() 
    {
        var scrollSizeX = GetComponent<RectTransform>().sizeDelta.x;
        screenTopOffset = buttonPrefab.GetComponent<RectTransform>().rect.height / 2; //so it matches with scroll vertical
        scrollbarOffset = -transform.parent.parent.GetComponent<ScrollRect>().verticalScrollbarSpacing;
        GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        var chapterIndex = PlayerPrefs.GetInt("chapterIndexKey");
        startLevelIndex = FindObjectOfType<Level>().GetStartLevelIndex(chapterIndex);
        levelsInChapter = FindObjectOfType<Level>().GetStartLevelIndex(chapterIndex + 1) - startLevelIndex;
        for (int index = startLevelIndex; index < levelsInChapter + startLevelIndex; index++)
        {
            int row = (index - startLevelIndex) / numberOfButtonsInRow;
            int column = (index - startLevelIndex) % numberOfButtonsInRow;
            var pos = SetUp(row, column);
            var button = Instantiate(buttonPrefab, transform.position, Quaternion.identity, gameObject.transform);
            var buttonSizeY = button.GetComponent<RectTransform>().sizeDelta.y;
            button.GetComponent<RectTransform>().anchoredPosition = pos;
            button.GetComponent<SelectCanvasButton>().SetLevelIndex(index);
            var newScrollSizeY = -pos.y + 2 * screenTopOffset - buttonSizeY / 2;
            GetComponent<RectTransform>().sizeDelta = new Vector2(scrollSizeX, newScrollSizeY);
        }
    }

    private Vector3 SetUp(int row, int column)
    {
        var totalWidth = GetComponent<RectTransform>().rect.width;
        var buttonOffset = (totalWidth - scrollbarOffset - screenSideOffset * 2) / (numberOfButtonsInRow - 1);
        var xPos = screenSideOffset + buttonOffset * column;
        var yPos = -screenTopOffset - buttonOffset * row;
        return new Vector3(xPos, yPos);
    }

    public int GetNumberOfLevelsInChapter()
    {
        return levelsInChapter; 
    }
}
