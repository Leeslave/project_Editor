using System;

namespace EditorGame.WorkIntegration
{
    /// <summary>고정된 짧은 인증 맵. 좌표는 화면처럼 왼쪽 위가 원점이다.</summary>
    public sealed class ShortMaze
    {
        private static readonly string[][] Layouts =
        {
            new[] { "###########", "#S..#....E#", "###.#.###.#", "#...#...#.#", "#.#####.#.#", "#...#K..#.#", "###.#.###.#", "#.........#", "###########" },
            new[] { "###########", "#S....#..E#", "#.###.#.#.#", "#...#...#.#", "###.#####.#", "#...#K....#", "#.###.###.#", "#.........#", "###########" }
        };
        private readonly string[] cells;
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool HasToken { get; private set; }
        public bool Succeeded { get; private set; }
        public bool Failed => Remaining <= 0 && !Succeeded;
        public float Remaining { get; private set; } = 35;
        public int Width => cells[0].Length;
        public int Height => cells.Length;
        public char Cell(int x, int y) => cells[y][x];

        public ShortMaze(int layout)
        {
            if (layout < 0 || layout >= Layouts.Length) throw new ArgumentOutOfRangeException(nameof(layout));
            cells = Layouts[layout];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (Cell(x, y) == 'S') { X = x; Y = y; }
        }

        public bool Move(int dx, int dy)
        {
            if (Succeeded || Failed || Math.Abs(dx) + Math.Abs(dy) != 1) return false;
            int x = X + dx, y = Y + dy;
            if (x < 0 || x >= Width || y < 0 || y >= Height || Cell(x, y) == '#') return false;
            X = x; Y = y;
            if (Cell(x, y) == 'K') HasToken = true;
            if (Cell(x, y) == 'E' && HasToken) Succeeded = true;
            return true;
        }

        public void Tick(float seconds)
        {
            if (!Succeeded && !Failed && seconds > 0) Remaining = Math.Max(0, Remaining - seconds);
        }
    }
}
