using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : ComponentClass
{
    public GameObject material;

    // Start is called before the first frame update
    void Start()
    {
         Do();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
     public override void Do()
    {
        /*// off된 led 에셋을 찾음
        GameObject offLed = GameObject.Find("LED Red");

        // on된 led 에셋을 찾음
        GameObject onLed = GameObject.Find("LED Red ON");

        // off된 led를 비활성화
        offLed.SetActive(false);

        // on된 led를 활성화
        onLed.SetActive(true);*/

        
       

        if (material != null)
        {

            material.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f);
        }
        else
        {
            Debug.LogWarning("Component_diode에 Material이 할당되지 않았습니다.");
        }
    
    }


}
