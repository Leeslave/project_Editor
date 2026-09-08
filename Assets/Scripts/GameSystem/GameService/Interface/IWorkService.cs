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
        /// 현재 실행 업무 코드 (날짜 초기화 후에는 null)
        /// </summary>
        public string CurrentWorkCode { get; }

        public event Action OnWorkClear;

        /// <summary>
        /// 모든 업무 목록 확인하기
        /// </summary>
        /// <returns>업무 코드와 표시 이름의 사본 목록 (이름 누락 시 코드 사용)</returns>
        public List<(string code, string name)> GetList();

        /// <summary>
        /// 미완료 업무와 씬을 확인한 뒤 실행 업무 코드를 설정
        /// </summary>
        /// <param name="workCode">실행할 업무 코드</param>
        /// <param name="sceneName">로드할 씬 이름</param>
        /// <returns>실행 정보 설정 성공 여부</returns>
        public bool TryStartWork(string workCode, out string sceneName);

        /// <summary>
        /// 개별 업무 완료 여부 확인
        /// </summary>
        /// <param name="workCode">확인할 업무 코드</param>
        /// <returns>완료 여부 (미등록 업무는 false)</returns>
        public bool IsWorkClear(string workCode);

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
        /// <returns>전체 업무 완료 여부 (미등록 코드는 false)</returns>
        public bool ClearWork(string workCode);

        /// <summary>
        /// 모든 업무 완료 여부 확인
        /// </summary>
        /// <returns>업무 완료 여부</returns>
        public bool IsWorkClear();
    }
}
