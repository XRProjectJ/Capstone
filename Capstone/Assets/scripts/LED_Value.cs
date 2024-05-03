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
            // LED_Blue ��ũ��Ʈ���� ShowR ���� �����ͼ� UI Text�� ǥ��
            registance_text.text = "����: " + ledScript.showR.ToString();
            voltage_text.text = "����: " + ledScript.showV.ToString();
            intense_text.text = "����: " + ledScript.showI.ToString();

        }
        else
        {
            Debug.LogError("LED ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }
    }
}
