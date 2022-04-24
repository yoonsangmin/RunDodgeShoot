using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInputField : MonoBehaviour
{
    public Button toTitleButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<InputField>().text == "")
            toTitleButton.interactable = false;
        else
            toTitleButton.interactable = true;
    }
}
