using System.Collections.Generic;

[System.Serializable]
public class DailyData
{
    /**
    * 하루 루틴동안의 게임 데이터
    * json 로드를 위한 직렬화
    *   - 날짜 정보
    *   - 업무 - 스테이지번호 데이터    
    *   - 시간대별 이동가능 월드 (문마다 스크립트 문자열과 비교)
    *     (특수이벤트도 컷신 데이터 포함된 이동 월드로 구현)
    */

    public Date date;   // 날짜 정보
    public Dictionary<string, int> workData;    // 업무 정보 <업무명, 스테이지>
    public Dictionary<string, Dictionary<int, string>> moveWorldData;   // 월드 이동 데이터 <문 이름, <시간대, 이동 월드>>
}