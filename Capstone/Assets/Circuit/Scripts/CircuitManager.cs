using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{

    private List<double> R = new List<double>();
    private List<double> V = new List<double>();
    private List<double> I = new List<double>();
    
    public void calcEntireR(ComponentClass root)
    {
        if (root == null) return;

        Stack<ComponentClass> s = new Stack<ComponentClass>();
        double r = 0;
        s.Push(root);
        while(s.Count > 0)
        {
            ComponentClass cur = s.Pop();
            r += cur.GetR();
            if (cur.plus.GetIsParallel())
            {
                R.Add(r);
                r = 0;
            }
            if(cur.minus.links.Count == 1 && cur.minus.links[0].GetIsParallel())
            {

            }
            if (cur.minus.GetIsParallel())
            {
                
            }
            for (int i = 0; i < cur.plus.links.Count; i++)
            {
                ComponentClass tmp = cur.plus.links[i].GetComponent();
                if(tmp.GetVisit() == false)
                {
                    s.Push(tmp);
                    tmp.SetVisit(true);
                }
                
            }

        }

    }
}
