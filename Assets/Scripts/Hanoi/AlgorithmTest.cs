using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class HanoiTest
{
    int[,,] DP;
    int[] Durability = { -1, -1, 10, -1,-1 };

    int start;

    int GoalMin = 0;

    int Cal1;
    int Cal2;
    bool CalJ;

    List<int> cnt = new List<int>(3);

    public HanoiTest()
    {
        start = 1 << 5;
        DP = new int[start, start, start];
        Back(start-1, 0, 0,0);
    }

    void Back(int Con1, int Con2, int Con3, int Sub)
    {
        int Cur = DP[Con1, Con2, Con3];

        // �ѹ��̶� ���� �� ���� ������, ���� ���� ���� ������ ũ�� �湮���� ����
        if (GoalMin != 0 && Cur >= GoalMin) return;
        // �ߺ� �湮 ������(BackTracking)
        if (Cur != 0 && Sub >= Cur) return;
        // �ϳ��� �ı������� ����.
        if (Durability[2] == 0) return;
        // ���� ���� ���� ���� GoalMin�� ����. ������ �ּҰ��� ���� ������ ������ �ּҰ��� �����.
        if (Con1 == 0 && Con2 == 0 && Con3 == start - 1) { GoalMin = Sub; return; }
        DP[Con1, Con2, Con3] = Sub;

        cnt.Clear();

        GetTop(Con1);
        int Top1 = Cal1;
        if (Top1 != -1) cnt.Add(Top1);
        GetTop(Con2);
        int Top2 = Cal1;
        if (Top2 != -1) cnt.Add(Top2);
;       GetTop(Con3);
        int Top3 = Cal1;
        if (Top3 != -1) cnt.Add(Top3);

        // �׻� ���� ���ſ��, ������ �� �ִ� ���� �켱���� ������.
        while (true)
        {
            CalJ = false;
            Cal1 = cnt.Min();
            Cal2 = 1 << Cal1;
            if (Top1 == Cal1)
            {
                if (MoveAble(Top1, Top2))
                {
                    CalJ = true;
                    Durability[Top1]--;
                    Back(Con1 - Cal2, Con2 + Cal2, Con3, Sub + 1);
                    Durability[Top1]++;
                }
                if (MoveAble(Top1, Top3))
                {
                    CalJ = true;
                    Durability[Top1]--;
                    Back(Con1 - Cal2, Con2, Con3 + Cal2, Sub + 1);
                    Durability[Top1]++;
                }
            }
            else if (Top2 == Cal1)
            {
                if (MoveAble(Top2, Top1))
                {
                    CalJ = true;
                    Durability[Top2]--;
                    Back(Con1 + Cal2, Con2 - Cal2, Con3, Sub + 1);
                    Durability[Top2]++;
                }
                if (MoveAble(Top2, Top3))
                {
                    CalJ = true;
                    Durability[Top2]--;
                    Back(Con1, Con2 - Cal2, Con3 + Cal2, Sub + 1);
                    Durability[Top2]++;
                }
            }
            else
            {
                if (MoveAble(Top3, Top1))
                {
                    CalJ = true;
                    Durability[Top3]--;
                    Back(Con1 + Cal2, Con2, Con3 - Cal2, Sub + 1);
                    Durability[Top3]++;
                }
                if (MoveAble(Top3, Top2))
                {
                    CalJ = true;
                    Durability[Top3]--;
                    Back(Con1, Con2 + Cal2, Con3 - Cal2, Sub + 1);
                    Durability[Top3]++;
                }
            }
            if (CalJ) break;
            cnt.Remove(Cal1);
        }
       /* // 1 -> 2
        if (Con1 > Con2)
        {
            Durability[Top1In]--;
            Back(Con1 - Top1, Con2 + Top1, Con3, Sub + 1);
            Durability[Top1In]++;
        }
        // 2 -> 1
        else if (Con2 > Con1)
        {
            Durability[Top2In]--;
            Back(Con1 + Top2, Con2 - Top2, Con3, Sub + 1);
            Durability[Top2In]++;
        }
        // 1 -> 3
        if (Con1 > Con3)
        {
            Durability[Top1In]--;
            Back(Con1 - Top1, Con2, Con3 + Top1, Sub + 1);
            Durability[Top1In]++;
        }
        // 3 -> 1
        else if (Con3 > Con1)
        {
            Durability[Top3In]--;
            Back(Con1 + Top3, Con2, Con3 - Top3, Sub + 1);
            Durability[Top3In]++;
        }
        // 2 -> 3
        if (Con2 > Con3)
        {
            Durability[Top2In]--;
            Back(Con1, Con2 - Top2, Con3 + Top2, Sub + 1);
            Durability[Top2In]++;
        }
        // 3 -> 2
        else if (Con3 > Con2)
        {
            Durability[Top3In]--;
            Back(Con1, Con2 + Top3, Con3 - Top3, Sub + 1);
            Durability[Top3In]++;
        }*/

        // �ش� ���κ��� ����� ��� ������ �湮 �� �� �� ����.
        DP[Con1, Con2, Con3] = Cur;
    }

    // a -> b
    bool MoveAble(int a, int b)
    {
        return a > b || b == -1;
    }


    void GetTop(int a)
    {
        if (a == 0) { Cal1 = -1; return; }
        for (Cal1 = 4; Cal1 > 0; Cal1--) if (((1 << Cal1) & a) != 0) break;
    }
}