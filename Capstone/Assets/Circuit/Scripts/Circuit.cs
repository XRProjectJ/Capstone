using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField] private ComponentClass root;
    // ���� ������ ����(����)�� ������ ���� ���
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
