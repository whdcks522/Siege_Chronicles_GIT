using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Creature;

public class TowerManager : MonoBehaviour
{
    [Header("타워의 최대 체력")]
    public float maxHealth;
    [Header("타워의 현재 체력")]
    public float curHealth;

    public TeamEnum curTeamEnum;

    [Header("타워의 아군 생성 위치")]
    public Transform creatureStartPoint;
    [Header("타워의 캐논 위치")]
    public Transform bulletStartPoint;

    [Header("상대 타워의 위치")]
    public Transform enemyTower;

    [Header("타워 위의 미니 UI")]
    public GameObject miniCanvas;
    public Image miniHealth;


    [Header("매니저")]
    public GameManager gameManager;
    ObjectManager objectManager;
    UIManager UiManager;
    AudioManager audioManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;

        if (curTeamEnum == TeamEnum.Blue)
        {
            miniHealth.color = Color.blue;
            enemyTower = gameManager.redTower;

            ourCreatureFolder = gameManager.objectManager.blueCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            miniHealth.color = Color.red;
            enemyTower = gameManager.blueTower;

            ourCreatureFolder = gameManager.objectManager.redCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.blueCreatureFolder;
        }

        StartCoroutine(UpdateCoroutine());
    }

    [Header("상대팀이 들어있는 폴더(스펠_시체폭발용)")]
    public Transform ourCreatureFolder;
    [Header("상대팀이 들어있는 폴더(스펠_사격용)")]
    public Transform enemyCreatureFolder;

    [Header("타워의 자원 정보")]
    public float curTowerResource = 0f;     //현재 자원
    public float maxTowerResource = 10f;    //최대 자원

    [Header("적 타워가 앞으로 사용할 스펠 데이터")]
    public SpellData futureSpellData;

    WaitForSeconds waitSec = new WaitForSeconds(0.05f);
    public int creatureSpawnIndex = 0;
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (UiManager.selectManager.index_Battle == 1)
            {
                if (maxTowerResource > curTowerResource)
                {
                    //스펠 사용을 위한 자원 증가
                    curTowerResource += gameManager.BankSpeedArr[curBankIndex] * 0.1f;
                }
                else if (maxTowerResource <= curTowerResource)
                {
                    //현재 자원량이 최대치를 넘지 않도록
                    curTowerResource = maxTowerResource;
                }

                if (curTeamEnum == TeamEnum.Red) //빨강 팀 타워에서
                {
                    if (futureSpellData != null)//소환할 것이 정해졌다면
                    {
                        if (curTowerResource >= futureSpellData.spellValue && CreatureCountCheck())//자원이 충분하면서 자신의 크리쳐 소환 여부가 충분할 때
                        {
                            //크리쳐 소환
                            if (gameManager.isEnemySpawn)
                            {
                                SpawnCreature(futureSpellData.spellPrefab.name);

                                //스펠 효과음
                                audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);
                            }
                            else 
                            {
                                curCreatureCount -= 1;
                            }

                            //비용 처리
                            curTowerResource -= futureSpellData.spellValue;

                            //다른 것을 소환하기 위해 초기화
                            futureSpellData = null;

                        }
                    }
                    else if (futureSpellData == null)//소환할 것이 정해지지 않았다면, 어떤 크리쳐를 소환할 지 무작위로 정함
                    {
                        /*
                        int createIndex = UnityEngine.Random.Range(0, gameManager.creatureSpellDataArr.Length - 1);

                        //0, 1, 2중에서 맞았고 2가 나온 경우만 소환

                        if (!recentHit && createIndex == 2) //쉴더는 가까울 때만 소환해야 하므로
                        {
                            createIndex = 0;
                        }

                        futureSpellData = gameManager.creatureSpellDataArr[createIndex];
                        recentHit = false;
                        */

                        if (recentHit)//최근에 맞았으면 
                        {
                            futureSpellData = gameManager.creatureSpellDataArr[2];
                            recentHit = false;
                        }
                        else //안맞았으면, 개구락지 - 노랑이 번갈아 소환
                        {
                            creatureSpawnIndex = (creatureSpawnIndex + 1) % 2;
                            futureSpellData = gameManager.creatureSpellDataArr[creatureSpawnIndex];
                        }
                    }
                }
            }

            yield return waitSec;
        }
    }
    public bool recentHit = false;//public 테스트중

    #region 타워 요소 초기화;

    [Header("뱅크 관련 요소들")]
    public int curBankIndex = 0;//현재 뱅크 레벨이 몇인지

    public void ResetTower()//게임 재시작을 위해 타워 정보 초기화
    {
        //타워 체력 초기화
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        //타워 자원 초기화
        curTowerResource = 0;

        //타워 은행 수치 초기화
        curBankIndex = 0;

        //타워 크리쳐 수 초기화
        curCreatureCount = 0;

        //소환 목록 초기화
        recentHit = false;
        creatureSpawnIndex = 0;
        futureSpellData = null;
    }
    #endregion

    #region 크리쳐 수 제한;

    [Header("자기 팀 크리쳐의 현재 수")]
    public int curCreatureCount;  

    public bool CreatureCountCheck()//소환 인수 증감 후, 소환 가능 여부 반환
    {
        bool canSpawn = false;

        if (curCreatureCount < gameManager.maxCreatureCount)//소환 가능한 경우
        {
            //크리쳐 수 제한 변화
            curCreatureCount++;
            canSpawn = true;
        }
        else if (curCreatureCount >= gameManager.maxCreatureCount)//소환 불가능한 경우
        {
            canSpawn = false;
        }

        if (curTeamEnum == Creature.TeamEnum.Blue)//파랑팀이면 
        {
            CreatureCountText();
        }

        return canSpawn;
    }

    public void CreatureCountText()//크리쳐 세는 텍스트 수정
    {
        //소환 제한 텍스트 갱신
        UiManager.creatureCountText.text = curCreatureCount.ToString() + "/" + gameManager.maxCreatureCount.ToString();

        //소환 제한 텍스트 진동 애니메이션
        UiManager.creatureCountAnim.SetBool("isFlash", true);

        if (curCreatureCount == gameManager.maxCreatureCount)
        {
            UiManager.creatureCountText.color = UiManager.textRed;
        }
        else 
        {
            UiManager.creatureCountText.color = UiManager.textYellow;
        }
    }
    #endregion

    GameObject damageFont = null;//데미지 폰트용 게임 오브젝트
    private void OnTriggerEnter(Collider other)//부딪힘
    {
        if (other.gameObject.CompareTag("Bullet"))//총알과 충돌
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            //피해량 확인
            float damage = bullet.bulletDamage;

            if (bullet.curTeamEnum != curTeamEnum && bullet.byCreature && damage != 0)//팀이 다를 경우면서 크리쳐에 의한 공격만 피해 처리
            {
                if (curTeamEnum == TeamEnum.Blue)
                {
                    damage /= 2;
                }
                else if (curTeamEnum == TeamEnum.Red)//gameLevel이 클수록 안아픔
                {
                    damage /= gameManager.gameLevel;
                }

                if (curTeamEnum == TeamEnum.Blue)//파랑 타워가 맞으면 파랑색
                {
                    damageFont = objectManager.CreateObj("BlueDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                }
                else //빨강 타워가 맞으면 빨강색
                {
                    damageFont = objectManager.CreateObj("RedDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                }
                //폰트 위치와 글자 조정
                damageFont.transform.position = other.transform.position;
                damageFont.GetComponent<DamageFont>().ReName(damage.ToString());

                //피해 관리
                DamageControl(damage);

                //피격한 총알 후처리
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                {
                    bullet.BulletOff();
                }
            }
        }
    }

    #region 데미지 계산
    public void DamageControl(float damage)
    {
        //최근에 맞은 여부 확인(쉴더 소환을 위함)
        recentHit = true;

        //체력 감소
        curHealth -= damage;

        //타워 체력바 관리
        miniHealth.fillAmount = curHealth / maxHealth;

        //Debug.LogWarning("여기는?");

        if (curHealth > 0)//타워가 공격받는 중
        {
            if (damage != 0)
                audioManager.PlaySfx(AudioManager.Sfx.TowerCrashSfx);
        }
        else if (curHealth <= 0 && !UiManager.settingBackground.activeSelf) //타워의 체력이 0이며, 설정 화면이 꺼져 있는 경우
        {
            //Debug.LogWarning("여러번 호출되나?");

            curHealth = 0;

            if (curTeamEnum == Creature.TeamEnum.Red)//빨간 팀이 진 경우
            {
                //설정 화면의 텍스트 수정
                UiManager.victoryTitle.SetActive(true);

                //승리 효과음
                audioManager.PlayBgm(AudioManager.Bgm.WinBgm);

                //불러올때 까지 기다림
                gameManager.fireManager.StartCor();
            }
            else if (curTeamEnum == Creature.TeamEnum.Blue) //파란 팀이 진 경우
            {
                //이미 사격중이라면, 중지
                if (Tower_Gun_Cor != null)
                    StopCoroutine(Tower_Gun_Cor);

                //설정 화면의 텍스트 수정
                UiManager.defeatTitle.SetActive(true);

                //패배 효과음
                audioManager.PlayBgm(AudioManager.Bgm.LoseBgm);
            }
            //게임 종료를 위해 정지 시키기
            UiManager.SettingControl(true);

            //설정 화면의 시작 버튼 비활성화
            UiManager.setBtn.interactable = false;
            UiManager.setBtn.transform.GetChild(0).GetComponent<Text>().color = UiManager.textRed;
        }
    }
    #endregion

    //타워 주술용(스킬용) 대포 각도 조절
    public void RadarControl(Vector3 targetVec) => bulletStartPoint.transform.LookAt(targetVec);

    #region 크리쳐 소환
    public void SpawnCreature(string tmpCreatureName)
    {
        //생명체 생성
        GameObject obj = objectManager.CreateObj(tmpCreatureName, ObjectManager.PoolTypes.CreaturePool);
        Creature creature = obj.GetComponent<Creature>();
        //활동 전에 설정
        creature.BeforeRevive(curTeamEnum, gameManager);
    }
    #endregion

    #region 스펠(무기) 사용 분류
    Vector3 enemyVec;
    public void WeaponSort(string tmpWeaponName) 
    {
        if (tmpWeaponName == gameManager.Gun.name) 
        {
            //이미 사격중이라면, 중지
            if (Tower_Gun_Cor != null)
                StopCoroutine(Tower_Gun_Cor);

            //사격
            Tower_Gun_Cor = StartCoroutine(Tower_Gun());
        } 
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
        else if (tmpWeaponName == gameManager.GrandCure.name) Tower_GrandCure();
        else if (tmpWeaponName == gameManager.CorpseExplosion.name) Tower_CorpseExplosion();
    }
    #endregion


    #region 미니건 난사

    //[Header("사격을 위한 살아있는 적 크리쳐 리스트")]
    List<GameObject> gunList = new List<GameObject>();
    Coroutine Tower_Gun_Cor;
    readonly int maxBulletCount = 6;//미니건의 발사 수

    IEnumerator Tower_Gun() 
    {
        //레이더가 적 타워를 쳐다봄
        RadarControl(enemyTower.transform.position);

        //정해진 총알을 적에게 n빵
        int bulletCount = 0;

        gunList.Clear();
        //적 크리쳐 위치 파악
        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf && //살아있는 경우
                enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature")) //크리쳐 레이어인 경우만(안그럼 잔상에 쏨)
            {
                //리스트에 더하기
                gunList.Add(enemyCreatureFolder.GetChild(i).gameObject);
            }
        }

        if (gunList.Count > 0) 
        {
            //Debug.LogWarning("리스트의 크기: "+ gunList.Count);

            gunList.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
                               .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

            while (bulletCount < maxBulletCount)
            {
                //사격 효과음
                audioManager.PlaySfx(AudioManager.Sfx.GunSfx);

                //타워가 적을 쳐다보도록
                enemyVec = gunList[bulletCount % gunList.Count].transform.position;
                
                //대상을 바라 보도록 설정
                RadarControl(enemyVec);

                GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
                Bullet bullet_bullet = bullet.GetComponent<Bullet>();
                Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

                bullet_bullet.gameManager = gameManager;
                bullet_bullet.Init();


                //총알 대상으로 설정(여기서만 쓰임)
                bullet_bullet.bulletTarget = gunList[bulletCount % gunList.Count].gameObject;
                //총알 위치 설정
                bullet.transform.position = bulletStartPoint.position + bulletStartPoint.transform.forward * 10;
                //총알 가속 설정
                bullet_rigid.velocity = (enemyVec - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;
                //총알 회전 설정
                Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
                bullet.transform.rotation = targetRotation;


                //총알 활성화
                bullet_bullet.BulletOn(curTeamEnum);

                bulletCount++;

                //Debug.LogWarning(bulletCount);

                //총 0.1초 대기
                yield return waitSec;
                yield return waitSec;
            }
        } 
    }
    
    #endregion

    #region 화염구
    void Tower_Flame()//적이 사용할 경우 가속쪽에 clickPoint 코드 변경 필요
    {
        //화염구 효과음
        audioManager.PlaySfx(AudioManager.Sfx.FlameSfx);

        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //이동
        bullet.transform.position = UiManager.cameraCloud.position;
        //가속
        bullet_rigid.velocity = (UiManager.clickSphere.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //활성화
        bullet_bullet.BulletOn(curTeamEnum);
    }
    #endregion

    #region 대회복
    void Tower_GrandCure()
    {
        //대회복 효과음
        audioManager.PlaySfx(AudioManager.Sfx.GrandCureSfx);

        GameObject bullet = objectManager.CreateObj("Tower_GrandCure", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //이동
        bullet.transform.position = UiManager.clickSphere.position;
        //활성화
        bullet_bullet.BulletOn(curTeamEnum);
    }
    #endregion

    #region 시체폭발
    void Tower_CorpseExplosion()
    {
        //시체폭발 적용 효과음
        audioManager.PlaySfx(AudioManager.Sfx.CorpseExplosionAdaptSfx);

        //타워의 레이더가 적 타워를 바라봄
        RadarControl(enemyTower.transform.position);

        GameObject bomb = objectManager.CreateObj("Tower_CorpseExplosion", ObjectManager.PoolTypes.BulletPool);
        Bullet bomb_bullet = bomb.GetComponent<Bullet>();
        //시체 폭발의 이동
        bomb.transform.position = bulletStartPoint.position + bulletStartPoint.transform.forward * 3;
        //시체 폭발의 팀 설정
        bomb_bullet.BulletOn(curTeamEnum);

        foreach (Transform obj in ourCreatureFolder)
        {
            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                //시체 폭발 아이콘 활성화
                Creature creature = obj.gameObject.GetComponent<Creature>();
                creature.CorpseExplosionObj.SetActive(true);
            }
        }      
    }
    #endregion
}
