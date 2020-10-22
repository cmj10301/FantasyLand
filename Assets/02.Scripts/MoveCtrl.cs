using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour 
{
    public enum MoveType
    {
        WAY_POINT,
        LOOK_AT,
        GEAR_VR
    }
    public MoveType moveType = MoveType.LOOK_AT;
    public float speed = 1.0f;
    public float damping = 3.0f;

    //모든 웨어포인트 저장할 배열
    public Transform[] points;
    private Transform tr;
    private CharacterController cc;
    private Transform camTr;
    private int nextIdx = 1;

    public static bool isStopped = false;
    void Start()
    {
        tr = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
        camTr = Camera.main.GetComponent<Transform>();

        //WayPointGroup 게임오브젝트를 검색해 변수를 저장
        GameObject wayPointGroup = GameObject.Find("WayPointGroup");
        if (wayPointGroup != null)
        {
            //WayPointGroup 하위에 있는 모든 게임오브젝트의 Transform 컴포넌트 추출
            points = wayPointGroup.GetComponentsInChildren<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped) return;
        switch (moveType)
        {
            case MoveType.WAY_POINT:
                MoveWayPoint();
                break;
            case MoveType.LOOK_AT:
                MoveLookAt(1);
                break;
            case MoveType.GEAR_VR:
                break;
        }
    }
    void MoveWayPoint()
    {
        Vector3 direction = points[nextIdx].position - tr.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    void MoveLookAt(int facing)
    {
        Vector3 heading = camTr.forward;
        heading.y = 0.0f;
        Debug.DrawRay(tr.position, heading.normalized * 0.1f, Color.red);

        cc.SimpleMove(heading * speed * facing);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("WAY_POINT"))
        {
            nextIdx = (++nextIdx >= points.Length ? 1 : nextIdx);
        }
    }
}
