using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���콺 ��ŷ���� �ڵ�
/// ���̸� ����Ͽ� ������Ʈ�� ���콺�� ���δ�.
/// </summary>
struct tempVector3
{
    public float x;
    public float y;
    public float z;

    public void SetPosition(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3 GetPosition()
    {
        Vector3 temp;
        temp.x = this.x;
        temp.y = this.y;
        temp.z = this.z;

        return temp;
    }
}
public class MousePicking : MonoBehaviour
{
    [Header("��ŷ������ �󸶳� �ָ����� ����")]
    [SerializeField]
    float distance;

    [Header("���콺 �� �� ���� ���� ũ��")]
    [SerializeField]
    float power;
    [SerializeField]
    float powerForBomb;

    float maxDistance = 1000f;
    Ray ray;
    RaycastHit hit;
    public LayerMask layerMask;

    // ���콺 ����
    private Vector3 mouseOffset;

    private float mZCoord;

    tempVector3 keepOBJpos;

    private bool targetOn = false;

    [Header("�������ϸ�")]
    [SerializeField] string mouseClick;
    [SerializeField] string throwSomething;

    private void Awake()
    {
        hit = new RaycastHit();
        ray = new Ray();
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void Update()
    {
        PickKey();

        HoldKey();

        ThrowKey();
    }
    void ThrowKey()
    {
        if (targetOn == true && Input.GetMouseButtonUp(0))
        {
            // ��ư�� ���� ���콺�� �ִ� ��ġ�� ������.
            Vector3 offset = Input.mousePosition;

            ray = Camera.main.ScreenPointToRay(offset); // ������ temp��� input.mousePosition

            if (!hit.transform) return; // ���������� destroy �Ȱ�� ���Ͻ�����

            var hrb = hit.rigidbody;

            hrb.freezeRotation = false;

            hrb.velocity = Vector3.zero;
            hrb.angularVelocity = Vector3.zero;

            if (hit.transform.CompareTag("Bomb")) hrb.AddForce(ray.direction * powerForBomb, ForceMode.Impulse);
            else hrb.AddForce(ray.direction * power, ForceMode.Impulse);

            targetOn = false;

            SoundMG.Instance.PlaySFX(throwSomething);
            // ��ư�� ���� ������ ���� �־��� �������� ������./*ToObjectVector(keepOBJpos) */
        }
    }
    void HoldKey()
    {
        // ������Ʈ�� �������¶�� ���콺�� ��ġ�� ������Ʈ�� ��ġ��Ų��.
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (targetOn)
            {
                if (!hit.transform) return;
                hit.transform.position = GetMouseWorldPos() + mouseOffset;
            }
        }
    }
    void PickKey()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundMG.Instance.PlaySFX(mouseClick);

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                if (hit.transform.TryGetComponent<CKey>(out CKey _key))
                {
                    _key.Picking = true;
                }

                // ������Ʈ�� ���� ��ġ�ߴ� ���� �����Ѵ�.
                keepOBJpos.SetPosition(hit.transform.position);

                //�ϴ� ī�޶������ ���δ�.
                hit.transform.position = hit.transform.position + ToCameraVector(hit);

                //ī�޶���� distance��ŭ ���� ������Ʈ�� �����ϴ� �������� �о��.
                int counting = 0;

                while (counting < distance)
                {
                    hit.transform.position = hit.transform.position + ToObjectVector(keepOBJpos);
                    counting++;
                }

                //Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

                mZCoord = Camera.main.WorldToScreenPoint(hit.transform.position).z;

                mouseOffset = hit.transform.position - GetMouseWorldPos();


                /// ��ŷ������ ī�޶� �ٶ󺸵���
                //hit.transform.LookAt(Camera.main.transform);
                //
                hit.transform.rotation = Quaternion.Euler(Vector3.zero);

                var rigid = hit.transform.GetComponent<Rigidbody>();

                rigid.freezeRotation = true;

                targetOn = true;
            }
        }
    }
    private Vector3 ToCameraVector(RaycastHit targetObject)
    {
        Vector3 length;
        length = Camera.main.transform.position - targetObject.transform.position;
        return length;
        //return length.normalized;   // ������Ʈ�κ��� ī�޶� ���� ũ�Ⱑ 1�� vector 
    }

    private Vector3 ToObjectVector(tempVector3 targetObject)
    //ī�޶�� ������Ʈ�� ����� ���̸� ���غ���
    {
        Vector3 length;
        length = targetObject.GetPosition() - Camera.main.transform.position;
        return length.normalized;// �븻�������ؼ� ������ ���Ѵ�  ī�޶󿡼����� ������Ʈ�� ���� ũ�Ⱑ 1�� vector�� ���ð��̴�.
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;           // �⺻ ���콺 ������;

        mousePoint.z = mZCoord;                             // ī�޶��� �������� ���� -> ��ũ������ �ٲ��� �� Z��;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}

