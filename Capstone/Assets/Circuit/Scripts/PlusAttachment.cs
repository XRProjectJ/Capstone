using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// + ���� �ǹ��ϴ� Attachment Ŭ����
public class PlusAttachment : Attachment
{
    // + �ؿ��� - �ظ� �޶�پ����
    public List<MinusAttachment> links = new List<MinusAttachment>();
    // �ش� ��ü�� ���� ��ǰ�� - ���� ���� (24.04.01 ���� ������)
    [SerializeField] protected MinusAttachment pair;
    // ������ ������ ����
    public bool isEndOfParallel = false;
    // ������ �������� ����
    public bool isStartOfParallel = false;
    // ������ ���̶�� ������ ���� ���� �̾����� �� ����
    public MinusAttachment endOfParallelLink = null;
    


    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<MinusAttachment>());
        // + �ؿ� ���� - ���� �޶�پ��ٸ� ������ �ǹ�
        // Ư�� + ���� ������ ������ �ǹ���
        if (links.Count > 1)
        {
            isStartOfParallel = true;
        }
        //if (other.GetComponent<MinusAttachment>().GetComponent().GetGrab() == true && other.GetComponent<Line>() != null)
        //{
        //    Vector3 newPos = other.GetComponent<MinusAttachment>().GetComponent().transform.localPosition - this.transform.localPosition;
        //    other.GetComponent<MinusAttachment>().GetComponent().transform.localPosition += newPos;
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<MinusAttachment>());

        // ����� ���� 1�� ���ϸ� ���̻� ������ �ƴ� 
        if (links.Count <= 1)
        {
            isStartOfParallel = false;
            isEndOfParallel = false;
        }
    }
    // Getter, Setter
    public MinusAttachment GetPair()
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
    public bool GetIsStartOfParallel()
    {
        return isStartOfParallel;
    }
    public void SetIsStartOfParallel(bool value)
    {
        isStartOfParallel = value;
    }
    public MinusAttachment GetEndOfParallelLink()
    {
        return endOfParallelLink;
    }
    public void SetEndOfParallelLink(MinusAttachment value)
    {
        endOfParallelLink = value;
    }
}
