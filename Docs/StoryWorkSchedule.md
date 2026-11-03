# 스토리 진행용 임시 업무 배정

기존 `StreamingAssets/DayData/dailyData0~13.json`의 업무 목록을 사용한다. TEST 전용 배정이 아니므로 일반 게임에서도 적용된다. 날짜, 대사, NPC, 위치 및 분기는 유지한다. 저장 파일은 직접 수정하지 않는다.

| 데이터 일자 | 업무 코드 / 기존 스테이지 | 용도 |
| --- | --- | --- |
| Day 0 | Document / 0 | 기존 배정 유지. 프롤로그 대사가 다음 날로 직접 전환한다. |
| Day 1 | ADFGVX_DT / 0 | 기존 해독 튜토리얼 |
| Day 2 | ADFGVX_ET / 0 | 기존 암호화 튜토리얼 |
| Day 3 | Maze / 0 | 기존 미로 튜토리얼 |
| Day 4 | Document / 0 | 일반 진술 대조 |
| Day 5 | ADFGVX / 5, Maze / 1 | 기존 날짜별 암호 과제 + 낮은 난도 미로 |
| Day 6 | ADFGVX / 6, Document / 0 | 기존 날짜별 해독 + 문서 |
| Day 7 | Maze / 1, SecureDocument / 0 | 미로 후 기밀 문서 수정 |
| Day 8 | ADFGVX / 8 | 기존 날짜별 암호 과제 |
| Day 9 | ADFGVX / 9, Document / 0 | 기존 날짜별 암호 과제 + 문서 |
| Day 10 | Maze / 1, SecureDocument / 0 | 미로 후 기밀 문서 수정 |
| Day 11 | Maze / 1, SecureDocument / 0 | 미로 후 기밀 문서 수정 |
| Day 12 | ADFGVX / 12, VoightKampff / 0 | 아침 해독, 저녁 대사에서 검사 |
| Day 13 | 없음 | 기존 핸더슨 고백 및 제2장 종료 |

일반·기밀 문서는 각각 보유한 Stage 0을 재사용하므로 날짜별 내용은 반복된다. 정식 난도 곡선과 스토리별 전용 문서는 이후 작성 대상이다. 닷쥐와 높은 난도의 미로는 이 임시 배정에서 제외했다.

기밀 문서 Stage 0은 Maze 완료 이력만 요구한다. 일반 게임에서는 과거의 완료 이력도 유효하다. 매 실행마다 인증하는 WorkIntegration_TEST의 방식과는 다르다.

Day 12 검사는 `availableFromTime: 3`으로 배정한다. 아침에는 목록과 전체 완료 판정에서 제외하고, 저녁에는 미완료 업무로 표시한다. 아침 해독만 마치면 오후 대사가 열리고, 저녁 대사의 `EXAM` 명령이 실제 검사 씬을 실행한다. 검사 통과·실패 결과는 기존 분기를 따른다.

업무 콘솔에서 표시 이름 또는 코드를 입력한다. 기밀 문서가 안 보이면 먼저 Maze를 완료한다. 이미 해당 날짜를 실행 중이었다면 날짜를 다시 진입해 배정을 다시 불러온다. Screen과 Screen_TEST의 UI 배치는 이 변경으로 수정하지 않는다.

검증: `StoryWorkScheduleValidation.Run`을 Unity batchmode에서 실행한다. 메모리 전용 저장 서비스와 실제 WorkController를 사용해 Day 0~13 데이터, 씬/과제 존재 여부, 기밀 해금, 완료 시 오후 전환, 중복 완료 방지, Day 12의 아침/저녁 분리를 검사한다. DirectPlayWorkService도 같은 분리를 검사한다. 실제 미니게임을 조작하거나 모든 대사를 끝까지 재생하는 검증은 아니다.
