# Claude Code Index — 지식·지침·스킬 인덱스

에이전트는 `/task-start`에서 이 파일을 **먼저** 읽고, 작업 주제와 매칭되는 파일만 선별 로드한다.  
모든 지침을 매 세션 통째로 로드하면 토큰 낭비다.

## 읽기 전략

1. **이 파일(`INDEX.md`)만 먼저 읽는다** (수백 토큰).
2. 작업 프롬프트의 키워드와 각 항목의 `keywords` 필드를 매칭한다.
3. 매칭된 파일만 `Read`로 펼친다. 매칭 안 되면 읽지 않는다.
4. `RULES.md` (루트)는 **항상 스캔** (불변 제약).
5. `CLAUDE.md`는 **항상 이미 로드되어 있다** (세션 시작 시 주입).

---

## Level 1 — Language & Engine (범용, 최상단)

다른 Unity/C# 프로젝트에도 그대로 적용되는 지식. 프로젝트 도메인보다 상위.

### [knowledge/RULES.md](knowledge/RULES.md) — 항상 스캔 (범용 코딩 강제 규약)
21개 프로그래밍 규약 (R1~R21).
- **keywords:** simplicity, abstraction, generalization, optimization, code review, dead code, naming, refactor, weed, parallel rework, comment
- **when to read:** **항상** 스캔. 이 파일은 1계층이며, 루트 [`../RULES.md`](../RULES.md)(프로젝트 불변 제약, 3계층)와 **다르다**.

### [knowledge/unity-scripting-gotchas.md](knowledge/unity-scripting-gotchas.md)
모델이 자주 틀리는 Unity 스크립팅 함정 3선.
- **keywords:** serialization depth, serialization null, ISerializationCallbackReceiver, SerializeReference, Dictionary serialize, inline serialization, coroutine stop, enabled false coroutine, WaitForSecondsRealtime, timeScale pause, IL2CPP, Managed Code Stripping, link.xml, Preserve, SerializedObject, FindProperty, NullReferenceException, Slider fillRect, Editor script, make-assets, Rigidbody velocity, linearVelocity, FindObjectOfType, FindObjectsOfType, FindFirstObjectByType, CS1061, compile error, Unity 2022.3, Unity 6 API not available
- **when to read:** `[Serializable]` 필드 설계, 코루틴으로 타이머/일시정지 구현, iOS·Quest 빌드 실패 (MissingMethod/TypeLoad) 대비, Editor Factory 스크립트 작성 시, Rigidbody/물리 코드나 FindObjectOfType류 코드를 새로 작성/수정할 때 (§5: 이 프로젝트는 2022.3.7f1이라 Unity 6 전용 API를 쓰면 컴파일 에러 — 반드시 확인)

### [knowledge/csharp-dotnet.md](knowledge/csharp-dotnet.md)
C#/.NET 언어 핵심 및 메모리 최적화.
- **keywords:** value type, reference type, struct, class, boxing, string, StringBuilder, event, delegate, subscribe, unsubscribe, generic, async, await, CancellationToken, property, field, LINQ, foreach
- **when to read:** 새 타입 설계, async·이벤트 구현, 자료구조 선택, 박싱/할당(GC) 회피 목적

### [knowledge/unity-mobile-performance.md](knowledge/unity-mobile-performance.md)
범용 Unity 모바일 성능 규칙 (Quest 등 모바일 XR 타겟 공통).
- **keywords:** profiling, GC, garbage collection, batching, draw call, SRP Batcher, texture compression, ASTC, audio compression, Canvas, Raycast Target, LOD, Occlusion Culling, shadow, lightmap, frame budget, thermal throttle, Update loop, object pool
- **when to read:** 프레임 드랍·GC 스파이크·빌드 용량 등 성능 이슈 조사 시, 90Hz 유지가 걸린 코드/에셋 설정 변경 시

### [knowledge/debugging/](knowledge/debugging/) — 디버깅 원칙 (per-file)
디버깅 방법론 원칙 10개.
- **keywords:** debugging, assumption, classify, Bohrbug, Heisenbug, symptom, root cause, fix, log, Debug.Log, print debugging, assertion, Assert, binary split, bisect
- **when to read:** 버그·예외(NRE)·오작동 조우 시 및 추적 시나리오 작성

### [knowledge/qa/](knowledge/qa/) — QA 원칙 (per-file)
QA 방법론 원칙 10개. 이 프로젝트(VR·인디) 맥락으로 적용.
- **keywords:** QA, test, testing, 검증, 릴리즈, regression, 회귀, 버그 리포트, severity, pass, fail, localization, 현지화, functional, non-functional, 90Hz, APK, 빌드 스모크
- **when to read:** `/qa` 스킬 실행 시, 릴리즈 전 검증 요청 시, 버그 리포트 작성 시

---

## Level 2 — Project Domain (이 프로젝트 전용)

> 현재 비어 있음 — Camp Lantern은 신규 프로젝트이며 GDD가 아직 없다.
> GDD/도메인 문서가 추가되면 여기에 파일 링크와 `keywords`를 등록한다. 구조는 [domain/README.md](domain/README.md) 참조.

---

## Level 3 — Immutable Constraints

### [../RULES.md](../RULES.md) — 항상 스캔
프로젝트의 절대 불변 제약. 위반 시 시스템이 실제로 망가진다.
- **RULE-01:** Domain Reload 트리거 금지 (InitializeOnLoad, asmdef autoReferenced)
- **RULE-02:** Unity 에셋 파일(.meta/.prefab/.unity/.asset 등) 직접 편집 금지
- **RULE-03:** 물리 연산은 FixedUpdate에서만
- **RULE-04:** ProjectSettings는 Claude가 직접 수정하지 않음
- **keywords:** domain reload, InitializeOnLoad, asmdef, meta, prefab, scene, FixedUpdate, physics, ProjectSettings
- **when to read:** 모든 작업 착수 시. 특히 에셋 파일, 물리, 프로젝트 설정 작업 시. (저장 데이터·플랫폼 분기 등 프로젝트 고유 RULE은 해당 시스템이 생기면 추가된다.)

---

## Level 4 — Path-scoped Rules

특정 경로/파일에 한정된 절대 코딩 규칙.

### [rules/scripts.md](rules/scripts.md)
범용 스크립팅 기본값 — 컴포넌트 캐싱, CancellationToken, 이벤트 구독 해제, Awake 초기화, GameObject 활성화 소유권, UI 초기화 순서. 프로젝트 고유 컨벤션이 정해지면 이 파일에 갱신.
- **keywords:** GetComponent caching, Update, FixedUpdate, CancellationToken, async, coroutine, event -=, event +=, OnDestroy, OnDisable, Awake, SetActive, Rigidbody isKinematic, Collider isTrigger, UI initialization order, OnEnable
- **when to read:** **C# 스크립트(.cs) 생성/수정 시 항상.** 특히 컴포넌트 초기화, 비동기 메서드 생성, 이벤트 구독, UI 초기화 시 필수 확인.

---

## Level 5 — Skills (on-demand)

`.claude/skills/` 하위 SKILL.md 파일로 정의된 커스텀 스킬. `/스킬명` 으로 호출.

| 스킬 | 파일 | 호출 시점 |
|---|---|---|
| `/task-start` | `skills/task-start/SKILL.md` | 코드 작업 착수 전 (자동 수행) |
| `/task-done` | `skills/task-done/SKILL.md` | 코드 작업 완료 후 |
| `/design` | `skills/design/SKILL.md` | 기획을 단계별 실행 계획으로 분해할 때 |
| `/make-assets` | `skills/make-assets/SKILL.md` | Unity 에셋(UI Prefab, Particle, Material 등) 생성할 때 |
| `/debug` | `skills/debug/SKILL.md` | 버그·예외·오작동 원인 추적할 때 |
| `/qa` | `skills/qa/SKILL.md` | 기획·규칙 기준으로 구현 검증할 때 |
| `/self-update` | `skills/self-update/SKILL.md` | 세션에서 습득한 지식을 지식 베이스에 반영할 때 |
| `/lsp-setup` | `skills/lsp-setup/SKILL.md` | C# LSP 연결 안 될 때 (자동 호출) |

## 인덱스 갱신 규칙
- `domain/` 폴더에 새 기획서나 시스템 설명서가 추가되면 이 인덱스(`Level 2`)에 `keywords`와 함께 반드시 등록한다.
- `keywords`는 작업 프롬프트에 실제로 등장할 고유 명사 위주로 작성한다.
