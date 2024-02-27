using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigNumberHandler : MonoBehaviour
{
    public string[] numberSuffixes;
    public string ConvertToString(float number, float decimals = 1f){
        int index = 0;
        while(number >= 1000){
            number /= 1000;
            index++;
        }
        number = Mathf.Round(number * 100 * decimals) / 100f / decimals;
        if(index == 0f){
            number = Mathf.Round(number * decimals) / decimals;
            return number.ToString();
        }
        return number.ToString() + " " + numberSuffixes[index];
    }
}
