# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

이 파일은 Claude Code(claude.ai/code)가 이 레포지토리에서 작업할 때 참고하는 가이드이다.

## 프로젝트 개요

**Camp Lantern** ("A Last_Robot") — Unity 2022.3.7f1 기반 VR 퍼즐/전투 게임, OpenXR(Quest 계열) 타겟. AutoHand로 VR 손 상호작용(그랩/원거리 그랩)을 구현하고 XR Interaction Toolkit 2.4.3을 rig/locomotion 용도로 병행한다.

실제 게임 코드는 `Assets/A Last_Robot/` 하위에 있으며, 팀원 이니셜별 폴더(`CHG`, `CMS`, `KYH`, `SCS`)에서 각자 작업하고 `A Last_Main/`에서 통합한다.

> 이 파일의 이전 버전은 "신규 프로젝트, GDD 없음, `Scripts/Test.cs` 플레이스홀더만 존재"로 서술되어 있었으나 더 이상 사실이 아니다 (`Assets/Oculus/`, `Scripts/Test.cs` 모두 삭제됨). `.claude/domain/`, `.claude/INDEX.md` 등 하위 문서에도 같은 취지의 낡은 서술이 남아 있을 수 있으니, 실제 상태와 다르면 그쪽을 신뢰하지 말고 코드를 직접 확인할 것.

## 불변 제약 (Inviolable Constraints)

프로젝트 루트의 [`RULES.md`](./RULES.md)에 시스템을 실제로 망가뜨리는 제약이 정리되어 있다 (에디터 멈춤, GUID 손상, 빌드 실패 등). 작업 시작 전 반드시 스캔할 것.

- **RULE-01**: Domain Reload 트리거 금지 (`[InitializeOnLoad]`, `autoReferenced: true`)
- **RULE-02**: Unity 에셋 파일 직접 편집 금지 (`.meta`/`.prefab`/`.unity`/`.asset`)
- **RULE-03**: 물리 API는 `FixedUpdate`에서만
- **RULE-04**: `ProjectSettings/`는 Claude가 직접 수정하지 않음

프로젝트 고유 시스템(저장 데이터, 플랫폼 분기 등)이 생기면 해당 시점에 새 RULE을 추가한다.

## Unity 버전 주의

`ProjectSettings/ProjectVersion.txt` 기준 실제 에디터 버전은 **2022.3.7f1**이다. Unity 6(6000.x)/2023.1+에서 새로 생긴 API(`Rigidbody.linearVelocity`, `FindFirstObjectByType`/`FindObjectsByType` 등)는 이 버전에 **존재하지 않는다** — 쓰면 경고가 아니라 컴파일 에러. 자세한 목록·구버전 대응 API는 [`.claude/knowledge/unity-scripting-gotchas.md`](./.claude/knowledge/unity-scripting-gotchas.md) §5 참조.

## 추가 규칙 및 컨벤션

프로젝트 지식은 `.claude/` 하위에 계층별로 정리되어 있다. 필요에 따라 참조할 것:

### `.claude/rules/` — 경로 기반 코딩 규칙 (자동 로드)

- `scripts.md` — 컴포넌트 캐싱, 비동기 CancellationToken, 이벤트 구독 해제, Awake 초기화, GameObject 활성화 소유권, UI 초기화 순서. `Assets/Scripts/**/*.cs`에 적용되는 범용 기본값 — 이 프로젝트의 실제 코드(`Assets/A Last_Robot/`)는 아직 이 경로 밖이므로 참고용으로만 취급하고, 실제 코드에서 관측되는 컨벤션과 다르면 실제 코드를 우선한다.

### `.claude/domain/` — 이 프로젝트 고유 설계 의도

시스템을 수정하기 **전에** 관련 기획서를 읽어 "왜 이렇게 설계됐는가"를 파악할 것. 현재 이 폴더는 비어 있지만(GDD 미도착), 실제로는 이미 상당한 게임 시스템이 구현되어 있다 — 아래 "핵심 게임플레이 시스템" 절이 코드에서 관측한 잠정 요약이다. 시스템을 깊이 수정하기 전에 팀원 폴더 내 관련 코드를 직접 읽어 설계 의도를 재확인할 것.

- `gdd/` — 기획서 도착 시 여기에 추가하고 INDEX.md Level 2에 등록.

### `.claude/knowledge/` — 범용 Unity/C# 레퍼런스

언어·엔진 베스트 프랙티스가 헷갈릴 때 참조:

- `RULES.md` — 21개 범용 코딩 원칙 (R1-R21)
- `csharp-dotnet.md` — 값 타입, 박싱, 이벤트, async, LINQ
- `unity-scripting-gotchas.md` — 직렬화 함정, 코루틴 주의점, IL2CPP, Unity API 리네임
- `unity-mobile-performance.md` — 모바일/XR 성능 규칙 (프로파일링, GC, 배칭, UI 등)
- `debugging/` — 디버깅 원칙 10개
- `qa/` — QA 원칙 10개

### `.claude/INDEX.md`

위 모든 계층에 대한 키워드 기반 라우팅. 어떤 파일을 열어야 할지 모를 때 참고.

## 빌드 커맨드

커스텀 빌드 파이프라인 없음. Unity 에디터의 `File > Build Settings`를 사용한다. `com.unity.test-framework` 패키지는 설치되어 있으나 실제 테스트 코드(`*Tests.cs`, `Tests/` 폴더)는 존재하지 않는다 — 자동화된 테스트/린트 커맨드가 없다. 빌드 자동화 스크립트가 추가되면 여기에 기록한다.

## 아키텍처

### 팀 폴더 구조

`Assets/A Last_Robot/`이 사실상의 게임 코드 루트다. 팀원 이니셜별로 분리되어 있고, 통합은 `A Last_Main/`에서 이루어진다:

- `A Last_Main/` — 통합 씬(스테이지별 Puzzle/Battle)과 공용 스크립트(Player, Enemy Bullet, Portal, UI)
- `CHG/` — 적 AI/발사체 시스템(`Enemy_HG/UTL/Managers`의 SG* 오브젝트 풀형 발사체 매니저), 1인칭 로봇 컨트롤러, 모바일 입력 UI
- `CMS/` — 상호작용 오브젝트(문, 파괴 가능 오브젝트), 씬 전환 트리거
- `KYH/` — 홀로그램/비디오 연출
- `SCS/` — 초기 스켈레톤(`Class`/`Scripts`/`Prefab`/`Scenes`)

프로젝트 전체가 `.asmdef` 경계 없이 기본 `Assembly-CSharp`으로 컴파일된다 — 서드파티 패키지(AutoHand, NaughtyAttributes, XRI Starter Assets)만 자체 asmdef를 갖는다. RULE-01(Domain Reload 금지)과 맞물려, 신규 `.asmdef` 추가나 기존 asmdef 구조 변경은 특히 주의.

### 씬 구성

스테이지별로 Puzzle/Battle 씬이 나뉜다 (`A Last_Main/Scene/`의 `S1 PuzzleMain`, `S1 BattleMain`, `S2 PuzzleMain`, `S3 PuzzleMain` 등). 씬 전환은 `SceneTransitionTrigger`(CMS)와 `StartButtonHandler`(UI)로 트리거된다. 팀원별 폴더에도 각자의 작업용/데모 씬이 따로 있다 (`CHG/Scene_HG`, `KYH/Scean`, `CMS/*.unity`, `SCS/Scenes`) — 이들은 최종 통합 씬이 아니라 개인 작업 스페이스이므로 어느 씬을 대상으로 작업하는지 혼동하지 않도록 주의한다.

### 핵심 게임플레이 시스템 (코드에서 관측, GDD 없음 — 정식 명세 아님)

- **엘리멘탈 시스템**: `SetElemental`/`ElementChange`(A Last_Main/Player) — `RadialSelection`으로 속성(마그네틱/파이어)을 선택하면 AutoHand의 `DistanceGrabbable`/`Grabbable` 컴포넌트를 on/off해 상호작용 가능한 오브젝트를 속성별로 토글.
- **발사체/전투**: 플레이어 측 `MeteorPistor`/`PlayerBulletImpact`(A Last_Main) 대 적 측 `EnemyBullet*`/`SGShotManager`·`SGProjectileManager`·`SGObjectPool`(CHG). SG* 계열은 오브젝트 풀 기반 발사 매니저.
- **포탈/트리거**: `PotalFuntion` + `PotalTag` — 트리거 진입 시 들어온 오브젝트를 비활성화하고, 지정된 대체 오브젝트를 활성화·재배치.
- **VR 상호작용**: 손 그랩/거리 그랩은 AutoHand(`Assets/AutoHand`) 기반. Meta/Oculus 네이티브 SDK는 프로젝트에 없으며, OpenXR + XR Interaction Toolkit 2.4.3을 병행 사용.

## 코드 컨벤션

- **한글 주석** 허용 — 실제 코드 전반에서 한글 주석이 표준으로 쓰이고 있다.
- 팀원별 폴더는 접미사로 작성자를 구분한다 (`_HG`, `_CMS` 등). 파일/클래스를 새로 만들 때 어느 팀원 폴더에 속하는지, 통합 폴더(`A Last_Main`)로 옮길지 먼저 확인할 것.
- 싱글톤은 `public static T instance { get; private set; }` + `Awake()`에서 할당하는 패턴이 쓰인다 (`GameManager` 등). 새 싱글톤을 추가할 때 기존 패턴을 따르되, 기존 구현에 결함이 보이면 임의로 "정리"하지 말고 사용자에게 먼저 보고한다.
- 그 외 컨벤션(비동기 라이브러리 선택, 이벤트 시스템 등)은 코드가 쌓이면서 정해지는 대로 이 섹션과 `.claude/rules/scripts.md`에 기록한다.

## 주요 패키지

`Packages/manifest.json` 기준 UPM 패키지:

- `com.unity.render-pipelines.universal` 14.0.8 (URP)
- `com.unity.xr.interaction.toolkit` 2.4.3, `com.unity.xr.openxr` 1.8.2, `com.unity.xr.management` 4.5.0
- `com.unity.cinemachine` 2.9.7, `com.unity.timeline` 1.7.5, `com.unity.textmeshpro` 3.0.6
- `com.unity.ai.navigation` 1.1.6, `com.unity.visualscripting` 1.8.0

UPM이 아니라 `Assets/` 하위에 직접 벤더링된 주요 에셋:

- `Assets/AutoHand` — VR 손 상호작용 핵심 프레임워크. 그랩 관련 코드는 대부분 `Autohand` 네임스페이스에 의존한다.
- `Assets/Photon` — PhotonLibs만 존재하며, 프로젝트 코드에서 `PhotonNetwork`/`PhotonView` 참조가 확인되지 않는다 (멀티플레이 미사용 또는 아직 미연결).
- `Assets/Sci-Fi Styled Modular Pack`, `Assets/Same Gev Dudios`, `Assets/3Dynamite`, `Assets/WorldMaterialsFree` 등 — 아트/환경 에셋.

## 플랫폼 노트

OpenXR 로더가 구성되어 있다 (`Assets/XR/Loaders/OpenXRLoader.asset`). VR 손 상호작용은 AutoHand 기반, Meta/Oculus 네이티브 SDK는 프로젝트에 없다. 정확한 타겟 기기(Quest 단독인지 PICO 등도 포함하는지)는 GDD 확정 후 여기에 기록한다.
