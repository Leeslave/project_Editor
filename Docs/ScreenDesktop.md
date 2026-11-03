# Screen 업무 단말 UI 리메이크

원본 인게임 진입점인 `Assets/Scenes/Screen.unity`는 이전 UI로 보존한다.
리메이크 UI는 별도 `Assets/Scenes/Screen_TEST.unity`에 적용한다.
본편의 단말기 진입과 미니게임 복귀는 `ScreenSceneRoute.Name`을 통해 `Screen_TEST`를 사용한다.
원본 `Screen.unity`는 이전 UI 확인용으로 보존한다.
일반/기밀 통합 TEST를 본편에 이식하거나 날짜별 업무·스토리·저장 규칙을 변경하는 작업은 포함하지 않는다.

## 화면

- 어두운 청록색 바탕, 회색 창, 종이색 본문, 녹색 콘솔을 사용하는 정보국 내부 단말.
- 상단: 기존 게임 로고, 정보국 명칭, 담당자, 전원 상태.
- 왼쪽: 업무 단말 홈, 업무 지원, 수신함, 메신저, 신문 열람, 프로그램 메뉴.
- 홈: 실제 업무 서비스에서 읽은 업무명·코드·완료 상태와 업무 지침.
- 하단: 게임 내 날짜·시간, 기존 전원·재부팅·자리 복귀 제어.
- 1600×1200 기준으로 비율을 유지하며 화면에 맞춰 축소/확대한다.
- 창·입력·스크롤·아이콘은 모두 편집 가능한 Unity uGUI 오브젝트다.

## 재사용한 자산

- Desktop90의 Box / BoxSelected / BoxPressed / BoxDepressGrey 및 닫기 심볼.
- `Assets/Images/Documents/Folder.png`.
- `Assets/Images/UI/MailIcon.png`, `Assets/Images/Sprite/ChatIcon.png`, `news 1.png`, `Logo.png`.
- 프로젝트의 `neodgm.ttf`, `neodgm SDF.asset`.
- 기존 메일·메시지 데이터와 메시지 행/말풍선 프리팹.

원본 이미지·폰트·외부 패키지를 바꾸거나 신규 이미지 자산을 생성하지 않았다.
메일 제목 행만 기존 MailPanel 동작을 사용하는 별도 `RetroMailRow.prefab`으로 만들었다.

## 기존 기능 연결

- ScreenManager의 전원 상태·부팅 애니메이션·종료 확인·복귀 동작을 유지한다.
- WorkManager의 업무 안내 애니메이션·코드 입력·실행 검사를 유지한다.
- MailManager의 일자별 메일 로딩·본문 표시를 새 수신 목록과 스크롤 본문에 연결한다.
- MsgManager의 목록/대화/뒤로 버튼 계층과 기존 프리팹 참조를 유지한다.
- NewspaperManager와 MenuManager는 기존 창 활성화/닫기를 사용한다.
- 기존 Screen 튜토리얼의 팝업 내용은 유지하고, 새 UI의 강조 위치와 패널 참조를 맞춘다.
- 화면을 여는 것만으로 업무 완료·인증·평판·저장을 수행하지 않는다.
- 실제 Play 검증에서 드러난 메일의 날짜 초기화 대기와 종료 확인 시 전원 플래그 확정을 보완했다.
- 기존 메시지·신문의 콘텐츠 구현 범위는 그대로다. 새 대화나 신문 내용을 임의로 추가하지 않았다.

## 관련 코드와 검증

`RetroScreenDesktop`은 표시·창 포커스만 담당한다. 업무의 판정과 저장은 기존 서비스에 위임한다.
`RetroScreenSceneBuilder`는 Screen_TEST에만 UI를 생성하고, 이미 생성된 씬에서는 가독성·스크롤과 참조를 점검한다.
TEST 씬이 없으면 원본을 별도 이름으로 저장한 다음 수정하며 원본에는 저장하지 않는다.
Unity 메뉴 `Tools / Screen Desktop / Build or Verify Screen_TEST UI`로 검증과 미리보기 렌더링을 실행할 수 있다.

`RetroScreenPlaySmoke.Run`은 DirectPlayController의 메모리 전용 서비스를 사용해 실제 Play 모드에서
Screen_TEST를 열고
부팅, 일자별 업무 표시, 기존 업무 콘솔, 메일, 프로그램 전환, 재부팅, 종료를 검사한다.
플레이어 저장 파일은 전후 해시로 변경 여부를 확인한다.

미리보기는 `output/screen-remake/`에 저장한다. UI 구현의 실제 동작 범위는 Unity 로그의
`SCREEN_REMAKE_VERIFY_OK`와 `SCREEN_REMAKE_PLAY_OK`로 확인한다.
