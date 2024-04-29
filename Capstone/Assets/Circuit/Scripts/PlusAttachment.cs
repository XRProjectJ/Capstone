using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// + 극을 의미하는 Attachment 클래스
public class PlusAttachment : Attachment
{
    // + 극에는 - 극만 달라붙어야함
    public List<MinusAttachment> links = new List<MinusAttachment>();
    // 해당 객체를 가진 부품의 - 극을 저장 (24.04.01 기준 사용안함)
    [SerializeField] protected MinusAttachment pair;
    // 병렬의 끝인지 저장
    public bool isEndOfParallel = false;
    // 병렬의 시작인지 저장
    public bool isStartOfParallel = false;
    // 병렬의 끝이라면 병렬의 끝이 어디로 이어지는 지 저장
    public MinusAttachment endOfParallelLink = null;


    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Add(obj.GetComponent<MinusAttachment>());
        linkSize++;
        // + 극에 여러 - 극이 달라붙었다면 병렬을 의미
        // 특히 + 극은 병렬의 시작을 의미함
        if (linkSize > 1)
        {
            isStartOfParallel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.GetComponent<MinusAttachment>() == null)
        {
            return;
        }
        links.Remove(obj.GetComponent<MinusAttachment>());
        linkSize--;

        // 연결된 것이 1개 이하면 더이상 병렬이 아님 
        if (linkSize <= 1)
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
