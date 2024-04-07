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

    //���� �����̴� ����
    public void LevelControl() 
    {
        if(audioManager != null)
            audioManager.PlaySfx(AudioManager.Sfx.LevelControlSfx);
    }

    #region ���� ����, ���� �ۼ�Ʈ �����ֱ� ����
    [Header("�ε� �г�")]
    public Image loadPanel;
    public int maxCreatureCount; //Ÿ�� �Ŵ����� ui�� ���ļ� ����
    public void StartGame()
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        // ���� ���� �Լ� ȣ��
        StartCoroutine(StartFadeOut());

        //StartActualGame();
    }
    #endregion


    Color fadeColor;
    IEnumerator StartFadeOut()//���̵� �ƿ��� ������(���������� ��)
    {
        // �ε� ������ Ȱ��ȭ
        loadPanel.gameObject.SetActive(true);

        fadeColor = loadPanel.color;
        float time = 1, minTime = 0;

        while (time > minTime)
        {
            time -= Time.deltaTime;
            float t = time / 1;//��� �ð�

            fadeColor.a = Mathf.Lerp(1, 0, t);
            loadPanel.color = fadeColor;

            yield return null;
        }
        StartActualGame();
    }

    // ���� ���� ���� �Լ�
    void StartActualGame()
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
    void SpawnCreature(int _index) //_index: ���������� �Ʒ��� �ִ� ��ư �� �� ��° ��ư����
    {
        for (int j = 0; j < 4; j++)
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();
            //Ȱ�� ���� ����
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������
            //SuperAgent superAgent = obj.GetComponent<SuperAgent>();
            //if (superAgent.useBullet != null)
            {

            }
        }
    }
    #endregion

    #region ���� ��ȯ
    void SpawnWeapon(int _index)//_index: �� ��° ��ư����
    {
        int mul = 1;
        if (spellBtnArr[_index].spellData.spellPrefab.name == gameManager.Gun.name)
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
