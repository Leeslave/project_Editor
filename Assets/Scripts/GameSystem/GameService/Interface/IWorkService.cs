using System;
using System.Collections.Generic;

namespace GameService
{
    public interface IWorkService : IService
    {
        /// <summary>
        /// 스크린 전원 활성화 상태
        /// </summary>
        public bool isScreenOn { get; set; }

        /// <summary>
        /// 모든 업무 클리어 이벤트
        /// </summary>
        public event Action OnWorkClear;

        /// <summary>
        /// 모든 업무 목록 확인하기
        /// </summary>
        /// <returns>업무데이터 리스트</returns>
        public List<string> GetList();

        /// <summary>
        /// 업무 스테이지 확인하기
        /// </summary>
        /// <param name="workCode">업무의 코드</param>
        /// <returns>해당하는 업무 스테이지번호</returns>
        public int GetStage(string workCode);

        /// <summary>
        /// 업무 완료 처리하기
        /// </summary>
        /// <param name="workCode">업무의 코드</param>
        public bool ClearWork(string workCode);

        /// <summary>
        /// 모든 업무 완료 여부 확인
        /// </summary>
        /// <returns>업무 완료 여부</returns>
        public bool IsWorkClear();
    }
}
