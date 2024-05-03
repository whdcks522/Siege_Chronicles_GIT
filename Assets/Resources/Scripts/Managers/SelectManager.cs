using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    public Animator anim;

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
        anim.SetBool("isSelect", true);
    }

    //���� �����̴� ����
    public void LevelControl() 
    {
        if(audioManager != null)
            audioManager.PlaySfx(AudioManager.Sfx.LevelControlSfx);
    }

    #region ���� ����, ���� �ۼ�Ʈ �����ֱ� ����
    [Header("�ε� �г�")]
    public int index_Battle;//���� â �ø��� �ִϸ��̼� ��, ���� ������ �̵��� ������, �ƴϸ� ������ ������
    //0: �⺻, 1: ���� ����, 2: ���� �ʱ�ȭ,

    public void StartGame(int i)//����â���� ���� ��ư �Ǵ�, �ʱ�ȭ ��ư Ŭ��
    {
        //�ִϸ��̼� �� ������ ��, �ʱ�ȭ ���� ����
        index_Battle = i;

        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        // ���� ���� �Լ� ȣ��
        anim.SetBool("isSelect", false);
    }
    #endregion

    public void StartActualGame()// ���� ���� ���� �Լ�
    {
        //battleUI�� ���� ����
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//������ �ִ� ��� �̹��� ����
            {
                //��ư�� ���� ������ ����
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                //�ش� ��ư�� �̹��� ����
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);


                //������Ʈ Ǯ���� ���� �̸� ����
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//����ü�� ���
                {
                    SpawnCreature(i);
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//������ ���
                {
                    SpawnWeapon(i);
                }
            }
            else if (spellBtnArr[i].spellData == null)//���� ��� ��ư ��Ȱ��ȭ
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }

        //���� ȯ�� �ʱ�ȭ
        gameManager.RetryGame();

        // UI ��Ȱ��ȭ
        gameObject.SetActive(false);

        // ���� ���� ����
        gameManager.gameLevel = (int)levelSlider.value;
    }

    #region ũ���� ��ȯ
    int spawnCreatureCount = 2;//��ȯ�ϴ� ũ������ ��
    void SpawnCreature(int _index) //_index: ���������� �Ʒ��� �ִ� ��ư �� �� ��° ��ư����
    {
        for (int j = 0; j < spawnCreatureCount; j++)//ũ���� ���� spawnCreatureCount���� ��ȯ
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();

            //Ȱ�� ���� ����
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������

            SuperAgent superAgent = obj.GetComponent<SuperAgent>();
            if (superAgent.useBullet != null)
            {

            }
        }
    }
    #endregion

    #region �ּ� ��ȯ
    void SpawnWeapon(int _index)//_index: �� ��° ��ư����
    {
        int mul = 1;
        if (spellBtnArr[_index].spellData.spellPrefab.name == gameManager.Gun.name)//����� ���� ������Ʈ�� �߰��� ����
            mul = 3;

        for (int j = 0; j < 4 * mul; j++)
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet.endBullet != null)//�ڽ� �Ѿ˵� ����
                objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
        }
    }
    #endregion
}
