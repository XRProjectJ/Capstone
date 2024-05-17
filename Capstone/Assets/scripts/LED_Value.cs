using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LED_Value : MonoBehaviour
{
    public GameObject LED_Object;

    public TextMeshProUGUI registance_text;
    public TextMeshProUGUI voltage_text;
    public TextMeshProUGUI intense_text;
    

    void Start()
    {

        LED ledScript = LED_Object.GetComponent<LED>();

        if (ledScript != null)
        {
            // LED_Blue 스크립트에서 ShowR 값을 가져와서 UI Text에 표시
            registance_text.text = "저항: " + ledScript.GetShowR().ToString();
            voltage_text.text = "전압: " + ledScript.GetShowV().ToString();
            intense_text.text = "전류: " + ledScript.GetShowI().ToString();

        }
        else
        {
            Debug.LogError("LED 스크립트를 찾을 수 없습니다.");
        }
    }
}
