using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// Make Pattern / End Pattern
public class PatternManager : MonoBehaviour
{
    [SerializeField] BulletManager BM;
    [SerializeField] Camera MainCam;
    [SerializeField] Player Pl;

    [SerializeField] GameObject PlatTop;          // 윗 Platform
    [SerializeField] GameObject PlatBottom;       // 아랫 Platform

    [SerializeField] GameObject Plat;
    [SerializeField] GameObject GameEnd;
    [SerializeField] GameObject[] Warnings;
    [SerializeField] GameObject ErrorObject;

    [SerializeField] GameObject Hand1;
    [SerializeField] GameObject Hand1_2;
    [SerializeField] GameObject Hand2;
    [SerializeField] GameObject Hand3;
    [SerializeField] GameObject Hand3_2;

    // 좌우 소환 위치(1페용)
    [SerializeField] Transform[] SPRE;    // Right
    [SerializeField] Transform[] SPLE;    // Left

    [SerializeField] float RepeatInterv;      // 패턴 반복 사이의 간격
    [SerializeField] float BulletInterv;      // 한 패턴 내 탄막 간의 간격
    [SerializeField] float PatternInterv;     // 패턴과 패턴 사이의 간격

    public bool IsEnd = false;      // 필사 패턴 여부

    public int CurPattern = 0;      // 현재 패턴
    int HPForPattern = 3;    // 일반 패턴용 HP

    List<GameObject> PlatL = new List<GameObject>();        // 레이저 패턴 중 사용되는 Platform들을 저장 <- EndPattern에 사용하기 위함.
    int PatternNum = 0;             // Num of Pattern(안 씀)

    List<List<int[]>> PTLE = new List<List<int[]>>();       // 패턴들의 List
    GameObject CurRazer = null;
    AudioSource AL;

    // 좌우 소환 위치(히든용)
    public Transform SPCNT;
    Vector2[][] SP;                     //SP의 모음
    Vector2[] SPT = new Vector2[26];    //W상
    Vector2[] SPB = new Vector2[26];    //하
    Vector2[] SPL = new Vector2[19];    //좌
    Vector2[] SPR = new Vector2[19];    //우

    // 4방향
    public Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };
    // 8방향 + 중앙
    public Vector2[][] DE =
    {
        new Vector2[] {Vector2.left,Vector2.up}, new Vector2[] {Vector2.zero,Vector2.up}, new Vector2[] { Vector2.right, Vector2.up },
        new Vector2[] {Vector2.left,Vector2.zero}, new Vector2[] {Vector2.zero,Vector2.zero}, new Vector2[] { Vector2.right, Vector2.zero },
        new Vector2[] {Vector2.left,Vector2.down}, new Vector2[] {Vector2.zero,Vector2.down}, new Vector2[] { Vector2.right, Vector2.down }
    };

    // WaitForSeconds;
    WaitForSeconds TwoSec = new WaitForSeconds(2);
    WaitForSeconds OneSec = new WaitForSeconds(1);
    WaitForSeconds LittleSec = new WaitForSeconds(0.05f);
    WaitForSeconds LittleLittle = new WaitForSeconds(0.02f);

    private void Awake()
    {
        AL = GetComponent<AudioSource>();
        for (int i = 0; i < 26; i++)            // SPCNT의 x를 기준으로 x를 1씩 증가시키며 해당 위치를 SPT(상), SPB(하)에 저장함. 이 떄 상과 하 사이의 y축 간격은 18
        {
            SPT[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y);

            SPB[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y - 18);

        }
        for (int i = 0; i < 19; i++)            // // SPCNT의 y를 기준으로 y를 1씩 감소시키며 해당 위치를 SPL(좌), SPR(우)에 저장함. 이 떄 좌와 우 사이의 x축 간격은 25
        {
            SPL[i] = new Vector2(SPCNT.position.x, SPCNT.position.y - i);
            SPR[i] = new Vector2(SPCNT.position.x + 25, SPCNT.position.y - i);
        }
        SP = new Vector2[][] { SPB, SPR, SPL, SPT };
        ReadExternalPattern();
    }

    private void Start()
    {
        StartCoroutine(MakeEasyPattern());
    }

    public void StartInit()
    {
        Pl.gameObject.SetActive(true);
        CurPattern = 0;
        StartPT(0);
    }

    public void NextPattern(int change = 1)
    {
        if(CurPattern == 0)
        {
            if (HPForPattern == 0)
            {
                StartPT(1);
            }
            else
            {
                HPForPattern -= change;
                StartPT(0);
            }
        }
        else if (CurPattern == 1)
        {
            Hand2.SetActive(false);
            StartPT(2);
        }
        else
        {
            print("!");
        }

    }

    Vector3 Left = new Vector3(-10.65f,3.5f,0);
    Vector3 Right = new Vector3(10.65f, 3.5f, 0);
    // EZ
    IEnumerator MakeEasyPattern()     // 1페이즈에 사용되는 패턴을 생성함.
    {
        /*PlatTop.SetActive(true); PlatTop.SetActive(true);*/
        yield return OneSec;
        List<int[]> CurPT = PTLE[Random.Range(0, PatternNum)];     // 저장된 패턴 중 임의로 1개의 패턴을 가져옴
        for (int CurRepeat = 0; CurRepeat < 2; CurRepeat++)             // 패턴 2회 반복 실행
        {
            if(CurRepeat % 2 == 0)
            {
                Warnings[0].SetActive(true);
                Hand3.SetActive(true);
                Hand3.transform.position = Right;
                for (int i = 0; i < 25; i++)
                {
                    yield return LittleLittle;
                    Hand3.transform.Translate(-1, 0, 0);
                }
                Hand3.SetActive(false);
                yield return TwoSec;
            }
            else
            {
                Warnings[1].SetActive(true);
                Hand3_2.SetActive(true);
                Hand3_2.transform.position = Left;
                for (int i = 0; i < 25; i++)
                {
                    yield return LittleLittle;
                    Hand3_2.transform.Translate(1, 0, 0);
                }
                Hand3_2.SetActive(false);
                yield return TwoSec;
            }
            for (int x = 0; x < CurPT[0].Length; x++)
            {
                for (int y = 0; y < CurPT.Count; y++)
                {
                    if (CurPT[y][x] == 1)                          // 좌 우 번갈아가면서 패턴을 생성
                    {
                        if (CurRepeat % 2 == 0)
                        {
                            BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position;
                        }
                        else
                        {
                            BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position;
                        }
                    }
                }
                yield return new WaitForSeconds(BulletInterv);
            }
            yield return new WaitForSeconds(RepeatInterv);
        }
        yield return OneSec;
        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(0);               // 다음 패턴을 실행
        yield break;
    }

    // Normal                   N1 -> N2 -> Hard
    IEnumerator PatternN1()       // Play Time : 25s
    {
        Hand2.SetActive(true);
        CurPattern = 1;
        // 화면에 노이즈 생성
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return OneSec;
        MakeGlitch(0, 0, 0);

        yield return OneSec;

        WaitForSeconds SecC = new WaitForSeconds(BulletInterv * 1.4f);

        /*
        한 세로 줄을 기준으로 총 11개의 탄막이 생성되는데, 이 중 4칸을 비워 Player가 지나갈 수 있는 공간을 만듬.
        이 때 4칸을 비우기 위한 기준으로 사용되는 변수인 K를 기준으로 K를 포함한 위의 3개의 칸은 비우며 나머지 칸은 탄막을 생성.
        이 떄 K가 7과 10일 때 K의 변화량에 -1을 곱해 변하는 방향을 바꿔 vvvvvv 모양이 생성되도록 함.
        또한 K가 7과 10일 때 플랫폼을 생성하는데, 난이도 조절을 위해 한 번에 플랫폼이 두개씩 생성되게 하기 위해, 한번 K의 증감을 무시함.
         */
        int j = Random.Range(1, 3);
        int dk = -1;                        // K의 변화량
        int k = 7;                          // 빈 칸을 만드는 기준으로 사용
        bool jk = true;                     // 발판을 두개 생성하기 위해 사용
        for (int i = 0; i < 110; i++)       // 총 110줄의 탄막이 생성됨
        {
            /*
             총 30줄이 생성되었을 때, 패턴이 생성된 반대의 방향에서
             7줄의 탄막이 생성되어 플레이어가 제자리에서 버티지 못하게 하여, 플레이어의 이동을 강제함
            */
            if (i == 30) for (int y = 6; y <= 12; y++) BM.MakeSmallBul(DF[2 / j] * 1.5f, Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 5; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) BM.MakeSmallBul(DF[j] * 6, Vector2.zero).transform.position = SP[j][y];
            }
            // K의 증감을 바꾸며, 플랫폼 생성.
            if (k == 7 || k == 10)
            {
                GameObject cnt = Instantiate(Plat);
                PlatL.Add(cnt);
                if (k == 7) cnt.transform.position = SP[j][k];
                else cnt.transform.position = SP[j][k + 3];
                cnt.GetComponent<Rigidbody2D>().AddForce(DF[j] * 6, ForceMode2D.Impulse);
                if (jk) dk *= -1;
                jk = !jk;
            }
            // 증감이 변화 된 직후의 K의 변화 1회 무시
            if (jk) k += dk;
            yield return SecC;
        }
        yield return new WaitForSeconds(10);        // 마지막 줄의 탄막이 생성되며 해당 탄막이 사라지는대 걸리는 시간
        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(1);
        yield break;
    }

    IEnumerator PatternN2()
    {
        CurPattern = 2;
        // 화면에 노이즈 생성
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return new WaitForSeconds(1.5f);
        MakeGlitch(0, 0, 0);
        yield return OneSec;

        // 모든 탄막 생성 위치의 중간 y값. 플랫폼 생성에 사용.
        float MidY = (SPLE[4].position.y + SPLE[5].position.y) / 2;

        // 상, 하 중 한 곳에 레이저를 생성하며(레이저는 총 6줄을 차지함) 중간 부분에 이동하는 플랫폼을 생성하여, 플레이어 또한 계속 이동하면서 해당 플랫폼을 이용하도록 함.
        // 플랫폼이 생성되는 반대의 위치 중 레이저 생성 위치에 포함되지 않는 4줄에서 랜덤으로 1~2개의 탄막을 생성하여 난이도 조절

        for (int i = 0; i < 3; i++)
        {
            int a1 = Random.Range(0, 2);
            Warnings[2 + a1].SetActive(true);
            // 플랫폼 생성
            GameObject cnt = Instantiate(Plat);
            cnt.GetComponent<Transform>().localScale = new Vector3(2f,0.1f);
            if (a1 == 0) 
            {
                cnt.transform.position = new Vector3(SPLE[4].position.x + 1, MidY, 0);
                cnt.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2, ForceMode2D.Impulse);
            }
            else
            {
                cnt.transform.position = new Vector3(SPRE[4].position.x - 1, MidY, 0);
                cnt.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 2, ForceMode2D.Impulse);
            }
            PlatL.Add(cnt);
            yield return TwoSec;
            StartCoroutine(CamShake(8.75f));

            // 플랫폼을 오른쪽에서, 레이저를 위에서 생성되게 함.
            if (a1 == 0)
            {
                BM.MakeSmallBul(Vector2.left * 3, Vector2.zero).transform.position = SPRE[Random.Range(0, 5)].position;
                
                for (int y = 0; y < 150; y++)
                {
                    for (int x = 0; x < 5; x++) BM.MakeSmallBul(Vector2.right * 25, Vector2.zero).transform.position = SPLE[5 + x].position;
                    yield return LittleSec;
                }

            }
            // 플랫폼을 왼쪽에서, 레이저를 아래에서 생성되게 함.
            else
            {
                BM.MakeSmallBul(Vector2.right * 3, Vector2.zero).transform.position = SPLE[Random.Range(5, 10)].position;
                
                for (int y = 0; y < 150; y++)
                {
                    for (int x = 0; x < 5; x++) BM.MakeSmallBul(Vector2.left * 25, Vector2.zero).transform.position = SPRE[x].position;
                    yield return LittleSec;
                }
            }
            yield return OneSec;
        }
        yield return new WaitForSeconds(5);

        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(2);
        yield break;
    }

    public void Clear()
    {
        EndPT(false);
        Pl.gameObject.SetActive(false);
        Pl.EndG.SetActive(true);
        Pl.EndG.GetComponent<RealEnd>().Ending(true);
    }

    IEnumerator CamShake(float time,float intensity = 1)      // 화면을 흔드는 효과를 연출함.
    {
        float Cx = MainCam.transform.position.x;
        float Cy = MainCam.transform.position.y;
        int count = (int)(time * 4);
        for (int i = 0; i < count; i++)
        {
            MainCam.transform.position = new Vector3(Cx - 0.5f * intensity, Cy, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx+0.5f * intensity, Cy, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy+0.5f * intensity, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy-0.5f * intensity, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy, -2);
            yield return LittleSec;
        }
    }
    public void EasyEndPattern() // 1페이즈 종료 후 무조건 사망하는 패턴을 생성함.
    {
        BM.DelBul();
        StopAllCoroutines();
        for (int i = 0; i < SPLE.Length; i++)
        {
            BM.MakeSmallBul(Vector2.left * 2.5f, Vector2.zero).transform.position = SPRE[i].position;
            BM.MakeSmallBul(Vector2.right * 2.5f, Vector2.zero).transform.position = SPLE[i].position;
        }
    }
    public void MakeGlitch(float a, float b, float c)   // 화면에 노이즈를 생성. 자세한 설명은 https://github.com/staffantan/unityglitch 참조
    {
        GlitchEffect MC = MainCam.GetComponent<GlitchEffect>();
        MC.intensity = a; MC.flipIntensity = b; MC.colorIntensity = c;
    }

    // 외부 CS에서 패턴을 실행 시킬 때, 외부에서 코루틴을 실행시키지 않고 이 CS 내부에서 코루틴을 실행시키기 위해 만듬.
    // 그렇게 한 이유는 외부 CS에서 코루틴을 실행 시킬 경우 해당 코루틴을 이 함수 내에서 제어할 수 없기 때문
    // Input : 실행 시킬 패턴의 정보가 담긴 코루틴
    IEnumerator ChangePT(IEnumerator a)     
    {
        StartCoroutine(a);
        yield break;
    }
    // 해당 CS 외부에서 패턴을 실행시킬 수 있도록 만든 함수
    // Input : 어떤 종류의 패턴을 실행시킬 것인지 나타내는 int값. 0일 경우 일반 패턴을, 1일 경우 2페이즈를 실행
    public void StartPT(int i)      
    {
        EndPT(false);
        foreach (GameObject s in Warnings) s.SetActive(false);
        Hand1.SetActive(false); Hand2.SetActive(false); Hand1_2.SetActive(false);
        Hand3.SetActive(false); Hand3_2.SetActive(false);
        AL.Play();
        switch (i)
        {
            case 0: StartCoroutine(ChangePT(MakeEasyPattern())); break;
            case 1: StartCoroutine(ChangePT(PatternN1())); break;
            case 2: StartCoroutine(ChangePT(PatternN2())); break;
        }
    }

    public void MusicOff()
    {
        AL.Stop();
    }
    
    // 현재 실행중인 패턴을 중지하며, 인수에 따라 패턴의 부산물을 지울 것인지 아닌지를 정함
    // 지우지 않는 경우를 만든 이유는 연출 때문
    // Input : 패턴의 부산물들을 지울 것인지 아닌지를 결정하는 bool값. True일 경우 지우지 않으며, false일 경우 모두 지움
    public void EndPT(bool _IsEnd)     
    {
        if (!_IsEnd)
        {
            StopAllCoroutines();
            foreach (var a in PlatL) Destroy(a);
            Pl.transform.GetChild(0).gameObject.SetActive(false);
            BM.DelBul();
        }
        else StopAllCoroutines();
    }

    void ReadExternalPattern()  // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
    {
        for (int i = 1; i < 5; i++)
        {
            string tmp = "Text/Dodge/Pattern_" + i.ToString();
            TextAsset textFile = Resources.Load(tmp) as TextAsset;
            if (textFile == null)
            {
                return;
            }
            StringReader stringReader = new StringReader(textFile.text);
            List<int[]> Data = new List<int[]>();

            while (stringReader != null)
            {
                string line = stringReader.ReadLine();
                if (line == null) break;
                int[] cnt = Array.ConvertAll(line.Split(' '), int.Parse);
                Data.Add(cnt);
            }
            PTLE.Add(Data);
            PatternNum += 1;
            stringReader.Close();
        }
    }
}
