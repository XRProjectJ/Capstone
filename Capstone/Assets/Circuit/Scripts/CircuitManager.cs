using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();


    public double calcR(ComponentClass root, ComponentClass node)
    {
        if (node == null) return 0;
        if (node == root) return 0;
        if (node.plus.GetIsEndOfParallel())
        {
            return node.GetR();
        }
        if (node.plus.GetIsStartOfParallel())
        {
            double tmp = 0;
            for(int i=0; i < node.plus.links.Count; i++)
            {
                tmp += 1/calcR(root, node.plus.links[i].GetComponent());
            }
            return node.GetR() + 1 / tmp;
        }
        else
        {
            return node.GetR() + calcR(root, node.plus.links[0].GetComponent());
        }
        
    }
}
