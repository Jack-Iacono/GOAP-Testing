using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text actionText;
    public TMP_Text stateText;
    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        stateText.text = "";
        actionText.text = "";
    }

    public void SetAction(string actionName)
    {
        actionText.text = actionName;
    }
    public void SetState(string stateName)
    {
        stateText.text = stateName;
    }
}
