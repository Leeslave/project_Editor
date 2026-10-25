# develop ← feature/Story 병합 검증

- 기준 develop: `424ee7ad7292b1338cf307f1f7a7bc236ca24fa9`
- 병합 Story: `eb495661a8f458fbb363b09744fc7fbfcfffcb77`
- 사용자 지시: 충돌한 코드·업무 처리는 develop, 대본·진행 데이터는 Story 선택.
- 충돌이 없던 변경은 Git 자동 병합 결과를 유지했습니다.

## 충돌 해결

| 파일 | 선택 |
| --- | --- |
| Assets/Scripts/Chat/Chat.cs | develop |
| Assets/Scripts/GameSystem/ActionHandler.cs | develop |
| Assets/Scripts/MainWorld/WorldSceneManager.cs | develop |
| Assets/Scripts/GameSystem/GameService/Controller/WorkManager.cs | develop의 삭제 유지. 기존 WorkController 사용 |
| Assets/StreamingAssets/ChatData/day 1/[1-1] 오전 사무실.json | Story |
| Assets/StreamingAssets/ChatData/day 1/[1-3] 오후 사무실.json | Story |
| Assets/StreamingAssets/ChatData/day 11/[11-0] 오전 클레이튼의 집.json | Story |
| Assets/StreamingAssets/DayData/dailyData1.json | Story |

선택된 파일은 Git 인덱스와 해당 브랜치의 blob이 동일한지 확인했습니다.

## 검증

- 남은 미해결 충돌: 0개.
- C# 전체 런타임 소스 컴파일: 오류 0개, 경고 194개.
- Unity가 생성했던 csproj의 파일 목록이 오래되어 임시 MSBuild 타깃에서 현행 Assets/Scripts 소스 목록을 사용했습니다. 누락된 로컬 VS Code 분석기는 제외했습니다. 프로젝트 설정은 바꾸지 않았습니다.
- Day 0~13의 일일 데이터 14개와 참조 대화 53개를 JSON으로 읽었고, 참조 대상 파일 누락은 없었습니다.
- Unity 플레이 테스트는 수행하지 않았습니다.
- 기존 Tools/StoryValidation의 .NET 테스트는 Story의 구 WorkManager API를 대상으로 하므로 이 병합 결과에 그대로 적용할 수 없습니다. 이전 IMPLEMENTATION.md의 테스트 통과 기록은 Story 단독 브랜치의 기록입니다.

## 확인된 실행 연동 문제

1. **검사 시작 액션 미지원:** Story의 `day 12/[12-3] 저녁 정보 1반.json`은 마지막에 `EXAM`을 호출하지만 선택한 develop ActionHandler에는 해당 액션 등록이 없습니다. 현재 조합으로는 그 지점에서 검사가 자동 시작되지 않습니다.
2. **저녁 업무 활성화 미연동:** 자동 병합된 Work 데이터에는 `availableFromTime`이 있지만 develop WorkController는 이를 필터링하지 않습니다. Day 12 검사 업무가 오전부터 목록·전체 완료 조건에 포함되어 일반 업무 후 오후 전환을 막을 수 있습니다.
3. **선택지 JUMP 구현:** develop의 ChatJumpAction은 문단 이동 대신 SkipChat 반복을 사용합니다. Story의 Day 1 선택지는 문단 인덱스를 대상으로 하므로 해당 선택지의 실제 동작 확인·호환 처리가 필요합니다.

위 문제를 해결하려면 develop 구조를 기준으로 Story 전용 연결 기능을 별도로 이식해야 합니다. 이번 병합에서는 사용자 지정 버전 선택을 유지하고 이 기능을 임의로 추가하지 않았습니다. 이 병합을 게임 실행 완료나 출시 가능한 상태로 간주하면 안 됩니다.

## 로컬 수정 보존

병합 전 별도 수정이 있던 Anton SDF 폰트와 dailyData1의 시작 위치 이름·대화 앵커 배치는 병합 커밋과 분리하여 작업 트리에 복원합니다. 기존 다른 stash는 변경하지 않습니다.
