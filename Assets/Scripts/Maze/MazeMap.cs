using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;


// �� �ڼ��ϰ� �˰������ ���ͳݿ� "������ �˰���" �� �˻��غ��ñ� ����帳�ϴ�.
public class MazeMap
{
    // Map �迭 ���� �κ�
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
        public List<int> Last_Rows = new List<int>();   // Group �� ���꿡 ��� �� Cell��.
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
            if (Cur_Col != Col - 1) Cell_Down();     // ������ Col���� ���� �ʿ䰡 ����.
        }
        TrimMaze();
        if(Random.Range(0,2) == 0)      // ���� or ������ ����(�ⱸ �ձ�)
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
        else                            // �Ʒ� or �� ����
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
    public Cell[,] Maze;   // Maze�� Cell���� ����
    public int Player_X;
    public int Player_Y;

    //�ܺ� ���� �ʿ� X
    int Col;// Y
    int Row;// X
    int Group_Count = 0; // ���� ���ǰ� �ִ� Group�� ����
    int Cur_Col = 0;     // ���� ���꿡 ���Ǵ� Maze�� Row;
    List<Group> Groups = new List<Group>();  // Group�� List
    void Init_Maze()
    {
        Maze = new Cell[Row, Col];
        for (int y = 0; y < Col; y++) for (int x = 0; x < Row; x++) Maze[x, y] = new Cell();   // Maze �迭 �ʱ�ȭ;
        Groups.Add(new Group()); // ���Ǹ� ���� Groups[0]�� �ʱ�ȭ. -> Group_Num�� 1���� �����̱� ����.
    }

    void Init_Row()
    {
        for (int i = 0; i < Row; i++)
        {
            // Cell�� ���� Group�� �������� �ʾ��� ��� ���� Group�� �Ҵ�.
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

    void Union()     // �� Cell���� ��ġ�� ����
    {
        for (int i = 0; i < Row - 1; i++)
        {
            int cnt = Random.Range(0, 2);
            int Group_Left = Maze[i, Cur_Col].Group;
            int Group_Right = Maze[i + 1, Cur_Col].Group;
            switch (cnt)
            {
                case 0: // ������ Cell�� Group�� ���� Cell�� Group�� ��ħ.
                    Maze[i, Cur_Col].Right = true;    // ������ �̵� ����
                    Maze[i + 1, Cur_Col].Left = true;  // ���� �̵� ����
                    if (Group_Left == Group_Right) break; // ���� �׷��� ��� �ٽ� �׷� �ȿ� �־� �� �ʿ�� ����.
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
                case 1: // �ƹ��͵� ����
                    break;
            }
        }
    }
    void Cell_Down()     // Group���� ������ ����.
    {
        for (int i = 1; i < Groups.Count; i++)
        {
            if (Groups[i].Last_Rows.Count == 0) continue; // ���� Cell�� ������(Last_Rows�� ���������) ������.
            int del_count = Groups[i].Last_Rows.Count; // ���߿� �̸�ŭ ���� ����.
            int cnt = Random.Range(1, 1 << (Groups[i].Last_Rows.Count - 1));  // �ش� Group�� � Cell���� ���� ������ ��Ʈ�� ����.
            int To = Groups[i].Last_Rows.Count;
            for (int l = 0; l < To; l++)
            {
                int cur_row = Groups[i].Last_Rows[l];   // ������ �ϰ� �ִ� Row
                if (((1 << l) & cnt) != 0)     // �ش� Row���� ���� ���
                {
                    Maze[cur_row, Cur_Col].Down = true;      // �Ʒ� �̵� ����
                    Maze[cur_row, Cur_Col + 1].Up = true;    // �� �̵� ����
                    Maze[cur_row, Cur_Col + 1].Group = Groups[i].Group_Num;
                    Groups[i].Last_Rows.Add(cur_row);
                    Groups[i].Cells.Add(new Tuple<int, int>(cur_row, Cur_Col + 1));
                }
            }
            Groups[i].Last_Rows.RemoveRange(0, del_count);   // Group ������ ���� Col�� �ִ� Cell���� ���� ����.
        }
    }
    void TrimMaze() // ������ ��� Cell�� �ϳ��� Group���� ��ħ
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
