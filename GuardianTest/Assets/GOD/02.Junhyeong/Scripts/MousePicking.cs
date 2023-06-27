using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 피킹관련 코드
/// 레이를 사용하여 오브젝트를 마우스에 붙인다.
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
    [Header("피킹했을때 얼마나 멀리있을 건지")]
    [SerializeField]
    float distance;

    [Header("마우스 땔 때 날릴 힘의 크기")]
    [SerializeField]
    float power;
    [SerializeField]
    float powerForBomb;

    float maxDistance = 1000f;
    Ray ray;
    RaycastHit hit;
    public LayerMask layerMask;

    // 마우스 관련
    private Vector3 mouseOffset;

    private float mZCoord;

    tempVector3 keepOBJpos;

    private bool targetOn = false;

    [Header("사운드파일명")]
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
            // 버튼을 때면 마우스가 있는 위치로 날린다.
            Vector3 offset = Input.mousePosition;

            ray = Camera.main.ScreenPointToRay(offset); // 원래는 temp대신 input.mousePosition

            if (!hit.transform) return; // 집고있을때 destroy 된경우 리턴시켜줌

            var hrb = hit.rigidbody;

            hrb.freezeRotation = false;

            hrb.velocity = Vector3.zero;
            hrb.angularVelocity = Vector3.zero;

            if (hit.transform.CompareTag("Bomb")) hrb.AddForce(ray.direction * powerForBomb, ForceMode.Impulse);
            else hrb.AddForce(ray.direction * power, ForceMode.Impulse);

            targetOn = false;

            SoundMG.Instance.PlaySFX(throwSomething);
            // 버튼을 때면 가지고 원래 있었던 방향으로 날린다./*ToObjectVector(keepOBJpos) */
        }
    }
    void HoldKey()
    {
        // 오브젝트를 누른상태라면 마우스의 위치에 오브젝트를 위치시킨다.
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

                // 오브젝트가 원래 위치했던 곳을 저장한다.
                keepOBJpos.SetPosition(hit.transform.position);

                //일단 카메라앞으로 붙인다.
                hit.transform.position = hit.transform.position + ToCameraVector(hit);

                //카메라부터 distance만큼 원래 오브젝트가 존재하던 방향으로 밀어낸다.
                int counting = 0;

                while (counting < distance)
                {
                    hit.transform.position = hit.transform.position + ToObjectVector(keepOBJpos);
                    counting++;
                }

                //Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

                mZCoord = Camera.main.WorldToScreenPoint(hit.transform.position).z;

                mouseOffset = hit.transform.position - GetMouseWorldPos();


                /// 피킹시점에 카메라를 바라보도록
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
        //return length.normalized;   // 오브젝트로부터 카메라를 보는 크기가 1인 vector 
    }

    private Vector3 ToObjectVector(tempVector3 targetObject)
    //카메라와 오브젝트의 방향과 길이를 구해보자
    {
        Vector3 length;
        length = targetObject.GetPosition() - Camera.main.transform.position;
        return length.normalized;// 노말라이즈해서 방향을 구한다  카메라에서부터 오브젝트를 보는 크기가 1인 vector가 나올것이다.
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;           // 기본 마우스 포지션;

        mousePoint.z = mZCoord;                             // 카메라의 포지션을 월드 -> 스크린으로 바꾼후 의 Z값;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}

