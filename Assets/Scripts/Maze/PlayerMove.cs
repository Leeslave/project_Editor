using System;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public MakeTile MT;
    // X, Y축 이동 속도
    public float Move_X;
    public float Move_Y;
    // Player의 이동 가능 여부
    public bool MoveAble;
    // 입력이 발생한 이후 다음 입력까지의 딜레이 설정
    public float InputDelay;
    // 플레이어의 시야
    public int Sight;
    // MainCamera
    public GameObject maincam;
    // KeyText
    public TMP_Text KeyText;
    // Clear 시 띄울 문구
    public GameObject Clear;
    // Timer
    public GameObject Timer;
    // 우측 상단에 띄울 Icon(나침반)
    public GameObject DirIcon;
    // 출구 위치를 나타내주는 화살표
    public GameObject DirMark;
    // 우측 상단에 띄울 Icon(횟불)
    public GameObject FireIcon;
    // 휙득한 Key들의 Object를 저장하는 List
    // 이 때 추후 계산 상의 편의를 위해 Player Object를 0번에 저장한다.
    List<GameObject> KeyTrain = new List<GameObject>();
    // KeyTrain의 마지막 Object의 이동 전 마지막 위치. Key 휙득 시 해당 Key의 위치 조정을 위해 사용
    Vector3 LastTrans;

    // RayHit용 임시 변수
    RaycastHit2D rayHit;
    // Player의 RigidBody Component를 담을 변수
    Rigidbody2D rigid;
    // 플레이어의 이동 계산에 사용됨.
    // Bf는 이동 전 위치, N은 이동 이후 위치
    float Bf_X;
    float Bf_Y;
    float NX;
    float NY;
    // 마지막 클리어 연출에 사용(문 개방)
    bool IsEnd = false;
    bool MS = true;
    // 플레이어의 이동 방향을 담을 임시 변수
    Vector3 Dir;

    Vector3 VCnt;

    // 현재 아이템을 먹었는지 여부
    bool IsFire = false;

    bool IsCom = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Bf_X = transform.position.x;
        Bf_Y = transform.position.y;
        MoveAble = true;
        Dir = Vector3.up;
        KeyTrain.Add(gameObject);
        LastTrans = transform.position;
        maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    private void Start()
    {
        CalcFog();
        KeyText.text = $"{KeyTrain.Count - 1}/{MT.KeyNum}";
    }

    void FixedUpdate()
    {
        if (MoveAble && MS)
        {
            NX = 0;
            NY = 0;
            // 이동 제어
            if (Input.GetButton("Horizontal"))
            {
                NX = Input.GetAxisRaw("Horizontal") * 10;
                Dir = NX < 0 ? Vector3.left : Vector3.right;
            }
            else if (Input.GetButton("Vertical"))
            {
                NY = Input.GetAxisRaw("Vertical") * 10;
                Dir = NY < 0 ? Vector3.down : Vector3.up;
            }

            if (NX != 0 || NY != 0)   // 움직임을 감지
            {
                bool IsMoveNext = true;     // 다음 이동 시, 조건이 만족되지 않은 출구, 벽과 부딪힌다면 이동하지 않음을 결정. 

                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Plat"));

                if (rayHit.collider != null)
                {
                    if(rayHit.collider.tag == "Wall")
                    {
                        IsMoveNext = false;
                    }
                    else if(rayHit.collider.tag == "ExitWall")
                    {
                        if (!IsEnd)     // 모든 Key를 모았으면 출구를 개방하며, 그렇지 않으면 이동 불가.
                        {
                            if (KeyTrain.Count - 1 == MT.KeyNum) { StartCoroutine(ClearGame()); }
                            IsMoveNext = false;
                        }
                        else           // 클리어
                        {
                            Clear.SetActive(true);
                            PlayerPrefs.SetString("Clear", "Y");
                            SceneManager.LoadScene("TestT");
                            Destroy(gameObject);
                        }
                    }
                }
                // RayCast로 아이템 감지
                rayHit = Physics2D.Raycast(transform.position, Dir, 10, LayerMask.GetMask("Default"));
                
                if (rayHit.collider != null && IsMoveNext)
                {
                    switch (rayHit.collider.name)
                    {
                        case "Key(Clone)":
                            // 플레이어의 뒤를 따라오는 Key의 특성 상, 플레이어와 충돌할 수 있음으로, 해당 연산에 영향을 받지 않는 tag 및 layer로 변경.
                            KeyTrain.Add(rayHit.collider.gameObject);
                            rayHit.collider.tag = "Untagged";
                            rayHit.collider.gameObject.layer = 6;
                            // 현재 맨 뒤의 Object의 직전 이동 위치로 Key의 위치를 정함
                            rayHit.collider.gameObject.transform.position = LastTrans;
                            KeyText.text = $"{KeyTrain.Count - 1}/{MT.KeyNum}";
                            break;
                        // 나침반 아이템을 먹었을 경우, 출구의 위치를 표시하는 화살표 생성
                        case "Compass(Clone)":
                            Destroy(rayHit.collider.gameObject);
                            IsCom = true;
                            DirMark.SetActive(true);
                            DirIcon.SetActive(true);
                            break;
                        // 횟불 아이템을 먹었을 경우, 30초간 무조건 시야 범위 안의 모든 안개를 걷어낸다.
                        case "Fire(Clone)":
                            Destroy(rayHit.collider.gameObject);
                            IsFire = true;
                            FireIcon.SetActive(true);
                            Invoke("FireOff", 30);
                            break;
                    }
                }
                if (IsMoveNext)
                {
                    LastTrans = KeyTrain[KeyTrain.Count-1].transform.position;
                    // KeyTrain에 담긴 모든 Key들의 위치를 자신 바로 앞의 Object의 위치로 변경함.(0은 Player임으로 계산 안함)
                    for(int i = KeyTrain.Count - 1; i > 0; i--) KeyTrain[i].transform.position = KeyTrain[i - 1].transform.position;
                    Bf_X = transform.position.x;
                    Bf_Y = transform.position.y;
                    transform.Translate(new Vector3(NX, NY, 0));
                    maincam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
                    CalcFog();
                    if (IsCom)
                    {
                        VCnt = (MT.Clear.transform.position - transform.position).normalized;
                        DirMark.transform.position = transform.position + (VCnt) * 7;
                        DirMark.transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(VCnt.y,VCnt.x) * Mathf.Rad2Deg);
                    }
                }
                // 플레이어의 수직 혹은 수평 이동키가 입력 되었을 경우 InputDelay 전까진 다음 입력을 받을 수 없게 함
                MoveAble = false;
                Invoke("AbleMove", InputDelay);
            }
        }
    }

    // 게임 클리어 연출.
    // 맨 뒤의 Key부터 0.5초동안 y축으로 5만큼 이동시키고 그 후 해당 위치에서 문으로 이동시킴(이 시간은 Key의 위치에 따라 다름)
    // 모든 Key가 들어갔으면 문이 개방되는 애니메이션이 재생(현재 해당 애니메이션이 없기에 넣지 않음)
    // 이 후 문에 들어가면 게임이 클리어 됨.
    IEnumerator ClearGame()
    {
        Timer.SetActive(false);
        Time.timeScale = 2;
        MS = false;
        for (int i = KeyTrain.Count - 1; i > 0; i--)
        {
            for (int x = 0; x < 50; x++)
            {
                KeyTrain[i].transform.Translate(0,0.1f,0);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.5f);
            Vector3 cnt = (MT.Clear.transform.position - KeyTrain[i].transform.position).normalized;
            while (Vector3.Magnitude(MT.Clear.transform.position - KeyTrain[i].transform.position) > 1)
            {
                KeyTrain[i].transform.Translate(cnt/2);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.25f);
            KeyTrain[i].SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        MT.Clear.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0,0);
        MS = true; IsEnd = true;
        Time.timeScale = 1;
    }

    // 플레이어의 시야 범위를 계산함 ( Instantiate, SetActive가 아닌 투명도 조절로 진행한다.)
    // 플레이어를 중심으로 하는 길이가 Sight인 정사각형을 생각한다.
    // 해당 정사각형을 미로의 좌표에 대입하여, 특정 부분이 미로의 어떤 Col,Row에 해당하는지 연산한 후
    // 플레이어를 중심으로 해당 좌표로 LayCast를 날려, 벽과 부딪히지 않고 해당 좌표까지 도달한다면, 해당 부분의 안개를 걷는다(투명도를 0으로 설정)
    // 한번 시야가 닿았던 부분은, 따로 생성해 두었던 List에 방문 여부를 저장하여 차후 안개를 생성하는 연산을 할 때 조금 흐린 안개로 바꾼다.(투명도를 0.5로 설정)
    // 벽에 부딪힌 경우 해당 부분에 방문한 적이 있었으면 흐린 안개를 생성.
    // 또한 플레이어가 이동한 방향의 반대이며 거리가 Sight + 1인 부분에 대한 안개 생성 관련 연산을 진행한다.

    void CalcFog()
    {
        int CurY = (int)((transform.position.y - 5) / Move_X);
        int CurX = (int)((transform.position.x - 5) / Move_X);

        for (int y = CurY - Sight + 1; y <= CurY + Sight - 1; y++)
        {
            // 해당 좌표가 미로의 범위 밖이면 연산하지 않는다.
            if (y >= MT.Col || y < 0) continue;
            for(int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        // 한 칸에 총 4개의 안개가 존재함으로, 해당 부분의 연산을 위한 것
                        int dx = x * 2 + b;
                        int dy = y * 2 + a;
                        // 레이케스트를 날릴 벡터와 해당 벡터의 값 계산
                        Vector3 RayVec = new Vector3(x * Move_X + 5 - 2.5f + 5 * b, y * Move_Y + 5 - 2.5f + 5 * a, 0) - transform.position;
                        float S = Vector3.Magnitude(RayVec) / Move_X;
                        if (S > Sight) continue;

                        if (IsFire)
                        {
                            MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                            MT.IsFog[dy][dx] = true;
                        }
                        else
                        {
                            rayHit = Physics2D.Raycast(transform.position, RayVec, S * Move_X, LayerMask.GetMask("Plat"));

                            // 충돌시 안개 제거
                            if (rayHit.collider == null)
                            {
                                MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                                MT.IsFog[dy][dx] = true;
                            }
                            // 이동했던 부분 반대편의 경우, 한번 안개를 걷은 적이 있음으로, 흐린 안개로 표시.
                            else
                            {
                                if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                            }
                        }
                    }
            }
        }
        // 추가로 이동 방향 반대 방향에 대해, 거리 + 1의 안개에 대해 이미 밝혀진 길이면 흐린 안개를 생성.
        if (Dir == Vector3.up)
        {
            if (CurY - Sight < 0) return;
            for (int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = x * 2 + b;
                        int dy = (CurY-Sight) * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.down)
        {
            if (CurY + Sight >= MT.Col) return;
            for (int x = CurX - Sight + 1; x <= CurX + Sight - 1; x++)
            {
                if (x >= MT.Row || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = x * 2 + b;
                        int dy = (CurY + Sight) * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.left)
        {
            if (CurX + Sight >= MT.Row) return;
            for (int x = CurY - Sight + 1; x <= CurY + Sight - 1; x++)
            {
                if (x >= MT.Col || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = (CurX + Sight) * 2 + b;
                        int dy = x * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
        else if (Dir == Vector3.right)
        {
            if (CurX - Sight < 0) return;
            for (int x = CurY - Sight + 1; x <= CurY + Sight - 1; x++)
            {
                if (x >= MT.Col || x < 0) continue;
                for (int a = 0; a < 2; a++) for (int b = 0; b < 2; b++)
                    {
                        int dx = (CurX - Sight) * 2 + b;
                        int dy = x * 2 + a;
                        if (MT.IsFog[dy][dx] == true) MT.Fogs[dy][dx].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                    }
            }
        }
    }

    void AbleMove()
    {
        MoveAble = true;
    }

    void FireOff()
    {
        IsFire = false;
        FireIcon.SetActive(false);
    }
}
