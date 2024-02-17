using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesManager : MonoBehaviour
{
    public Animator animator;
    public bool showObjectives = false;
    public TextMeshProUGUI openCloseText;
    // Start is called before the first frame update
    void Start()
    {
        showObjectives = true;
    }

    public void ToggleObjectives(){
        showObjectives = !showObjectives;
        animator.SetBool("Showing", showObjectives);
        if(showObjectives){
            openCloseText.text = "-";
        }
        else{
            openCloseText.text = "+";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
