using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class logManager : MonoBehaviour
{
    [Header("Battle Log")]
    public GameObject logAligner;
    public GameObject logText;
    private static logManager _instance;
    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            //Debug.Log("Instance was found");
        }
        else
        {
            _instance = this;
            //Debug.Log("Instance was not found");
            DontDestroyOnLoad(gameObject);
        }

        //DontDestroyOnLoad(this.gameObject);

    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            //Debug.Log("Instance was found");
        }
        else
        {
            _instance = this;
            //Debug.Log("Instance was not found");
            DontDestroyOnLoad(gameObject);
        }
   

    }

    // Update is called once per frame
    void Update()
    {
        if (logAligner == null && GameObject.FindGameObjectWithTag("Aligner") !=false)
        {
            logAligner = GameObject.FindGameObjectWithTag("Aligner").transform.GetChild(1).GetChild(0).gameObject;
        }
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
