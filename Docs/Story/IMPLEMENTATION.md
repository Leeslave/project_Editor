# 1~2장 스토리 개정 구현 보고서

대상: 현재 로컬 `feature/Story`, 작업 시작 HEAD `7a4f0c10`. 로컬이 추적 중인 원격 `feature/Story`는 `6b0ccbd9`이며 7개 커밋 앞서 있었습니다. 기존 미커밋 수정을 보존하기 위해 이번 작업에서 pull/merge, 커밋, 푸시는 하지 않았습니다.

입력 문서의 확정 설정을 부록 초안보다 우선했습니다. 특히 초안 Day 8의 헤이탄톤/전쟁 조사 설명을 1~2장에 넣지 않았습니다.

## 사건별 변경과 대응

| 날짜 | 변경 이유 |
| --- | --- |
| Day 0 | 심문 프롤로그를 후반 조사와 맞추고 날짜·배경 스포일러 제거. 사용자 수정 [0-1] 파일은 그대로 유지. |
| Day 1 | 스크린 소개의 확정적인 감시 설명을 완화하고 클로버 재회·렉스 독감·ID 카드·복호화 연결. 기존 선택지와 사용자 dailyData1 유지. |
| Day 2 | 핸더슨의 일주일 대타 승인과 3반 업무 대행, 암호화 학습 및 친구 관계 정리. |
| Day 3 | 가정사와 아이들(아들·딸), 별거·연락 단절 및 식사 제안. 영웅적 비밀 경력 추가 없음. |
| Day 4 | 오래된 업무 노트와 보안 프로그램 튜토리얼, 식사 약속. |
| Day 5 | USB 전달, 스크린·기록 수정 경고, 친구 보호 의도와 첫 식사. 케네디 최고 비서 설정 반영. |
| Day 6 | 렉스 복귀·진심 어린 감사, 클로버 메시지로 1장 마무리. |
| Day 7 | 해킹 의혹 도입과 핸더슨의 내부 지침. 브리핑 전달 실수를 사전 배신으로 확정하지 않음. |
| Day 8 | 핸더슨이 확인된 정보와 추측을 구분해 경고. 프라이스 술집/귀가 장면은 일정에서 제외. |
| Day 9 | 커피·인사 약속과 렉스의 감사 및 식사 제안. |
| Day 10 | 프라이스 카페 등장을 제거하고 셋의 식사·귀갓길·클로버의 침묵 유지. |
| Day 11 | 체포와 국적 주장을 정부의 공식 보도로 명시. 사무실 진입 차단을 풀고 핸더슨 집무실로 연결. |
| Day 12 | 월터의 첫 방문 → 정상 업무 → 가족 정보로 렉스 압박 → 사실 증언 → 구금·심문 → 보이드캄프 → 성공/실패. |
| Day 13 | 검사 통과자만 NEXTDAY로 진입하는 다음 날 새벽의 2장 마지막 대면. 예전 검사/QA 일정 대신 고백·분노·유보된 협력으로 종료. |

## 날짜와 검사 분기

Day 12의 원래 일반 업무 `ADFGVX(stage 12)`와 `Maze(stage 9)`를 유지했습니다. `VoightKampff(stage 0, availableFromTime 3)`를 추가하여 오전에는 검사 업무가 완료 조건/명령 목록에 들어가지 않고, 저녁에만 활성화되도록 했습니다. 기존 미니게임 코드/스테이지는 변경하지 않았습니다.

- 시간대 0 → 집 대화 끝에서 NEXTTIME 1.
- 시간대 1 → 월터 첫 방문과 일반 업무. 두 업무를 모두 완료한 뒤 시간대 2.
- 시간대 2 → 퇴근 대화 끝에서 NEXTTIME 3.
- 시간대 3 → 렉스와 월터 대화, 귀갓길 구금, 심문. 기존 POSCHANGE로 거리/심문실 위치를 전환하고 EXAM 액션으로 검사 씬 진입.
- 성공 → 시간대 4의 일시적 조사 통과 대사 → NEXTDAY → Day 13 새벽 핸더슨 집무실.
- 실패 → 시간대 5의 추방 대사/기존 망명 CG → EXIT. 다음 날 세이브를 해금하지 않음.
- Day 13 새벽 → 혁명단 지도자·영입 계획 고백, 단독 작전을 몰랐다는 해명, 책임 인정, 클레이튼의 분노와 가입/용서 유보 → 제2장 종료 → EXIT.

검사의 성공 판정은 기존 복합 턴 성공 조건을 재사용했습니다. Day 12에서 턴 실패는 재시도 대신 추방 분기로 전환합니다. 새 점수/판정 임계값은 임의로 추가하지 않았습니다. Day 13은 3장이 아니라 다음 날 새벽의 2장 에필로그입니다. 기존 심문 파일을 인덱스만 바꿔 실행하는 방식은 사용하지 않았습니다.

## 원본 보존·초안 반영·새로 작성한 부분

원본의 장면 구성과 업무 코드/스테이지, ID 카드 상호작용, 기존 선택지 데이터, CG 리소스·표정 인덱스, USB 전달, 첫 식사, 3인 식사, 커피와 귀갓길의 침묵을 보존했습니다. 같은 의미의 대사는 제공된 개정 초안으로 다듬었습니다. BGM/SFX/글자 출력 설정과 초상화는 기존 정상 문단을 재사용했고, 실제로 자리에 없는 인물의 초상화는 제거했습니다.

작업 시작 때 이미 수정되어 있던 `day 0/[0-1] 오전 정보 1반.json`과 `DayData/dailyData1.json`은 백업과 바이트 단위로 같음을 확인했습니다. 폰트 두 파일과 ShaderGraph 설정의 사용자 변경도 직접 편집하지 않았습니다.

새로 작성/보완한 대사는 Day 7 핸더슨 집무실의 짧은 내부 지침, 최종 대면의 부장급 감시·하급 직원 접근 권한·대타 영입 계획 설명, Day 13 조사 종료 통지입니다. 모두 문서의 확정 설정을 연결하기 위한 문장입니다. 초안의 ‘렉스가 클로버 업무를 대신했다’는 역전된 감사 대사는 클로버가 렉스를 대신 근무한 것으로 바로잡았습니다. 새 영웅 경력, 혁명단 상세 조직, 클로버 유년기/마이클, 렉스 용서 선택, 3장 본편은 추가하지 않았습니다.

## 프라이스와 기존 자료 처리

원래 Day 7 [7-1], Day 8 [8-0]/[8-5]/[8-6] 파일은 삭제하거나 덮어쓰지 않고 실행 일정에서만 제외했습니다. 새 Day 7 [7-H]와 Day 8 핸더슨 장면이 필요한 사건 전달을 담당합니다. Day 10 카페는 클로버와 클레이튼만 등장하도록 수정했고, 해당 원본 JSON은 `Deferred/Price_day10_original.json`에 보관했습니다. 프라이스·헤이탄톤·전쟁 조사 자료는 3장에서 재배치할 수 있도록 남겼으며 실제 3장 일정은 작성하지 않았습니다.

기존 Resources/Chat/Text의 후속 초안과 Day 14~15는 수정하지 않았습니다. 옛 Day 13 심문/추방 및 QA JSON도 역사 자료로 남아 있지만 새 일정에서는 참조하지 않습니다.

## 진행 코드 수정 이유

- Chat: JUMP를 지정 문단 이동으로 처리하고, 스킵 시 현재 마지막 문단의 진행 액션까지 실행. 선택지의 이전 액션 재실행과 종료 후 타이핑 코루틴 잔류를 방지.
- ActionHandler: EXAM 씬 연결, 엔딩 EXIT에서 additive로 열린 게임 씬 정리.
- DailyData/WorkManager: 늦은 업무 활성 시각, 업무 이름이 없을 때 코드 사용, 실제 완료 상태 확인. 원래 `All(done => true)` 때문에 하나만 해도 전부 완료되던 문제 수정.
- DayController: Day 4~6의 비어 있는 0번 시간대를 건너뛰고 실제 오전 대화에서 시작. 해당 일일 데이터의 기존 일정은 그대로 유지.
- LocationController: 3/4/6개 시간대에 맞춰 이동 제한 배열 생성, 날짜 전환 중 이전 시간 인덱스 방어.
- WorldSceneManager: 스크린/검사에서 월드로 복귀할 때 현재 시간·위치 복원.
- VK_ManagerScript: 누락된 WorkService 초기화, 실패 재시도용 턴 핸들 정리, 검사 결과의 중복 처리 방지 및 Day 12 전용 결과 분기.

오전→업무 전환이 없던 Day 1 복도, Day 2 집, Day 9/10 카페, Day 12 집에 NEXTTIME 1을 추가했습니다. Day 11은 라디오의 사무실 이동과 충돌하던 입실 차단을 제거하고 기사 확인 후 핸더슨 집무실로 이어지게 했습니다.

## 검증 결과

- 전체 C# 프로젝트 컴파일: 오류 0개, 경고 202개(주로 Unity 직렬화 필드 미할당 경고).
- 저장된 csproj가 가리키는 로컬 VS Code Unity 분석기 DLL이 없어, 임시 MSBuild 타깃으로 분석기만 제외한 후 컴파일했습니다. 프로젝트/분석기 설정 파일을 수정하지 않았습니다.
- Newtonsoft.Json `TypeNameHandling.All`로 실제 데이터 클래스 역직렬화: 대화 JSON 69개, 문단 1,008개, 일일 데이터 14개 통과.
- 활성 일정 검사: 참조 대화 53개, 문단 728개, 경로·타입 컨테이너·액션·선택지·리소스 존재·오전 연결·진입 차단·성공/실패 경로 통과.
- 원본 선택지 객체 보존, 사용자 수정 파일 두 개 바이트 동일 확인.
- 실제 WorkManager/DayController/LocationController 소스를 사용하는 .NET 테스트: 일반 업무 둘 다 완료, 검사 시간 제한, 검사 완료 시 오후로 되돌아가지 않음, 결과 시간대 이동, 3개 시간대 날짜 및 다음 날 전환 검증 통과.
- .NET 테스트의 Unity 호스트는 스텁입니다. Unity 에디터 플레이 테스트, 씬 로딩 중 프레임 동작, 입력/난도/오디오·CG 배치 검증은 하지 않았습니다.

재현 명령(저장소 루트):

```powershell
python Tools/StoryValidation/validate_story.py
dotnet restore Tools/StoryValidation/StoryValidation.csproj --source .
dotnet run --project Tools/StoryValidation/StoryValidation.csproj --no-restore -- .
```

테스트 호스트는 .NET 9와 Unity가 이미 받은 Newtonsoft 패키지 캐시를 사용하며 실행 게임의 .NET/Unity 버전을 변경하지 않습니다.

## 남은 확인 항목

연도는 자료가 일치하지 않습니다. Day 1/3에는 23년, 여러 날짜에는 17년, Day 0에는 8년이 있으며 Day 5/11/13의 원래 연도는 0입니다. 문서 지시대로 새 연도를 확정하지 않았습니다. Day 11의 12월 27일, Day 13의 12월 29일은 기존 메일과 Day 12의 12월 28일에 근거해 월/일만 채웠습니다. Day 5의 0/0/0 및 연도 정합성은 설정 확정 후 별도 정리가 필요합니다.

새 조사관·보좌관 그림이나 별도 음향 리소스는 추가하지 않았고, 지원되지 않는 연출은 내레이션과 기존 CG/페이드로 표현했습니다. 기존 일반 미니게임 데이터 자체는 유지했으며 난도/모든 스테이지의 실제 플레이 가능성까지 검증한 것은 아닙니다.

기존에 해금된 Day 13 저장 데이터는 이제 에필로그로 열립니다. 개정된 체포→검사 흐름을 확인하려면 새 게임 또는 Day 12부터 다시 진행해야 합니다. 실패한 새 진행은 Day 13을 새로 해금하지 않습니다.

Unity 확인 절차:

1. 새 게임에서 프롤로그 → Day 1 두 아침 선택지의 각 선택 → 복도 → 담화·ID 카드 → 복호화 → 오후/다음 날 진행. 스킵도 첫 문단/마지막 문단 양쪽에서 확인.
2. Day 4~6 오전 노트/USB/렉스 복귀가 보이고, 두 개 업무가 있는 날 하나만 끝내면 오후가 열리지 않는지 확인.
3. Day 7/8/10에 프라이스 초상화/음성/기존 술집 장면이 나오지 않는지, Day 10 식당과 귀갓길 CG를 확인.
4. Day 11 기사에서 정부 발표 표현, 핸더슨 집무실 전환, Day 12 월터 방문 후 업무·렉스 협박·구금 순서 확인.
5. Day 12 검사 튜토리얼 후 한 번은 정상 완료, 한 번은 시간 초과/눈동자 이탈로 실패. 성공은 일시적 조사 통과 후 Day 13 새벽 대면, 실패는 추방 후 시작 화면으로 복귀하는지 확인.
6. Day 13에서 핸더슨의 단독 작전 무지/책임과 클레이튼의 가입·용서 유보가 유지되고, 종료 후 3장 대본이 임의로 시작되지 않는지 확인.

## 변경 파일 전체 목록

아래 목록은 작업 시작 시점의 작업 트리와 비교한 이번 변경만 포함합니다. 기존 사용자 변경 파일은 위 보존 항목에 별도 기재했습니다.

- `Assets/Scripts/Chat/Chat.cs`
- `Assets/Scripts/GameSystem/ActionHandler.cs`
- `Assets/Scripts/GameSystem/GameData/DailyData.cs`
- `Assets/Scripts/GameSystem/GameService/Controller/DayController.cs`
- `Assets/Scripts/GameSystem/GameService/Controller/LocationController.cs`
- `Assets/Scripts/GameSystem/GameService/Controller/WorkManager.cs`
- `Assets/Scripts/MainWorld/WorldSceneManager.cs`
- `Assets/Scripts/Voight/VK_ManagerScript.cs`
- `Assets/StreamingAssets/ChatData/day 0/[0-0] 오전 클레이튼의 집.json`
- `Assets/StreamingAssets/ChatData/day 10/[10-0] 오전 회사 앞 카페.json`
- `Assets/StreamingAssets/ChatData/day 10/[10-1] 오전 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 10/[10-2] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 10/[10-3] 저녁 일식당.json`
- `Assets/StreamingAssets/ChatData/day 10/[10-4] 밤 1번가.json`
- `Assets/StreamingAssets/ChatData/day 11/[11-0] 오전 클레이튼의 집.json`
- `Assets/StreamingAssets/ChatData/day 11/[11-1] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 11/[11-2] 오전 핸더슨의 집무실.json`
- `Assets/StreamingAssets/ChatData/day 11/[11-3] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 11/[11-4] 오후 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-0] 오전 클레이튼의 집.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-1] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-2] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-3] 저녁 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-4] 검사 통과.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-4] 검사 통과.json.meta`
- `Assets/StreamingAssets/ChatData/day 12/[12-5] 검사 실패 추방.json`
- `Assets/StreamingAssets/ChatData/day 12/[12-5] 검사 실패 추방.json.meta`
- `Assets/StreamingAssets/ChatData/day 13/[13-H] 새벽 핸더슨의 고백.json`
- `Assets/StreamingAssets/ChatData/day 13/[13-H] 새벽 핸더슨의 고백.json.meta`
- `Assets/StreamingAssets/ChatData/day 1/[1-0] 오전 클레이튼의 집.json`
- `Assets/StreamingAssets/ChatData/day 1/[1-1] 오전 복도.json`
- `Assets/StreamingAssets/ChatData/day 1/[1-1] 오전 사무실.json`
- `Assets/StreamingAssets/ChatData/day 1/[1-2] 오전 사무실 ID 카드.json`
- `Assets/StreamingAssets/ChatData/day 1/[1-3] 오후 사무실.json`
- `Assets/StreamingAssets/ChatData/day 2/[2-0] 오전 클레이튼의 집.json`
- `Assets/StreamingAssets/ChatData/day 2/[2-1] 오전 사무실.json`
- `Assets/StreamingAssets/ChatData/day 2/[2-2] 오후 사무실.json`
- `Assets/StreamingAssets/ChatData/day 3/[3-0] 오전 클레이튼의 집 1.json`
- `Assets/StreamingAssets/ChatData/day 3/[3-1] 오전 핸더슨의 집무실 1.json`
- `Assets/StreamingAssets/ChatData/day 3/[3-2-1] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 3/[3-3] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 4/[4-0] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 4/[4-1] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 5/[5-0] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 5/[5-1] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 5/[5-2] 오후 핸더슨의 집무실.json`
- `Assets/StreamingAssets/ChatData/day 5/[5-3] 저녁 정보반 복도 1.json`
- `Assets/StreamingAssets/ChatData/day 5/[5-4] 밤 일식당.json`
- `Assets/StreamingAssets/ChatData/day 6/[6-0] 오전 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 6/[6-1] 오후 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-0] 오전 클레이튼의 집 1.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-2] 오전 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-3] 오후 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-4] 저녁 대강당.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-H] 오전 핸더슨의 집무실.json`
- `Assets/StreamingAssets/ChatData/day 7/[7-H] 오전 핸더슨의 집무실.json.meta`
- `Assets/StreamingAssets/ChatData/day 8/[8-1] 오전 정보 1반.json`
- `Assets/StreamingAssets/ChatData/day 8/[8-2] 오전 핸더슨의 집무실.json`
- `Assets/StreamingAssets/ChatData/day 8/[8-3] 오전 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 8/[8-4] 오후 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 9/[9-0] 오전 회사 앞 카페.json`
- `Assets/StreamingAssets/ChatData/day 9/[9-1] 오전 정보 1반 1.json`
- `Assets/StreamingAssets/ChatData/day 9/[9-2] 오후 정보 1반.json`
- `Assets/StreamingAssets/DayData/dailyData11.json`
- `Assets/StreamingAssets/DayData/dailyData12.json`
- `Assets/StreamingAssets/DayData/dailyData13.json`
- `Assets/StreamingAssets/DayData/dailyData7.json`
- `Assets/StreamingAssets/DayData/dailyData8.json`
- `Assets/StreamingAssets/MailData/Day 13/제1정보반 12월 29일 전달사항.txt`

추가 검토·검증 산출물:

- `Docs/Story/IMPLEMENTATION.md` — 본 보고서
- `Docs/Story/RUNTIME_SCRIPT.md` — 실제 참조 대본의 읽기용 검토본
- `Docs/Story/scene_mapping.json` — 변경 장면과 문단 수 대응
- `Docs/Story/Deferred/Price_day10_original.json` — Day 10 프라이스 장면 원본
- `Tools/StoryValidation/StoryValidation.csproj`
- `Tools/StoryValidation/EngineStubs.cs`
- `Tools/StoryValidation/Program.cs`
- `Tools/StoryValidation/validate_story.py`
- `Tools/StoryValidation/.gitignore`

## 커밋 준비 시 원격 변경 통합

사용자 요청으로 수정본을 복구하고, 기존 원격 feature/Story의 7개 커밋 위에 개정본을 재적용했습니다. 겹친 Day 1~2의 구 대사는 개정 대본으로 해결했습니다. 원격 dailyData1의 존재하지 않는 `[1-0] 오전 복도.json` 참조를 실제 `[1-1] 오전 복도.json`로 수정하고, 원본 작업 트리의 ID 카드 상호작용 데이터를 재사용해 Nametag 연결을 포함했습니다. 기존 사용자의 시작 위치 이름·대화 앵커 배치는 커밋과 분리해 로컬에 보존합니다.
