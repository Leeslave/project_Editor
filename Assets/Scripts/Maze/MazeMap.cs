using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;


// 더 자세하게 알고싶으면 인터넷에 "엘러의 알고리즘" 을 검색해보시길 권장드립니다.
public class MazeMap
{
    // Map 배열 생성 부분
    public class Cell
    {
        public int col, row;
        public int Group = 0;
        public bool Right;
        public bool Left;
        public bool Up;
        public bool Down;
        public Vector3 Exit = Vector3.zero;
    };
    public class Group
    {
        public int Group_Num;
        public List<Tuple<int, int>> Cells = new List<Tuple<int, int>>();
        public List<int> Last_Rows = new List<int>();   // Group 내 연산에 사용 될 Cell들.
    };

    public void MazeMaking(int C, int R)
    {
        Col = C;
        Row = R;
        Player_X = Random.Range(0, Row);
        Player_Y = Random.Range(0, Col);
        Init_Maze();
        for (; Cur_Col < Col; Cur_Col++)
        {
            Init_Row();
            Union();
            if (Cur_Col != Col - 1) Cell_Down();     // 마지막 Col에선 내릴 필요가 없다.
        }
        TrimMaze();
        if(Random.Range(0,2) == 0)      // 왼쪽 or 오른쪽 뚫음(출구 뚫기)
        {
            int cnt = Player_Y <= Col / 2 ? Random.Range(0, Col / 2) : Random.Range(Col / 2, Col); 
            if(Player_X > Row / 2)
            {
                Maze[0, cnt].Exit = Vector3.left;
            }
            else
            {
                Maze[Row - 1, cnt].Exit = Vector3.right;
            }
        }
        else                            // 아래 or 위 뚫음
        {
            int cnt = Player_X > Row / 2 ? Random.Range(0, Row / 2) : Random.Range(Row / 2, Row);
            if (Player_Y > Col / 2)
            {
                Maze[cnt, Col - 1].Exit = Vector3.down;
            }
            else
            {
                Maze[cnt, 0].Exit = Vector3.up;
            }
        }
    }
    public Cell[,] Maze;   // Maze의 Cell들의 모음
    public int Player_X;
    public int Player_Y;

    //외부 참조 필요 X
    int Col;// Y
    int Row;// X
    int Group_Count = 0; // 현재 사용되고 있는 Group의 갯수
    int Cur_Col = 0;     // 현재 연산에 사용되는 Maze의 Row;
    List<Group> Groups = new List<Group>();  // Group의 List
    void Init_Maze()
    {
        Maze = new Cell[Row, Col];
        for (int y = 0; y < Col; y++) for (int x = 0; x < Row; x++) Maze[x, y] = new Cell();   // Maze 배열 초기화;
        Groups.Add(new Group()); // 편의를 위해 Groups[0]값 초기화. -> Group_Num은 1부터 시작이기 때문.
    }

    void Init_Row()
    {
        for (int i = 0; i < Row; i++)
        {
            // Cell에 아직 Group이 배정되지 않았을 경우 새로 Group을 할당.
            if (Maze[i, Cur_Col].Group == 0)
            {
                Maze[i, Cur_Col].Group = ++Group_Count;
                Maze[i, Cur_Col].row = i;
                Maze[i, Cur_Col].col = Cur_Col;

                Group Nw_Group = new Group();
                Nw_Group.Group_Num = Group_Count;
                Nw_Group.Last_Rows.Add(i);
                Nw_Group.Cells.Add(new Tuple<int, int>(i, Cur_Col));
                Groups.Add(Nw_Group);
            }
        }
    }

    void Union()     // 각 Cell들을 합치는 과정
    {
        for (int i = 0; i < Row - 1; i++)
        {
            int cnt = Random.Range(0, 2);
            int Group_Left = Maze[i, Cur_Col].Group;
            int Group_Right = Maze[i + 1, Cur_Col].Group;
            switch (cnt)
            {
                case 0: // 오른쪽 Cell의 Group을 왼쪽 Cell의 Group에 합침.
                    Maze[i, Cur_Col].Right = true;    // 오른쪽 이동 가능
                    Maze[i + 1, Cur_Col].Left = true;  // 왼쪽 이동 가능
                    if (Group_Left == Group_Right) break; // 같은 그룹일 경우 다시 그룹 안에 넣어 줄 필요는 없다.
                    Maze[i + 1, Cur_Col].Group = Group_Left;
                    Groups[Group_Left].Cells.AddRange(Groups[Group_Right].Cells);
                    Groups[Group_Left].Last_Rows.AddRange(Groups[Group_Right].Last_Rows);
                    foreach (Tuple<int, int> l in Groups[Group_Right].Cells)
                    {
                        Maze[l.Item1, l.Item2].Group = Group_Left;
                    }
                    Groups[Group_Right].Last_Rows.Clear();
                    Groups[Group_Right].Cells.Clear();
                    break;
                case 1: // 아무것도 안함
                    break;
            }
        }
    }
    void Cell_Down()     // Group에서 밑으로 내림.
    {
        for (int i = 1; i < Groups.Count; i++)
        {
            if (Groups[i].Last_Rows.Count == 0) continue; // 내릴 Cell이 없으면(Last_Rows가 비어있으면) 무시함.
            int del_count = Groups[i].Last_Rows.Count; // 나중에 이만큼 삭제 예정.
            int cnt = Random.Range(1, 1 << (Groups[i].Last_Rows.Count - 1));  // 해당 Group의 어떤 Cell에서 내릴 것인지 비트로 정함.
            int To = Groups[i].Last_Rows.Count;
            for (int l = 0; l < To; l++)
            {
                int cur_row = Groups[i].Last_Rows[l];   // 연산을 하고 있는 Row
                if (((1 << l) & cnt) != 0)     // 해당 Row에서 내릴 경우
                {
                    Maze[cur_row, Cur_Col].Down = true;      // 아래 이동 가능
                    Maze[cur_row, Cur_Col + 1].Up = true;    // 위 이동 가능
                    Maze[cur_row, Cur_Col + 1].Group = Groups[i].Group_Num;
                    Groups[i].Last_Rows.Add(cur_row);
                    Groups[i].Cells.Add(new Tuple<int, int>(cur_row, Cur_Col + 1));
                }
            }
            Groups[i].Last_Rows.RemoveRange(0, del_count);   // Group 내에서 이전 Col에 있던 Cell들의 정보 삭제.
        }
    }
    void TrimMaze() // 마지막 모든 Cell을 하나의 Group으로 합침
    {
        for(int Y = 0; Y < Col; Y++)
        {
            for(int X = 0; X < Row-1; X++)
            {
                int Group_Left = Maze[X, Y].Group;
                int Group_Right = Maze[X+1, Y].Group;
                if (Group_Left != Group_Right)
                {
                    Maze[X, Y].Right = true;
                    Maze[X + 1, Y].Left = true;
                    Groups[Group_Left].Cells.AddRange(Groups[Group_Right].Cells);
                    Groups[Group_Left].Last_Rows.AddRange(Groups[Group_Right].Last_Rows);
                    foreach (Tuple<int, int> l in Groups[Group_Right].Cells)
                    {
                        Maze[l.Item1, l.Item2].Group = Group_Left;
                    }
                    Groups[Group_Right].Last_Rows.Clear();
                    Groups[Group_Right].Cells.Clear();
                }
            }
        }
    }
}
