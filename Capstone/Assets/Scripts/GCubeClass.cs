using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach ��Ͽ� ���� ��ũ��Ʈ (�׷����� ��带 �ǹ�)
public class GCubeClass : MonoBehaviour
{
    public List<GEdgeClass> outer; // ����� �ٸ� ��ϰ� ���� �� �� �ֵ����ϴ� �Ǹ���
    public List<GEdgeClass> inner; // ��ϰ� ����� �ٸ� ����� �Ǹ���
    public GameObject aBlock; // Attach ��� (�Ǹ����� ����Ǵ� ����� �Ʒ��κ�)
    public GameObject mBlock; // MainBlock - �Ǹ���+ť��+AttachBlock �� ��ģ��
    public List<GEdgeClass> current; // ���� ������ �Ǹ���

    // ó������ �����̴� ������� -> firstMove �� true �� �͸� ����� ��ϵ��� ��������̰� ��
    // �׷��������� A-B ����� ����� ���¿��� A �� �������� B �� �̵���Ű�� B �� ���������� �ٽ� A �� �����̴� ���� �ݺ��Ͽ�
    // �̻��� ������ ���ư�����
    public bool firstMove = false;
    // ��� ������� ���õ� ��ϰ� �׷��� ���� ��ϵ��� �Ÿ��� ����
    public Vector3 distance = new Vector3(0.0f, 0.0f, 0.0f);
    // BFS ���� �ش� ����� �̹� �ѹ� �湮�߾����� üũ�ϴ� ����
    public bool visit = false;

    private Vector3 prevPos = new Vector3(0.0f, 0.0f, 0.0f); // �̵����� ����ϱ� ���� ������ ��ġ�� ����
    private Vector3 prevRot = new Vector3(0.0f, 0.0f, 0.0f); // ȸ������ ����ϱ� ���� ������ ȸ������ ����
    private Quaternion prevRotQua = Quaternion.identity;

    private GSetPosition setPosition; // BFS ���ø��� ����ϱ� ���� ����
    private GRotationProcess rotationProcess; // BFS ���ø��� ����ϱ� ���� ����
    private GSetChangeTransform setChangeTransform; // BFS ���ø��� ����ϱ� ���� ����
    private GSetRotation setRotation;

    private void Start()
    {
        // ������ ���۵ɶ�  BFS ���ø��� ����ϱ� ���� ������ �ʱ�ȭ
        setPosition = GetComponent<GSetPosition>();
        rotationProcess = GetComponent<GRotationProcess>();
        setChangeTransform = GetComponent<GSetChangeTransform>();
        setRotation = GetComponent<GSetRotation>();
        // �׳� �����ϸ� hasChanged �� true �� �ٲ� -> (0,0,0) �� ����� �׷� �� ���� (��Ȯ�� ���� X)
        this.mBlock.transform.hasChanged = false;
    }
    public Vector3 getPrevRot()
    {
        return prevRot;
    }
    public Vector3 getPrevPos()
    {
        return prevPos;
    }
    public Quaternion getPrevRotQua()
    {
        return prevRotQua;
    }
    private void Update()
    {
        // ����� �̵��� �����鼭 firstMove �� Ȱ��ȭ �Ǿ� �ִ� ��쿡 ����� ��ϵ��� �׸�ŭ �Ȱ��� �̵���Ŵ
        if (this.mBlock.transform.hasChanged && this.firstMove == true)
        {
            //Debug.Log(mBlock.transform.name);
            // �̵��� ���ϰ� ����� ����� �׸�ŭ �̵���Ű��
            Vector3 moveOffset = this.mBlock.transform.position - prevPos;
            setPosition.bfs(this, moveOffset);

            // ȸ���� ���ϰ� ����� ����� �׸�ŭ ȸ����Ű��
            /*Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
            //Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);
            Vector3 rotateOffset = currentRot - prevRot;
            if (rotateOffset != new Vector3(0.0f, 0.0f, 0.0f))
            {
                rotationProcess.bfs(this, rotateOffset);
            }*/
            Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
            //Vector3 currentRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);
            Vector3 rotateOffset = currentRot - prevRot;
            if (rotateOffset != new Vector3(0.0f, 0.0f, 0.0f))
            {
                setRotation.bfs(this, this);
            }

            // �� ��� + ����� ����� hasChanged ��Ȱ��ȭ
            this.mBlock.transform.hasChanged = false;
            setChangeTransform.bfs(this, null);
        }
        prevPos = this.mBlock.transform.position;
        prevRot = Quaternion.ToEulerAngles(this.mBlock.transform.rotation);
        prevRotQua = this.mBlock.transform.rotation;
        /*prevPos = this.mBlock.transform.localPosition;
        prevRot = Quaternion.ToEulerAngles(this.mBlock.transform.localRotation);*/
    }

}