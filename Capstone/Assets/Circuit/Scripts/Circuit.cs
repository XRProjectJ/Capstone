using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField] private ComponentClass root;
    // 병렬 내부의 직렬(가지)의 저항의 합을 계산
    private double calcRSerialInParallel(ComponentClass node)
    {
        if (node.plus.GetIsEndOfParallel())
        {
            return node.GetR();
        }
        return node.GetR() + calcRSerialInParallel(node.plus.links[0].GetComponent());
    }
    private double calcEntireR()
    {
        ComponentClass cur = root;
        double R = 0;
        for(int i=0; i< cur.plus.links.Count; i++)
        {
            
        }

        return R;
    }
}
