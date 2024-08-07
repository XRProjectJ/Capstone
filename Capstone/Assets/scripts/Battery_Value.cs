using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Battery_Value : MonoBehaviour
{

    public GameObject Battery_Object;

    public TextMeshProUGUI registance_text;
    public TextMeshProUGUI voltage_text;
    public TextMeshProUGUI intense_text;


    void Start()
    {

        Power batteryScript = Battery_Object.GetComponent<Power>();

        if (batteryScript != null)
        {
            // LED_Blue 스크립트에서 ShowR 값을 가져와서 UI Text에 표시
            registance_text.text = "저항: " + batteryScript.GetShowR().ToString();
            voltage_text.text = "전압: " + batteryScript.GetShowV().ToString();
            intense_text.text = "전류: " + batteryScript.GetShowI().ToString();

        }
        else
        {
            Debug.LogError("Battery 스크립트를 찾을 수 없습니다.");
        }
    }

    private void Update()
    {
        Power batteryScript = Battery_Object.GetComponent<Power>();
        if (batteryScript != null)
        {
            // LED_Blue 스크립트에서 ShowR 값을 가져와서 UI Text에 표시
            registance_text.text = "저항: " + batteryScript.GetShowR().ToString();
            voltage_text.text = "전압: " + batteryScript.GetShowV().ToString();
            intense_text.text = "전류: " + batteryScript.GetShowI().ToString();

        }
        else
        {
            Debug.LogError("Battery 스크립트를 찾을 수 없습니다.");
        }
    }
}
