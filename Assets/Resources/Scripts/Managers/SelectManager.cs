using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("���õ� ���� ��ư")]
    public SpellButton selectedSpellBtn;

    [Header("���õ� ���� ������")]
    public Image selectedSpellIcon;

    [Header("���õ� ���� �̸�")]
    public Text selectedSpellName;

    [Header("���õ� ���� Ÿ��")]
    public Text selectedSpellType;

    [Header("���õ� ���� ���")]
    public Text selectedSpellValue;

    [Header("���õ� ���� ����")]
    public Text selectedSpellDesc;

    [Header("�����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("������ �г�")]
    public GameObject rightPanel;


    [Header("���� �����̴�")]
    public Slider levelSlider;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager uiManager;
    AudioManager audioManager;
    ObjectManager objectManager;
    
    void Awake()
    {
        uiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
        objectManager = gameManager.objectManager;

        //������ �г� ��Ȱ��ȭ
        rightPanel.SetActive(false);
    }

    public void StartGame()//���� ����, ���� �ۼ�Ʈ �����ֱ� ����
    {
        //UI ��Ȱ��ȭ
        gameObject.SetActive(false);

        //���� ���� ����
        gameManager.gameLevel = (int)levelSlider.value;

        //battleUI�� ���� ����
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//������ �ִ� ��� �̹��� ����
            {
                //��ư�� ���絥���� ����
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);

                //������Ʈ Ǯ���� ���� �̸� ����
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//����ü�� ���
                {
                    for (int j = 0; j < 8; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
                        Creature creature = obj.GetComponent<Creature>();
                        //Ȱ�� ���� ����
                        creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������
                        SuperAgent superAgent = obj.GetComponent<SuperAgent>();
                        //superAgent.useBullet
                    }
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//������ ���
                {
                    for (int j = 0; j < 8; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
                        Bullet bullet = obj.GetComponent<Bullet>();
                        if (bullet.endBullet != null)//�ڽ� �Ѿ˵� ����
                            objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
                    }
                }
            }
            else if (spellBtnArr[i].spellData == null)//���� ��� ��ư ��Ȱ��ȭ
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        //���� ȯ�� �ʱ�ȭ
        gameManager.resetEnv();
    }



    public void RestartGame()//���� �ʱ�ȭ
    {
        SceneManager.LoadScene(0);
    }
}
