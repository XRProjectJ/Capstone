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
            // LED_Blue ��ũ��Ʈ���� ShowR ���� �����ͼ� UI Text�� ǥ��
            registance_text.text = "����: " + batteryScript.showR.ToString();
            voltage_text.text = "����: " + batteryScript.showV.ToString();
            intense_text.text = "����: " + batteryScript.showI.ToString();

        }
        else
        {
            Debug.LogError("Battery ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }
    }
}
