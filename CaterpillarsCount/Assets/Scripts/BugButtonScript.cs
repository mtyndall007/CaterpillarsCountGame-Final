using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class BugIDEvent : UnityEvent<string>
{
}

public class BugButtonScript : MonoBehaviour
{

    public Button bugButton;
    public BugClickedEvent bugClicked;

    void Start()
    {
        bugButton.onClick.AddListener(() => ButtonClicked(EventSystem.current.currentSelectedGameObject.name));

        ColorBlock theColor = bugButton.colors;
        theColor.highlightedColor = Color.cyan;
        theColor.normalColor = bugButton.colors.normalColor;
        theColor.pressedColor = Color.blue;
        bugButton.colors = theColor;
    }

    private void ButtonClicked(string bugType)
    {
        GameManager.instance.BugSelectionUI(bugType);

    }
}
