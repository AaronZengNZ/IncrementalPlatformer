using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    //accept a list of fonts
    public List<TMP_FontAsset> fonts;
    public List<string> fontNames;
    public TMP_FontAsset currentFont;
    public string currentFontName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentFont();
        UpdateFont();
    }

    public void ChangeCurrentFont(string fontName){
        //change the current font to the font that is selected
        currentFontName = fontName;
        Debug.Log("changefont");
    }

    private void UpdateCurrentFont(){
        //update the current font to the font that is currently selected
        currentFont = fonts[fontNames.IndexOf(currentFontName)];
        UpdateFont();
    
    }

    private void UpdateFont(){
        //update the font of all text mesh pro objects that can be found
        foreach(TMP_Text text in FindObjectsOfType<TMP_Text>()){
            if(text.tag == "SetFont"){
                continue;
            }
            // [[[[[[[[[[IMPORTANT!!! DO NOW!!!!!!!!!!!!!! ONLY WITH BOLDTEXT TAG!!!!!!!]]]]]]]]]]
            //make every font's font style's uppercase be true and also make them bold
            if(text.tag == "BoldText"){
                text.font = currentFont;
                continue;
            }
            text.fontStyle = FontStyles.UpperCase;

            text.font = currentFont;
        }
    }
}
