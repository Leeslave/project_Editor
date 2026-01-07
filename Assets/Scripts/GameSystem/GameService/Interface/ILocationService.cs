namespace GameService
{
    public interface ILocationService : IService
    {
        /// <summary>
        /// 월드 내 지역 이동
        /// </summary>
        /// <param name="location"></param>
        /// <param name="position"></param>
        public void MoveLocation(World location, int position);
    }
}