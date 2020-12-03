using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageDisplay : MonoBehaviour
{
    public Text msgTxt;
    public Button dismissBtn;
    private logManager lm;

    private void Start()
    {
        lm = GameObject.FindGameObjectWithTag("LM").GetComponent<logManager>();
    }

    public void DisplayMessage(string msgToShow,UnityEngine.Events.UnityAction actionOnDismiss)
    {
        msgTxt.text = msgToShow;
        lm.updateLog(msgToShow);
        dismissBtn.onClick.AddListener(actionOnDismiss);
    }
}
