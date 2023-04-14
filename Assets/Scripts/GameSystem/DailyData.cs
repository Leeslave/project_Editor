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

    public Date date;
    public Dictionary<string, int> workData;
    public Dictionary<string, Dictionary<int, string>> moveWorldData;
}