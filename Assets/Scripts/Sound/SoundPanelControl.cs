using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanelControl : MonoBehaviour
{ 
    public void SoundPanelSwichButton()
    {
        if (this.gameObject.activeSelf == true)
        {
            this.gameObject.SetActive(false);
        }
        else if(this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
    }
}
