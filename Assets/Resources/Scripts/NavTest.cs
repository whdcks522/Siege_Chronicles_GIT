using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NavTest : MonoBehaviour
{
    NavMeshAgent agent;
    //Animator anim;
    LineRenderer lr;
    Coroutine draw;


    public Transform spot;
    //public NavMeshSurface nms;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //anim = GetComponent<Animator>();

        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material.color = Color.green;
        lr.enabled = false;
    }

    //private void Start()
    //{
        //nms.BuildNavMesh();//surface에서 시작 시 동적으로 베이크------------------------------
    //}

    void Update()
    {
        //마우스 클릭
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //목적지 설정
                agent.SetDestination(hit.point);

                //애니메이션 실행
                //anim.SetFloat("Speed", 2.0f);
                //anim.SetFloat("MotionSpeed", 2.0f);

                //마크 표시 설정
                spot.gameObject.SetActive(true);
                spot.position = hit.point;

                //이미 실행중이라면 종료
                if (draw != null) StopCoroutine(draw);
                //경로 보이게 라인 렌더러 코루틴실행
                draw = StartCoroutine(DrawPath());
            }
        }
        //도착함
        else if (agent.remainingDistance < 0.1f)
        {
            //애니메이션
            //anim.SetFloat("Speed", 0f);
            //anim.SetFloat("MotionSpeed", 0f);

            //마크 표시 설정
            spot.gameObject.SetActive(false);

            //라인 렌더러 종료
            lr.enabled = false;
            if (draw != null) //정지중 다시 가려고하면 자동으로 꺼짐 방지
                StopCoroutine(draw);//시작했던 코루틴 종료-----------------------------------
        }
    }

    IEnumerator DrawPath()
    {
        lr.enabled = true;
        yield return null;
        while (true)
        {
            int cnt = agent.path.corners.Length;//가는 경로를 점으로 표기했을 때, 점의 갯수
            lr.positionCount = cnt;
            for (int i = 0; i < cnt; i++)
            {
                lr.SetPosition(i, agent.path.corners[i]);//점들을 표기
            }
            yield return null;
        }
    }
}
