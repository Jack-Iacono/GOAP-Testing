using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text actionText;
    public TMP_Text stateText;
    public TMP_Text goalStateText;
    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        stateText.text = "";
        actionText.text = "";
        goalStateText.text = "";
    }

    public void SetAction(string actionName)
    {
        actionText.text = "Current Action: " + actionName;
    }
    public void SetState(string stateName)
    {
        stateText.text = "Current:" + stateName;
    }
    public void SetGoal(string stateName)
    {
        goalStateText.text = "Goal:" + stateName;
    }
}
