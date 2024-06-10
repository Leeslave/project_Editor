using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// Make Pattern / End Pattern
public class PatternManager : MonoBehaviour
{
    [SerializeField] BulletManager BM;
    [SerializeField] Camera MainCam;
    [SerializeField] Player Pl;

    [SerializeField] GameObject PlatTop;          // ?? Platform
    [SerializeField] GameObject PlatBottom;       // ??? Platform

    [SerializeField] GameObject Plat;
    [SerializeField] GameObject GameEnd;
    [SerializeField] GameObject[] Warnings;
    [SerializeField] public GameObject ErrorObject;

    [SerializeField] GameObject Hand1;
    [SerializeField] GameObject Hand1_2;
    [SerializeField] GameObject Hand2;
    [SerializeField] GameObject Hand3;
    [SerializeField] GameObject Hand3_2;

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

    [SerializeField] bool IsTest;
    [SerializeField] bool NormalPattern = false;

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
    }

    private void Start()
    {
        if (!IsTest) StartCoroutine(MakeEasyPattern());
        else { StartPT(2); ErrorObject.SetActive(true); }
    }

    public void StartInit()
    {
        Pl.gameObject.SetActive(true);
        CurPattern = 0;
        StartPT(0);
    }

    public void NextPattern(ref int HPForPattern, int change = 1)
    {
        CamShake(0.5f);
        if(CurPattern == 0)
        {
            HPForPattern -= change;
            if (HPForPattern == 0)
            {
                if(NormalPattern) StartPT(1);
                else
                {
                    GameSystem.Instance.ClearTask("Dodge");
                    //LoadTestTrash.Instance.LoadScene = "Screen";
                    //SceneManager.LoadScene("LoadT");
                    GameSystem.LoadScene("Screen");
                }
            }
            else
            {
                StartPT(0);
            }
        }
        else if (CurPattern == 1)
        {
            if (change == 1)
            {
                change = 0;
                Hand2.SetActive(false);
                StartPT(2);
            }
            else StartPT(1);
        }
        else
        {
            StartPT(2);
        }

    }

    Vector3 Left = new Vector3(-10.65f,3.5f,0);
    Vector3 Right = new Vector3(10.65f, 3.5f, 0);
    // EZ
    IEnumerator MakeEasyPattern(bool IsTu = false)     // 1?????? ????? ?????? ??????.
    {
        /*PlatTop.SetActive(true); PlatTop.SetActive(true);*/
        AD.MusicOn();
        yield return OneSec;
        int NextPtNum = Random.Range(0, PatternNum + 1);
        print(NextPtNum);
        List<int[]> CurPT = new List<int[]>();
        if (NextPtNum < PatternNum) CurPT = PTLE[Random.Range(0, PatternNum)];     // ????? ???? ?? ????? 1???? ?????? ??????
        else
        {
            for(int i = 0; i < 10; i++) CurPT.Add(new int[25]);
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
        for (int CurRepeat = 0; CurRepeat < 2; CurRepeat++)             // ???? 2? ??? ????
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
            int cnt = Random.Range(0, 4);
            int XL = CurPT[0].Length - 1, YL = CurPT.Count - 1; 
            switch (cnt)
            {
                case 0:
                    for (int x = 0; x < CurPT[0].Length; x++)
                    {
                        for (int y = 0; y < CurPT.Count; y++) if (CurPT[y][x] == 1) { if (CurRepeat % 2 == 0) BM.MakeSmallBul(Vector2.left * 10, Vector2.zero).transform.position = SPRE[y].position; else BM.MakeSmallBul(Vector2.right * 10, Vector2.zero).transform.position = SPLE[y].position; }
                        yield return new WaitForSeconds(BulletInterv);
                    }break;
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

    // Normal                   N1 -> N2 -> Hard
    IEnumerator PatternN1()       // Play Time : 25s
    {
        MainCam.transform.position = new Vector3(0, 0, -2);
        AD.NoiszeOn();
        Hand2.SetActive(true);
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
        for (int i = 0; i < 140; i++)       // ?? 110???? ???? ??????
        {
            /*
             ?? 30???? ????????? ??, ?????? ?????? ????? ??????
             7???? ???? ??????? ????? ????????? ????? ????? ???, ??????? ????? ??????
            */
            if (i == 30) for (int y = 6; y <= 12; y++) BM.MakeSmallBul(DF[2 / j] * 1.5f, Vector2.zero).transform.position = SP[2 / j][y];
            for (int y = 5; y <= 14; y++)
            {
                if (!(y >= k && y <= k + 3)) BM.MakeSmallBul(DF[j] * 6, Vector2.zero).transform.position = SP[j][y];
            }
            // K?? ?????? ????, ???? ????.
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
            // ?????? ??? ?? ?????? K?? ??? 1? ????
            if (jk) k += dk;
            yield return SecC;
        }
        yield return new WaitForSeconds(10);        // ?????? ???? ???? ??????? ??? ???? ??????? ????? ???
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
        Pl.gameObject.SetActive(false);
        Pl.EndG.SetActive(true);
        Pl.EndG.GetComponent<RealEnd>().Ending(true);
        GameSystem.Instance.ClearTask("Dodge");
        GameSystem.LoadScene("Screen");
    }

    IEnumerator CamShake(float time,float intensity = 1)      // ????? ???? ????? ??????.
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
        EndPT(false);
        foreach (GameObject s in Warnings) s.SetActive(false);
        Hand1.SetActive(false); Hand2.SetActive(false); Hand1_2.SetActive(false);
        Hand3.SetActive(false); Hand3_2.SetActive(false);
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
            Pl.transform.GetChild(0).gameObject.SetActive(false);
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
}
