using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  - ���� �ǹ��ϴ� Attachment Ŭ����
public class MinusAttachment : Attachment
{
    // - �ؿ��� + ���� ���� �͸� �޶���� �� ����
    public List<PlusAttachment> links = new List<PlusAttachment>();
    // �ش� ��ü�� ���� ��ǰ�� + ���� ���� (24.04.01 ���� ������)
    [SerializeField] protected PlusAttachment pair;
    private bool isEndOfParallel = false;

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<PlusAttachment>());


        // ���� ��ǰ���� �޶� �پ��ִ� ���� ������
        // Ư�� - �ؿ� ���� ��ǰ�� �޶�پ� �ִ� ���� ������ ������ ������ �ǹ�
        if (links.Count > 1)
        {
            for (int i = 0; i < links.Count; i++)
            {
                links[i].GetComponent<PlusAttachment>().SetIsEndOfParallel(true);
                links[i].GetComponent<PlusAttachment>().SetEndOfParallelLink(this);
            }
            isEndOfParallel = true;
        }
        /*Debug.Log("GetGrab() : " + obj.GetComponent<PlusAttachment>().GetComponent().GetGrab());
        if (obj.GetComponent<PlusAttachment>().GetComponent().GetGrab() == true)
        {
            Debug.Log("����ġ");
            //GameObject model = obj.GetComponent<PlusAttachment>().GetComponent().gameObject.transform.parent.gameObject;
            //Debug.Log("model : " + model);
            //GameObject attachment = obj.GetComponent<PlusAttachment>().gameObject;
            //Debug.Log("attachment : " + attachment);
            //Vector3 offset = attachment.transform.localPosition - model.transform.localPosition;
            //model.transform.localPosition += offset;

            Vector3 offset = obj.GetComponent<PlusAttachment>().GetOffset();
            obj.transform.position = this.transform.position;
            obj.transform.localPosition += offset;

        }*/


    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<PlusAttachment>());
        // ������ ���������� ����ó���� �͵��� ���� ������ �ƴ϶�� �ٽ� �ٲ������
        obj.GetComponent<PlusAttachment>().SetIsEndOfParallel(false);
        obj.GetComponent<PlusAttachment>().SetEndOfParallelLink(null);
        if(links.Count < 2)
        {
            isEndOfParallel = false;
        }
    }
    // Getter
    public PlusAttachment GetPair()
    {
        return pair;
    }
    public bool GetIsEndOfParallel()
    {
        return isEndOfParallel;
    }
    public void SetIsEndOfParallel(bool value)
    {
        isEndOfParallel = value;
    }
}
