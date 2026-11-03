namespace EditorGame.WorkIntegration
{
    public enum MazeLessonStage { Introduction, Move, Token, Exit, Approved }

    /// <summary>토큰·출구 판정은 ShortMaze에 맡기고 실제 이동만 관찰한다.</summary>
    public sealed class MazeWorkLesson
    {
        public bool IntroRead;
        private readonly int startX, startY;
        private bool moved;
        public MazeWorkLesson(ShortMaze maze) { startX = maze.X; startY = maze.Y; }
        public MazeLessonStage Current(ShortMaze maze)
        {
            if (maze.Succeeded) return MazeLessonStage.Approved;
            if (!IntroRead) return MazeLessonStage.Introduction;
            moved |= maze.X != startX || maze.Y != startY;
            if (maze.HasToken) return MazeLessonStage.Exit;
            return moved ? MazeLessonStage.Token : MazeLessonStage.Move;
        }
    }
}
