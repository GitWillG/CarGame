using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectCar : MonoBehaviour
{
    public Text nameText;
    public Text statText;
    public Image carImage;

    public List<carSO> cars;
    public int carNo;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("carManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    
    void start()
    {
        
    }
    public void nextCar()
    {
        if (carNo + 1 < cars.Count)
        {
            carNo++;
        }
        else
        {
            carNo = 0;
        }
        popCar();
    }
    public void lastCar()
    {
        if (carNo - 1 >= 0)
        {
            carNo--;
        }
        else
        {
            carNo = cars.Count -1;
        }
        popCar();
    }
    public void popCar()
    {
        nameText.text = cars[carNo].name;
        statText.text = "Speed:" + cars[carNo].Speed + " | Accel:" + cars[carNo].Accel;
        carImage.sprite = cars[carNo].carImage;

    }

}
