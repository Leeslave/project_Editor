using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

// Make Pattern / End Pattern
public class PatternManager : MonoBehaviour
{
    [SerializeField] BulletManager BM;
    [SerializeField] Camera MainCam;
    [SerializeField] Player player;

    [SerializeField] GameObject PlatTop;          // ?? Platform
    [SerializeField] GameObject PlatBottom;       // ??? Platform

    [SerializeField] GameObject Plat;
    [SerializeField] GameObject GameEnd;
    [SerializeField] GameObject[] Warnings;
    [SerializeField] public GameObject ErrorObject;


    // ??? ??? ???(1???)
    [SerializeField] Transform[] SPRE;    // Right
    [SerializeField] Transform[] SPLE;    // Left

    [SerializeField] float RepeatInterv;      // ???? ??? ?????? ????
    [SerializeField] float BulletInterv;      // ?? ???? ?? ?? ???? ????
    [SerializeField] float PatternInterv;     // ????? ???? ?????? ????

    public bool IsEnd = false;      // ??? ???? ????

    public int CurPattern = 0;      // ???? ????

    List<GameObject> PlatL = new List<GameObject>();        // ?????? ???? ?? ????? Platform???? ???? <- EndPattern?? ?????? ????.
    int PatternNum = 0;             // Num of Pattern(?? ??)

    List<List<int[]>> PTLE = new List<List<int[]>>();       // ??????? List
    public Audio_DG AD;

    // ??? ??? ???(?????)
    public Transform SPCNT;
    Vector2[][] SP;                     //SP?? ????
    Vector2[] SPT = new Vector2[26];    //W??
    Vector2[] SPB = new Vector2[26];    //??
    Vector2[] SPL = new Vector2[19];    //??
    Vector2[] SPR = new Vector2[19];    //??

    // 4????
    public Vector2[] DF =
    {
                      Vector2.up,
        Vector2.left,            Vector2.right,
                     Vector2.down
    };
    // 8???? + ???
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

    // 바로 2페이즈로
    [SerializeField] bool IsTest;

    [SerializeField] GameObject TutorialObject;
    int StageInt = 0;

    private void Awake()
    {
        AD = GetComponent<Audio_DG>();
        for (int i = 0; i < 26; i++)            // SPCNT?? x?? ???????? x?? 1?? ???????? ??? ????? SPT(??), SPB(??)?? ??????. ?? ?? ??? ?? ?????? y?? ?????? 18
        {
            SPT[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y);

            SPB[i] = new Vector2(SPCNT.position.x + i, SPCNT.position.y - 18);

        }
        for (int i = 0; i < 19; i++)            // // SPCNT?? y?? ???????? y?? 1?? ??????? ??? ????? SPL(??), SPR(??)?? ??????. ?? ?? ??? ?? ?????? x?? ?????? 25
        {
            SPL[i] = new Vector2(SPCNT.position.x, SPCNT.position.y - i);
            SPR[i] = new Vector2(SPCNT.position.x + 25, SPCNT.position.y - i);
        }
        SP = new Vector2[][] { SPB, SPR, SPL, SPT };
        ReadExternalPattern();

        try
        {
            StageInt = GameSystem.Instance.GetTask("Maze");
        }
        catch { }

        

        if (StageInt == 0) { TutorialObject.SetActive(true); player.InitHP = 2; }

        player.Init();
    }

    private void Start()
    {
        // Test용으로 바로 3페이즈로 넘어가는 기능
        if (!IsTest) StartCoroutine(MakeEasyPattern());
        else { StartPT(2); ErrorObject.SetActive(true); }
    }

    public void StartInit()
    {
        player.gameObject.SetActive(true);
        CurPattern = 0;
        StartPT(0);
        
    }

    public void NextPattern(ref int HPForPattern, int change = 1)
    {
        EndPT(false);
        CamShake(0.5f);
        if(CurPattern == 0)
        {
            HPForPattern -= change;
            
            if (HPForPattern == 0)
            {
                if (StageInt >= 2) StartPT(1);
                else player.GameClear();
            }
            else
            {
                StartPT(0);
            }
        }
        else if (CurPattern == 1)
        {
            if (change == 1 && StageInt == 3)
            {
                change = 0;
                StartPT(2);
            }
            else StartPT(1);
        }
        else if (StageInt == 3)
        {
            StartPT(2);
        }

    }

    Vector3 Left = new Vector3(-10.65f, 3.5f, 0);
    Vector3 Right = new Vector3(10.65f, 3.5f, 0);
    // EZ
    IEnumerator MakeEasyPattern(bool IsTu = false)     // 1?????? ????? ?????? ??????.
    {
        AD.MusicOn();
        int NextPtNum = Random.Range(0, PatternNum + 1);
        List<int[]> CurPT = new List<int[]>();
        if (NextPtNum < PatternNum) CurPT = PTLE[Random.Range(0, PatternNum)];

        else
        {
            for (int i = 0; i < 10; i++) CurPT.Add(new int[25]);
            for (int i = 0; i < 25; i++)
            {
                List<int> L = new List<int>();
                for (int x = 0; x < 1; x++)
                {
                    int cnt = Random.Range(1, 9);
                    while (L.Contains(cnt)) cnt = Random.Range(1, 9);
                    L.Add(cnt);
                }
                for (int x = 0; x < 10; x++) CurPT[x][i] = i % 5 == 0 ? (L.Contains(x) ? 1 : 0) : 0;
            }
        }

        if (!ErrorObject.activeSelf) StartCoroutine(AddCMD((2 + BulletInterv * CurPT[0].Length + RepeatInterv) * 2));

        yield return OneSec;


        for (int CurRepeat = 0; CurRepeat < 2; CurRepeat++)             // ???? 2? ??? ????
        {
            if (CurRepeat % 2 == 0)
            {
                Warnings[0].SetActive(true);
                yield return TwoSec;
            }
            else
            {
                Warnings[1].SetActive(true);
                yield return TwoSec;
            }
            int cnt = Random.Range(0, 4);
            int XL = CurPT[0].Length - 1, YL = CurPT.Count - 1;
            switch (cnt)
            {
                case 0:
                    for (int x = 0; x < CurPT[0].Length; x++)
                    {
                        for (int y = 0; y < CurPT.Count; y++) if (CurPT[y][x] == 1) { if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position; else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position; }
                        yield return new WaitForSeconds(BulletInterv);
                    }
                    break;
                case 1:
                    for (int x = 0; x < CurPT[0].Length; x++)
                    {
                        for (int y = 0; y < CurPT.Count; y++) if (CurPT[y][XL - x] == 1) { if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position; else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position; }
                        yield return new WaitForSeconds(BulletInterv);
                    }
                    break;
                case 2:
                    for (int x = 0; x < CurPT[0].Length; x++)
                    {
                        for (int y = 0; y < CurPT.Count; y++) if (CurPT[YL - y][XL - x] == 1) { if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position; else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position; }
                        yield return new WaitForSeconds(BulletInterv);
                    }
                    break;
                case 3:
                    for (int x = 0; x < CurPT[0].Length; x++)
                    {
                        for (int y = 0; y < CurPT.Count; y++) if (CurPT[YL - y][x] == 1) { if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position; else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position; }
                        yield return new WaitForSeconds(BulletInterv);
                    }
                    break;
            }

            yield return new WaitForSeconds(RepeatInterv);
        }
        yield return OneSec;
        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(0);               // ???? ?????? ????
        yield break;
    }

    public TMP_Text[] CMDs;
    [HideInInspector] public int CurProcess = 0;

    string[] Suffix = { "1st", "2nd", "3rd", "4th", "5th" };
    IEnumerator AddCMD(float time)
    {
        string cnt;
        WaitForSeconds WFS = new WaitForSeconds(time * 0.1f);
        for (int i = 0; i <= 10; i++)
        {
            cnt = $"<i>{Suffix[CurProcess]}</i> Process executed... [";
            for (int x = 0; x < i; x++) cnt += '■'; for (int x = i; x < 10; x++) cnt += "  "; cnt += ']';
            CMDs[CurProcess].text = cnt;
            yield return WFS;
        }

        CMDs[CurProcess].text = $"<i>{Suffix[CurProcess]}</i> Access Key Acquired!"; CurProcess++;
    }

    public IEnumerator EndCMD()
    {
        CMDs[CurProcess].text = "Access Accept!";
        yield return TwoSec;
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.ClearTask("Dodge");
            GameSystem.LoadScene("Screen");
        }
        else
        {
            SceneManager.LoadScene("Screen");
        }
    }

    // Normal                   N1 -> N2 -> Hard
    IEnumerator PatternN1()       // Play Time : 25s
    {
        MainCam.transform.position = new Vector3(0, 0, -2);
        AD.NoiszeOn();
        CurPattern = 1;
        // ??? ?????? ????
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return OneSec;
        MakeGlitch(0, 0, 0);

        yield return OneSec;

        //AD.MusicOn(1);
        WaitForSeconds SecC = new WaitForSeconds(BulletInterv * 1.4f);

        /*
        ?? ???? ???? ???????? ?? 11???? ???? ????????, ?? ?? 4??? ??? Player?? ?????? ?? ??? ?????? ????.
        ?? ?? 4??? ???? ???? ???????? ????? ?????? K?? ???????? K?? ?????? ???? 3???? ??? ???? ?????? ??? ???? ????.
        ?? ?? K?? 7?? 10?? ?? K?? ??????? -1?? ???? ????? ?????? ??? vvvvvv ????? ????????? ??.
        ???? K?? 7?? 10?? ?? ?????? ????????, ????? ?????? ???? ?? ???? ?????? ????? ??????? ??? ????, ??? K?? ?????? ??????.
         */
        int j = Random.Range(1, 3);
        int dk = -1;                        // K?? ?????
        int k = 7;                          // ?? ??? ????? ???????? ???
        bool jk = true;                     // ?????? ??? ??????? ???? ???
        for (int i = 0; i < 190; i++)       // ?? 110???? ???? ??????
        {
            /*
             ?? 30???? ????????? ??, ?????? ?????? ????? ??????
             7???? ???? ??????? ????? ????????? ????? ????? ???, ??????? ????? ??????
            */
            if (i == 30) for (int y = 7; y <= 11; y++) BM.MakeSmallBul(DF[2 / j], Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 5; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) BM.MakeSmallBul(DF[j] * 12, Vector2.zero).transform.position = SP[j][y];
            }
            // K?? ?????? ????, ???? ????.
            if (k == 7 || k == 10)
            {
                GameObject cnt = Instantiate(Plat);
                PlatL.Add(cnt);
                if (k == 7) cnt.transform.position = SP[j][k];
                else cnt.transform.position = SP[j][k + 3];
                cnt.GetComponent<Rigidbody2D>().AddForce(DF[j] * 12, ForceMode2D.Impulse);
                if (jk) dk *= -1;
                jk = !jk;
            }
            // ?????? ??? ?? ?????? K?? ??? 1? ????
            if (jk) k += dk;
            yield return SecC;
        }
        yield return new WaitForSeconds(9);        // ?????? ???? ???? ??????? ??? ???? ??????? ????? ???
        if (!ErrorObject.activeSelf) ErrorObject.SetActive(true);
        StartPT(1);
        yield break;
    }

    IEnumerator PatternN2()
    {
        MainCam.transform.position = new Vector3(0, 0, -2);
        AD.NoiszeOn();
        CurPattern = 2;
        // ??? ?????? ????
        MakeGlitch(0.1f, 0.5f, 0.7f);
        yield return new WaitForSeconds(1.5f);
        MakeGlitch(0, 0, 0);
        yield return OneSec;

        AD.MusicOn(1);
        // ??? ?? ???? ????? ??? y??. ???? ?????? ???.
        float MidY = (SPLE[4].position.y + SPLE[5].position.y) / 2;

        // ??, ?? ?? ?? ???? ???????? ???????(???????? ?? 6???? ??????) ??? ???? ?????? ?????? ???????, ????? ???? ??? ?????? ??? ?????? ???????? ??.
        // ?????? ??????? ????? ??? ?? ?????? ???? ????? ??????? ??? 4????? ???????? 1~2???? ???? ??????? ????? ????

        for (int i = 0; i < 3; i++)
        {
            int a1 = Random.Range(0, 2);
            Warnings[2 + a1].SetActive(true);
            // ???? ????
            GameObject cnt = Instantiate(Plat);
            cnt.GetComponent<Transform>().localScale = new Vector3(2f, 0.1f);
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

            // ?????? ?????????, ???????? ?????? ??????? ??.
            if (a1 == 0)
            {
                BM.MakeSmallBul(Vector2.left * 3, Vector2.zero).transform.position = SPRE[Random.Range(0, 5)].position;

                for (int y = 0; y < 150; y++)
                {
                    for (int x = 0; x < 5; x++) BM.MakeSmallBul(Vector2.right * 25, Vector2.zero).transform.position = SPLE[5 + x].position;
                    yield return LittleSec;
                }

            }
            // ?????? ???????, ???????? ??????? ??????? ??.
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
        player.gameObject.SetActive(false);
        player.EndG.SetActive(true);
        player.EndG.GetComponent<RealEnd>().Ending(true);
        GameSystem.Instance.ClearTask("Dodge");
        GameSystem.LoadScene("Screen");
    }

    IEnumerator CamShake(float time, float intensity = 1)      // ????? ???? ????? ??????.
    {
        float Cx = MainCam.transform.position.x;
        float Cy = MainCam.transform.position.y;
        int count = (int)(time * 4);
        for (int i = 0; i < count; i++)
        {
            MainCam.transform.position = new Vector3(Cx - 0.5f * intensity, Cy, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx + 0.5f * intensity, Cy, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy + 0.5f * intensity, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy - 0.5f * intensity, -2);
            yield return LittleSec;
            MainCam.transform.position = new Vector3(Cx, Cy, -2);
            yield return LittleSec;
        }
    }
    public void EasyEndPattern() // 1?????? ???? ?? ?????? ?????? ?????? ??????.
    {
        BM.DelBul();
        StopAllCoroutines();
        for (int i = 0; i < SPLE.Length; i++)
        {
            BM.MakeSmallBul(Vector2.left * 2.5f, Vector2.zero).transform.position = SPRE[i].position;
            BM.MakeSmallBul(Vector2.right * 2.5f, Vector2.zero).transform.position = SPLE[i].position;
        }
    }
    public void MakeGlitch(float a, float b, float c)   // ??? ?????? ????. ????? ?????? https://github.com/staffantan/unityglitch ????
    {
        GlitchEffect MC = MainCam.GetComponent<GlitchEffect>();
        MC.intensity = a; MC.flipIntensity = b; MC.colorIntensity = c;
    }

    // ??? CS???? ?????? ???? ??? ??, ?????? ?????? ??????? ??? ?? CS ??????? ?????? ??????? ???? ????.
    // ????? ?? ?????? ??? CS???? ?????? ???? ??? ??? ??? ?????? ?? ??? ?????? ?????? ?? ???? ????
    // Input : ???? ??? ?????? ?????? ??? ????
    IEnumerator ChangePT(IEnumerator a)
    {
        StartCoroutine(a);
        yield break;
    }
    // ??? CS ?????? ?????? ?????? ?? ????? ???? ???
    // Input : ?? ?????? ?????? ?????? ?????? ??????? int??. 0?? ??? ??? ??????, 1?? ??? 2?????? ????
    public void StartPT(int i)
    {
        foreach (GameObject s in Warnings) s.SetActive(false);
        switch (i)
        {
            case 0: StartCoroutine(ChangePT(MakeEasyPattern())); break;
            case 1: StartCoroutine(ChangePT(PatternN1())); break;
            case 2: StartCoroutine(ChangePT(PatternN2())); break;
        }
    }

    // ???? ???????? ?????? ???????, ????? ???? ?????? ????? ???? ?????? ??????? ????
    // ?????? ??? ??? ???? ?????? ???? ????
    // Input : ?????? ??????? ???? ?????? ??????? ??????? bool??. True?? ??? ?????? ??????, false?? ??? ??? ????
    public void EndPT(bool _IsEnd)
    {
        if (!_IsEnd)
        {
            StopAllCoroutines();
            foreach (var a in PlatL) Destroy(a);
            player.transform.GetChild(0).gameObject.SetActive(false);
            BM.DelBul();
        }
        else StopAllCoroutines();
    }

    void ReadExternalPattern()  // Read Pattern In Resources Folder. Name of The Pattern File Must be Pattern_X ( ex) Pattern_1, Pattern_2)
    {
        for (int i = 1; i <= 5; i++)
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

    public void ExternalStopCor()
    {
        StopAllCoroutines();
    }
}

