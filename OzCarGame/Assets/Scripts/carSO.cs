using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Car", menuName = "Car")]
public class carSO : ScriptableObject
{
    public string carName;
    public string Speed;
    public string Accel;
    public Sprite carImage;
    public GameObject carPrefab;
}
