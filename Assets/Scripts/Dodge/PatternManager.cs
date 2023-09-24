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

    [SerializeField] GameObject PlatTop;          // �� Platform
    [SerializeField] GameObject PlatBottom;       // �Ʒ� Platform

    [SerializeField] GameObject Plat;
    [SerializeField] GameObject GameEnd;
    [SerializeField] GameObject[] Warnings;
    [SerializeField] GameObject ErrorObject;

    [SerializeField] GameObject Hand1;
    [SerializeField] GameObject Hand1_2;
    [SerializeField] GameObject Hand2;
    [SerializeField] GameObject Hand3;
    [SerializeField] GameObject Hand3_2;

    // �¿� ��ȯ ��ġ(1���)
    [SerializeField] Transform[] SPRE;    // Right
    [SerializeField] Transform[] SPLE;    // Left

    [SerializeField] float RepeatInterv;      // ���� �ݺ� ������ ����
    [SerializeField] float BulletInterv;      // �� ���� �� ź�� ���� ����
    [SerializeField] float PatternInterv;     // ���ϰ� ���� ������ ����

    public bool IsEnd = false;      // �ʻ� ���� ����

    public int CurPattern = 0;      // ���� ����
    int HPForPattern = 3;    // �Ϲ� ���Ͽ� HP

    List<GameObject> PlatL = new List<GameObject>();        // ������ ���� �� ���Ǵ� Platform���� ���� <- EndPattern�� ����ϱ� ����.
    int PatternNum = 0;             // Num of Pattern(�� ��)

    List<List<int[]>> PTLE = new List<List<int[]>>();       // ���ϵ��� List
    GameObject CurRazer = null;
    AudioSource AL;

    // �¿� ��ȯ ��ġ(�����)
    public Transform SPCNT;
    Vector2[][] SP;                     //SP�� ����
    Vector2[] SPT = new Vector2[26];    //W��
    Vector2[] SPB = new Vector2[26];    //��
    Vector2[] SPL = new Vector2[19];    //��
    Vector2[] SPR = new Vector2[19];    //��

    // 4����
    public Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };
    // 8���� + �߾�
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
        for (int i = 0; i < 26; i++)            // SPCNT�� x�� �������� x�� 1�� ������Ű�� �ش� ��ġ�� SPT(��), SPB(��)�� ������. �� �� ��� �� ������ y�� ������ 18
        {
            SPT[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y);

            SPB[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y - 18);

        }
        for (int i = 0; i < 19; i++)            // // SPCNT�� y�� �������� y�� 1�� ���ҽ�Ű�� �ش� ��ġ�� SPL(��), SPR(��)�� ������. �� �� �¿� �� ������ x�� ������ 25
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
    IEnumerator MakeEasyPattern()     // 1����� ���Ǵ� ������ ������.
    {
        /*PlatTop.SetActive(true); PlatTop.SetActive(true);*/
        yield return OneSec;
        List<int[]> CurPT = PTLE[Random.Range(0, PatternNum)];     // ����� ���� �� ���Ƿ� 1���� ������ ������
        for (int CurRepeat = 0; CurRepeat < 2; CurRepeat++)             // ���� 2ȸ �ݺ� ����
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
                    if (CurPT[y][x] == 1)                          // �� �� �����ư��鼭 ������ ����
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
        StartPT(0);               // ���� ������ ����
        yield break;
    }

    // Normal                   N1 -> N2 -> Hard
    IEnumerator PatternN1()       // Play Time : 25s
    {
        Hand2.SetActive(true);
        CurPattern = 1;
        // ȭ�鿡 ������ ����
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return OneSec;
        MakeGlitch(0, 0, 0);

        yield return OneSec;

        WaitForSeconds SecC = new WaitForSeconds(BulletInterv * 1.4f);

        /*
        �� ���� ���� �������� �� 11���� ź���� �����Ǵµ�, �� �� 4ĭ�� ��� Player�� ������ �� �ִ� ������ ����.
        �� �� 4ĭ�� ���� ���� �������� ���Ǵ� ������ K�� �������� K�� ������ ���� 3���� ĭ�� ���� ������ ĭ�� ź���� ����.
        �� �� K�� 7�� 10�� �� K�� ��ȭ���� -1�� ���� ���ϴ� ������ �ٲ� vvvvvv ����� �����ǵ��� ��.
        ���� K�� 7�� 10�� �� �÷����� �����ϴµ�, ���̵� ������ ���� �� ���� �÷����� �ΰ��� �����ǰ� �ϱ� ����, �ѹ� K�� ������ ������.
         */
        int j = Random.Range(1, 3);
        int dk = -1;                        // K�� ��ȭ��
        int k = 7;                          // �� ĭ�� ����� �������� ���
        bool jk = true;                     // ������ �ΰ� �����ϱ� ���� ���
        for (int i = 0; i < 110; i++)       // �� 110���� ź���� ������
        {
            /*
             �� 30���� �����Ǿ��� ��, ������ ������ �ݴ��� ���⿡��
             7���� ź���� �����Ǿ� �÷��̾ ���ڸ����� ��Ƽ�� ���ϰ� �Ͽ�, �÷��̾��� �̵��� ������
            */
            if (i == 30) for (int y = 6; y <= 12; y++) BM.MakeSmallBul(DF[2 / j] * 1.5f, Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 5; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) BM.MakeSmallBul(DF[j] * 6, Vector2.zero).transform.position = SP[j][y];
            }
            // K�� ������ �ٲٸ�, �÷��� ����.
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
            // ������ ��ȭ �� ������ K�� ��ȭ 1ȸ ����
            if (jk) k += dk;
            yield return SecC;
        }
        yield return new WaitForSeconds(10);        // ������ ���� ź���� �����Ǹ� �ش� ź���� ������´� �ɸ��� �ð�
        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(1);
        yield break;
    }

    IEnumerator PatternN2()
    {
        CurPattern = 2;
        // ȭ�鿡 ������ ����
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return new WaitForSeconds(1.5f);
        MakeGlitch(0, 0, 0);
        yield return OneSec;

        // ��� ź�� ���� ��ġ�� �߰� y��. �÷��� ������ ���.
        float MidY = (SPLE[4].position.y + SPLE[5].position.y) / 2;

        // ��, �� �� �� ���� �������� �����ϸ�(�������� �� 6���� ������) �߰� �κп� �̵��ϴ� �÷����� �����Ͽ�, �÷��̾� ���� ��� �̵��ϸ鼭 �ش� �÷����� �̿��ϵ��� ��.
        // �÷����� �����Ǵ� �ݴ��� ��ġ �� ������ ���� ��ġ�� ���Ե��� �ʴ� 4�ٿ��� �������� 1~2���� ź���� �����Ͽ� ���̵� ����

        for (int i = 0; i < 3; i++)
        {
            int a1 = Random.Range(0, 2);
            Warnings[2 + a1].SetActive(true);
            // �÷��� ����
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

            // �÷����� �����ʿ���, �������� ������ �����ǰ� ��.
            if (a1 == 0)
            {
                BM.MakeSmallBul(Vector2.left * 3, Vector2.zero).transform.position = SPRE[Random.Range(0, 5)].position;
                
                for (int y = 0; y < 150; y++)
                {
                    for (int x = 0; x < 5; x++) BM.MakeSmallBul(Vector2.right * 25, Vector2.zero).transform.position = SPLE[5 + x].position;
                    yield return LittleSec;
                }

            }
            // �÷����� ���ʿ���, �������� �Ʒ����� �����ǰ� ��.
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

    IEnumerator CamShake(float time,float intensity = 1)      // ȭ���� ���� ȿ���� ������.
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
    public void EasyEndPattern() // 1������ ���� �� ������ ����ϴ� ������ ������.
    {
        BM.DelBul();
        StopAllCoroutines();
        for (int i = 0; i < SPLE.Length; i++)
        {
            BM.MakeSmallBul(Vector2.left * 2.5f, Vector2.zero).transform.position = SPRE[i].position;
            BM.MakeSmallBul(Vector2.right * 2.5f, Vector2.zero).transform.position = SPLE[i].position;
        }
    }
    public void MakeGlitch(float a, float b, float c)   // ȭ�鿡 ����� ����. �ڼ��� ������ https://github.com/staffantan/unityglitch ����
    {
        GlitchEffect MC = MainCam.GetComponent<GlitchEffect>();
        MC.intensity = a; MC.flipIntensity = b; MC.colorIntensity = c;
    }

    // �ܺ� CS���� ������ ���� ��ų ��, �ܺο��� �ڷ�ƾ�� �����Ű�� �ʰ� �� CS ���ο��� �ڷ�ƾ�� �����Ű�� ���� ����.
    // �׷��� �� ������ �ܺ� CS���� �ڷ�ƾ�� ���� ��ų ��� �ش� �ڷ�ƾ�� �� �Լ� ������ ������ �� ���� ����
    // Input : ���� ��ų ������ ������ ��� �ڷ�ƾ
    IEnumerator ChangePT(IEnumerator a)     
    {
        StartCoroutine(a);
        yield break;
    }
    // �ش� CS �ܺο��� ������ �����ų �� �ֵ��� ���� �Լ�
    // Input : � ������ ������ �����ų ������ ��Ÿ���� int��. 0�� ��� �Ϲ� ������, 1�� ��� 2����� ����
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
    
    // ���� �������� ������ �����ϸ�, �μ��� ���� ������ �λ깰�� ���� ������ �ƴ����� ����
    // ������ �ʴ� ��츦 ���� ������ ���� ����
    // Input : ������ �λ깰���� ���� ������ �ƴ����� �����ϴ� bool��. True�� ��� ������ ������, false�� ��� ��� ����
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
