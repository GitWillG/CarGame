using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class logManager : MonoBehaviour
{
    [Header("Battle Log")]
    public GameObject logAligner;
    public GameObject logText;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateLog(string logContent)
    {
        GameObject tempLog = Instantiate(logText, logAligner.transform);
        tempLog.transform.parent = logAligner.transform;
        tempLog.transform.SetAsFirstSibling();

        Text textData = tempLog.GetComponent<Text>();
        textData.text = "[Log]: " + logContent;

        // Debug.Log("New log entered :" + textData.text);
    }
}
