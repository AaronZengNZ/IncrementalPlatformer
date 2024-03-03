using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public Animator[] animators;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggleHovered(int index)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (i == index)
            {
                animators[i].SetBool("Hover", true);
            }
        }
    }
    public void ToggleUnhovered(int index)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (i == index)
            {
                animators[i].SetBool("Hover", false);
            }
        }
    }
}
