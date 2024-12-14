using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUI : MonoBehaviour
{
    [SerializeField, Tooltip("Default: Start")]
    private OVRInput.Button _button = OVRInput.Button.Start;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(_button))
        {
            GameObject UI = this.gameObject.transform.GetChild(0).gameObject;
            UI.SetActive(!UI.activeSelf);
        }
    }
}
