using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : ComponentClass
{
    public GameObject material;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {

        // LED의 원래 색상 저장
        originalColor = material.GetComponent<Renderer>().material.color;

        material.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
     public override void Do(double totalCurrent)
    {

        /*if (material != null)
        {

            material.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f);
        }
        else
        {
            Debug.LogWarning("Component_diode에 Material이 할당되지 않았습니다.");
        }*/

        // 원래 색상으로 돌아가기
        //material.GetComponent<Renderer>().material.color = originalColor;/

        /*// 전류의 세기 가져오기
        double current = GetI();

        // 전류의 세기에 따라 LED의 색상 조절
        Color newColor = originalColor * (float)current;
        material.GetComponent<Renderer>().material.color = newColor;*/




        // 부품 전류 
        double componentCurrent = GetI();

     
        double ratio = componentCurrent / totalCurrent;

        // LED의 색상 조절
        Color newColor = originalColor * (float)ratio;

        // LED 새로운 색상으로
        material.GetComponent<Renderer>().material.color = newColor;
    }

}



