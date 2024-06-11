using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;
using Unity.VisualScripting;

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
    public Image selectedSpellTypeImage;
    public Sprite[] spellTypeImageArr;//�ٲ� �̹����� ��� �ִ���
    public Text selectedSpellTypeText;

    [Header("���õ� ���� ���")]
    public Text selectedSpellValue;

    [Header("���õ� ���� ����")]
    public Text selectedSpellDesc;

    [Header("�����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

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
    [Header("�ִϸ��̼� �� ���������� �̵��� ������, �ʱ�ȭ�� ������")]
    public int index_Battle;//0: �⺻, 1: ���� ����, 2: ���� �ʱ�ȭ,

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
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//ũ������ ���
                {
                    //���̴� ��� ����
                    //uiManager.spellBtnArr[i].spellBtnShader.material = gameManager.SpellCreatureMat;
                    uiManager.spellBtnArr[i].spellBtnShader.gameObject.SetActive(true);

                    if (i == 0)
                    {
                        Debug.Log("0����");

                        //uiManager.spellBtnArr[i].spellBtnShader.materialForRendering.SetColor("_BaseColor", Color.red);
                        //uiManager.spellBtnArr[i].spellBtnShader.materialForRendering.SetColor("_AddColor", Color.blue);
                    }
                    else 
                    {
                        Debug.Log("������ ��°");

                        //uiManager.spellBtnArr[i].spellBtnShader.materialForRendering.SetColor("_BaseColor", Color.yellow);
                        //uiManager.spellBtnArr[i].spellBtnShader.materialForRendering.SetColor("_AddColor", Color.red);
                    }
                    
                    //���͸��� ����
                    //skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

                    //ũ���� �̸� ����
                    SpawnCreature(spellBtnArr[i].spellData.spellPrefab.name);
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//������ ���(�� �� ��찡 ���� �� �־)
                {
                    //���̴� ��� ����
                    //uiManager.spellBtnArr[i].spellBtnShader.material = gameManager.SpellWeaponMat;
                    uiManager.spellBtnArr[i].spellBtnShader.gameObject.SetActive(true);

                    //���� �̸� ����
                    SpawnWeapon(spellBtnArr[i].spellData.spellPrefab.name);
                }
            }
            else if (spellBtnArr[i].spellData == null)//���� ��� ��ư ��Ȱ��ȭ
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }

        if (gameManager.isEnemySpawn)//���� ��ȯ�ϴ� ���¸�, �߰��� ��ȯ
        {
            for (int i = 0; i < gameManager.creatureSpellDataArr.Length; i++)
            {
                SpawnCreature(gameManager.creatureSpellDataArr[i].spellPrefab.name);
            }
        }
        
        //���� ȯ�� �ʱ�ȭ
        gameManager.RetryGame();

        // ���� ���� ����
        gameManager.gameLevel = (int)levelSlider.value;

        // UI ��Ȱ��ȭ
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    #region ũ���� ��ȯ
    void SpawnCreature(string _str) //str ũ���ĸ� gameManager.maxCreatureCount���� ��ȯ
    {
        for (int j = 0; j < gameManager.maxCreatureCount; j++)//ũ���� ���� gameManager.maxCreatureCount���� ��ȯ
        {
            GameObject obj = objectManager.CreateObj(_str, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();

            //Ȱ�� ���� ����
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������

            SpawnWeapon(creature.useBullet.name);
        }
    }
    #endregion

    #region �ּ�(��ų) ��ȯ
    void SpawnWeapon(string _str)//str �ּ��� spawnCreatureCount���� ��ȯ
    {
        int mul = 2;
        //����� ���� ��ü
        if (_str == gameManager.Gun.name) mul = gameManager.maxCreatureCount;
        //��ü ������ �Ʊ� ��ü�� ����
        else if (_str == gameManager.CorpseExplosion.name) mul = gameManager.maxCreatureCount;
        //�������� 3�߾� ��
        else if (_str == gameManager.creatureSpellDataArr[1].spellPrefab.GetComponent<Creature>().useBullet.name) mul = 6;


        for (int i = 0; i < mul; i++)
        {
            GameObject obj = objectManager.CreateObj(_str, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet = obj.GetComponent<Bullet>();

            if (bullet.endBullet != null) //�ڽ� �Ѿ˵� ����
            {
                for (int j = 0; j < 2; j++)
                    objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
            }
        }
    }
    #endregion
}
