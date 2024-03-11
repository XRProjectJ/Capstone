using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BFS �� ���鼭 ����� ��ϵ��� ȸ�� ��Ŵ

public class GSetRotation : BfsGCube
{
    // ȸ�� ��Ű�� ��� 3����
    // 1. (�κ��� ����) ȸ�� �߽� ��ü�� �̵���Ų �� ȸ�� ��Ų �� ȸ�� �� ȸ������ �������� ����ġ
    // -> ȸ�� ���� �ٲ�� �̻��� ȸ���� �̷����
    // -> ���Ϸ� ������ �߰��� ��ȭ�� �༭ ����� ������ �����ϰ� ���ʹϾ����θ� �õ��غ������� ������ ����
    // 2. (����) ȸ�� �߽� ��ü�� �߽����� �� � ��Ű�� (ȸ���� �� �� ��̶�� �������� ����)
    // -> ����� ��ü�� ���� ��ġ ������ �Ϻ� �ҽǵǴ� ����
    // 3. (����) opengl ���� ó�� ȸ�� �߽� ��ü�� ���庯ȯ ��Ŀ� ����� ��ü�� ���庯ȯ ��� ���ϱ�
    // -> ����Ƽ���� ��� �ڵ�� �ű�� �� �𸣰��� 
    override public void bfsMainFunction(GCubeClass obj, GCubeClass root)
    {

        obj.firstMove = false;
        obj.mBlock.transform.Translate(obj.distance);
        Quaternion prevRot = root.getPrevRotQua();
        // ������ �� ȸ���� ����� �� ���ο� ȸ���� ���� => ���⼭ ������ ����� ���� : ��ȭ�� ���ϴ� ���� �ʿ�
        /*Quaternion parentRot = Quaternion.Inverse(prevRot) * root.mBlock.transform.rotation;
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;*/
        
        // ���� ����� ȸ���� �ݿ�����
        obj.mBlock.transform.rotation = root.mBlock.transform.rotation;

        obj.mBlock.transform.Translate(-obj.distance);

    }
    public void bfsMainFunction2(GCubeClass obj, GCubeClass root)
    {
        obj.firstMove = false;
        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f) - Quaternion.ToEulerAngles(root.mBlock.transform.rotation);
        float posX = obj.distance.magnitude * Mathf.Cos(offset.y) * Mathf.Cos(offset.x);
        float posY = obj.distance.magnitude * Mathf.Sin(offset.x);
        float posZ = obj.distance.magnitude * Mathf.Sin(offset.y) * Mathf.Cos(offset.x);
        obj.mBlock.transform.position = root.mBlock.transform.position + new Vector3(posX, posY, posZ);
       
        Vector3 rotOffset = Quaternion.ToEulerAngles(root.mBlock.transform.rotation) - root.getPrevRot();
        Quaternion parentRot = Quaternion.EulerAngles(rotOffset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;

        //obj.mBlock.transform.localPosition += obj.distance;
    }
    public void bfsMainFunction3(GCubeClass obj, GCubeClass root)
    {
        // ��� ������ ���� �θ�-�ڽ� ���� �����ϱ�

        Matrix4x4 rootWorldMat = root.mBlock.transform.localToWorldMatrix;
        Matrix4x4 objWorldMat = obj.mBlock.transform.localToWorldMatrix;
        objWorldMat *= rootWorldMat;
        obj.firstMove = false;
        //obj.mBlock.transform.localToWorldMatrix = objWorldMat;

        /*Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f) - Quaternion.ToEulerAngles(root.mBlock.transform.rotation);
        float posX = obj.distance.magnitude * Mathf.Cos(offset.y) * Mathf.Cos(offset.x);
        float posY = obj.distance.magnitude * Mathf.Sin(offset.x);
        float posZ = obj.distance.magnitude * Mathf.Sin(offset.y) * Mathf.Cos(offset.x);
        obj.mBlock.transform.position = root.mBlock.transform.position + new Vector3(posX, posY, posZ);

        Vector3 rotOffset = Quaternion.ToEulerAngles(root.mBlock.transform.rotation) - root.getPrevRot();
        Quaternion parentRot = Quaternion.EulerAngles(rotOffset);
        Quaternion myRot = obj.mBlock.transform.rotation;
        obj.mBlock.transform.rotation = parentRot * myRot;*/

        //obj.mBlock.transform.localPosition += obj.distance;
    }
}
