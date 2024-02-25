using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigNumberHandler : MonoBehaviour
{
    public string[] numberSuffixes;
    public string ConvertToString(float number){
        int index = 0;
        while(number >= 1000){
            number /= 1000;
            index++;
        }
        number = Mathf.Round(number * 100) / 100f;
        return number.ToString() + " " + numberSuffixes[index];
    }
}