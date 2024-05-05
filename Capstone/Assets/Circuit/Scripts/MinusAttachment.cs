using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  - 극을 의미하는 Attachment 클래스
public class MinusAttachment : Attachment
{
    // - 극에는 + 극을 가진 것만 달라붙을 수 있음
    public List<PlusAttachment> links = new List<PlusAttachment>();
    // 해당 객체를 가진 부품의 + 극을 저장 (24.04.01 기준 사용안함)
    [SerializeField] protected PlusAttachment pair;
    public bool isEndOfParallel = false;

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<PlusAttachment>());

        // 여러 부품들이 달라 붙어있는 경우는 병렬임
        // 특히 - 극에 여러 부품이 달라붙어 있는 경우는 병렬이 끝나는 지점을 의미
        if (links.Count > 1)
        {
            for (int i = 0; i < links.Count; i++)
            {
                links[i].GetComponent<PlusAttachment>().SetIsEndOfParallel(true);
                links[i].GetComponent<PlusAttachment>().SetEndOfParallelLink(this);
            }
            isEndOfParallel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<PlusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<PlusAttachment>());
        // 연결이 끊어졌으면 병렬처리된 것들을 전부 병렬이 아니라고 다시 바꿔줘야함
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
