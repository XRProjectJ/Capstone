using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Voltage_Value : MonoBehaviour
{
    public TextMeshProUGUI maintext;
    public GameObject LED_BLUE;

    void Start()
    {

        LED ledScript = LED_BLUE.GetComponent<LED>();

        if (ledScript != null)
        {
            // LED_Blue 스크립트에서 ShowR 값을 가져와서 UI Text에 표시
            maintext.text = "전압: " + ledScript.showV.ToString();
        }
        else
        {
            Debug.LogError("LED 스크립트를 찾을 수 없습니다.");
        }
    }
}
