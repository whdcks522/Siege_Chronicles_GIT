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
        //nms.BuildNavMesh();//surface���� ���� �� �������� ����ũ------------------------------
    //}

    void Update()
    {
        //���콺 Ŭ��
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //������ ����
                agent.SetDestination(hit.point);

                //�ִϸ��̼� ����
                //anim.SetFloat("Speed", 2.0f);
                //anim.SetFloat("MotionSpeed", 2.0f);

                //��ũ ǥ�� ����
                spot.gameObject.SetActive(true);
                spot.position = hit.point;

                //�̹� �������̶�� ����
                if (draw != null) StopCoroutine(draw);
                //��� ���̰� ���� ������ �ڷ�ƾ����
                draw = StartCoroutine(DrawPath());
            }
        }
        //������
        else if (agent.remainingDistance < 0.1f)
        {
            //�ִϸ��̼�
            //anim.SetFloat("Speed", 0f);
            //anim.SetFloat("MotionSpeed", 0f);

            //��ũ ǥ�� ����
            spot.gameObject.SetActive(false);

            //���� ������ ����
            lr.enabled = false;
            if (draw != null) //������ �ٽ� �������ϸ� �ڵ����� ���� ����
                StopCoroutine(draw);//�����ߴ� �ڷ�ƾ ����-----------------------------------
        }
    }

    IEnumerator DrawPath()
    {
        lr.enabled = true;
        yield return null;
        while (true)
        {
            int cnt = agent.path.corners.Length;//���� ��θ� ������ ǥ������ ��, ���� ����
            lr.positionCount = cnt;
            for (int i = 0; i < cnt; i++)
            {
                lr.SetPosition(i, agent.path.corners[i]);//������ ǥ��
            }
            yield return null;
        }
    }
}
