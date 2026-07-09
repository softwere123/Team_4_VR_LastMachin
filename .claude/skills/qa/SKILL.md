---
name: qa
description: 기획·스펙·규칙에 비추어 구현을 평가하는 툴 사용 지침. 사용자에게 "플레이해 보고 문제 있나 말해 달라"를 시키지 않고, 에이전트가 LSP 로 소스를 읽고 스스로 합격·불합격을 판정한다.
---

# Skill: /qa (Agent-driven QA)

**전제:** QA 의 책임은 에이전트에게 있다. 사용자에게 Play 후 감각으로 "이상하다" 보고 받지 않는다. 에이전트가 **합격 기준을 문서로 명시**하고 → LSP·Grep 으로 정적 평가 → 기준별 Pass/Fail 보고한다.

이 문서는 **어떤 툴로 어떤 기준을 검증할지**만 다룬다. 작업 착수·종료 절차는 `/task-start`, `/task-done` 참고.

### 스케일 전제 — 인디 개발자

이 템플릿은 인디·소규모 팀용이다. QA 엔진이 하는 일은 **사용자가 명시적으로 요청했을 때** 돌아가야 한다. 다음은 **기본 동작 아님**:

- ❌ 매 `/task-done` 뒤 자동 QA 1패스 — 느리고 과하다.
- ❌ 기획 조항 N 개면 N 개 전수 평가 — 중요 조항 위주.

**대신 기본 동작:** 사용자가 "QA 해 줘" 또는 "릴리즈 전 검증" 을 요청하면 그때 실행.

---

## 1. 합격 기준의 출처

검증하기 전에 기준을 **문서 링크**로 박는다. 기준 없는 QA 는 감상문이다.

| 출처 | 성격 | 언제 쓰나 |
|---|---|---|
| [`RULES.md`](../../RULES.md) | 불변 제약 | 릴리즈 전 / 의심될 때 스캔 |
| [`.claude/knowledge/RULES.md`](../../knowledge/RULES.md) | 범용 코딩 규약 21개 | 코드 품질 QA |
| [`.claude/rules/scripts.md`](../../rules/scripts.md) | `.cs` 경로·코딩 규칙 | `.cs` 신규·수정 QA |
| [`.claude/knowledge/csharp-dotnet.md`](../../knowledge/csharp-dotnet.md) | C# 스펙 규약 | 타입·async·이벤트 QA |
| [`.claude/knowledge/unity-scripting-gotchas.md`](../../knowledge/unity-scripting-gotchas.md) | Unity 직렬화·코루틴 함정 | Unity 특화 QA |
| `.claude/domain/*.md` / 기획서 | 프로젝트 고유 행동 규약 | 기능 QA |

기준이 명시 안 되어 있으면 QA 하지 않는다. 먼저 사용자에게 기준을 묻거나 `/design` 으로 스펙 추출.

---

## 2. 툴 매트릭스 — 기준 유형별 검증 툴

| 기준 유형 | 1순위 (정적) | 증거 형태 |
|---|---|---|
| **RULE-01** Domain Reload 미트리거 | `Grep '\[InitializeOnLoad\]'` + `Grep '"autoReferenced": true'` | grep 매치 0 건 |
| **RULE-02** Unity 에셋 파일 직접 편집 금지 | `git diff --stat "*.meta" "*.prefab" "*.unity" "*.asset"` | diff 라인 0 |
| **RULE-03** 물리 API = FixedUpdate 전용 | `Grep` 으로 `Update\|LateUpdate` 내 `AddForce/MovePosition` 스캔 + `LSP.incomingCalls` 로 호출자 맥락 | 호출자 모두 `FixedUpdate` 계열 확인 |
| **RULE-04** ProjectSettings 미수정 | `git diff --stat ProjectSettings/` | diff 라인 0 |
| 이벤트 구독·해제 쌍 (`rules/scripts.md`) | `Grep "\+=\s*(?:this\.)?\w+"` + 같은 파일에서 `-=` 매칭 | `+=` 라인마다 `OnDestroy`/`OnDisable` 에 대응 `-=` 존재 |
| `GetComponent` 캐싱 (`rules/scripts.md`) | `Grep "Update\|FixedUpdate" -A 20` 에서 `GetComponent` 동거 여부 | `Update()` 내 `GetComponent` 호출 0 건 |
| 기능 행동 (기획서 조항) | `LSP.findReferences` / `outgoingCalls` 로 조항 구현 지점 확인 | 조항별 Pass/Fail + 파일:라인 |

---

## 3. 정적 평가 — LSP / Grep / Read

QA 의 대부분은 정적 평가로 해결된다.

### 커버리지 전수 확인

기획서 조항 하나하나가 **실제로 구현됐는지**를 LSP 로 증명:

```
기획서 "OOO 3회 완료 시 보상 지급"
  → LSP.workspaceSymbol "RewardTrigger"    (심볼 발견)
  → LSP.findReferences                     (모든 사용처)
  → LSP.outgoingCalls 각 지점              (어떤 경로로 보상 지급)
  → Read 해당 라인                          (조건 카운트 + 보상 로직 확인)
```

조항마다 증거 라인 (파일:라인) 을 보고에 남긴다.

### 규칙 전수 스캔 스니펫

```
# 이벤트 누수
Grep "\+=\s*[A-Za-z_]\w*"  --type cs   # 구독 지점
Grep "-=\s*[A-Za-z_]\w*"   --type cs   # 해제 지점
# → 같은 파일에서 쌍 확인

# FixedUpdate 외 물리 호출 (RULE-03 위반)
Grep "void Update|void LateUpdate" --type cs -A 30   # Update 내 물리 호출 찾기

# static 초기화 누락 (RULE-01 리스크)
Grep "private\s+static\s+\w+\s+\w+" --type cs --glob "Assets/Scripts/**"
Grep "RuntimeInitializeOnLoadMethod"  --type cs
```

### 중복·데드코드

```
LSP.documentSymbol            # 파일 내 심볼 전수
LSP.findReferences 각 심볼    # 참조 0 이면 죽은 코드 후보
```

---

## 4. 리포트 형식

QA 결과는 **기준별 Pass/Fail + 증거 경로** 로 보고한다. 줄글 감상 금지.

```
기준 1: RULE-01 Domain Reload 미트리거
  - [Pass] grep '\[InitializeOnLoad\]' 매치 0 건
  - [Pass] grep '"autoReferenced": true' 매치 0 건

기준 2: RULE-03 물리 API FixedUpdate 전용
  - [Pass] AddForce 호출처 2건 모두 FixedUpdate 내 확인
           Assets/Scripts/Player/PlayerMotor.cs:87
           Assets/Scripts/Gameplay/Projectile.cs:143

기준 3: 이벤트 구독·해제 쌍
  - [Fail] Assets/Scripts/UI/InventoryPanel.cs:55
           OnItemChanged += 구독은 있으나 OnDisable/OnDestroy 에 대응 -= 없음
  - 제안 수정: OnDisable()에 OnItemChanged -= Handler 추가

요약: 8 기준 중 7 통과, 1 실패. 실패는 수정 후 재검증 필요.
```

Fail 은 항상 **파일:라인 + 제안 수정**까지. 단순 "문제 있음" 금지.

---

## 5. 금지 사항 / 가이드라인

### 강한 금지 (반드시 지킨다)
- **사용자에게 "플레이해 보고 이상하면 말씀해 주세요" 금지.** QA 는 에이전트가 판정한다.
- **기준 없이 QA 시작 금지.** 스펙 없으면 먼저 사용자에게 기준을 묻거나 `/design` 으로 추출.
- **Pass 에 증거 없음 금지.** 각 기준마다 파일:라인을 붙인다.

### 약한 가이드 (상황에 따라 조정)
- **핵심 조항 위주로.** 기획 조항 전수 평가 강요 안 함. **실패 시 아픈 것** (저장 데이터, 재화, 플랫폼 빌드) 을 우선.
- **재검증 주기는 변경 위험도 기준.** 큰 리팩터 뒤엔 재돌. 문구 수정 뒤엔 스킵 OK.

---

## 6. Global App Testing 10 원칙 (Do/Don't)

소스: *Global App Testing, The Ultimate QA Testing Handbook*.

| # | 원칙 | 언제 펼칠 것인가 | 파일 |
|---|---|---|---|
| 1 | 좁은 포커스 + 측정 가능한 기대값 | 테스트 케이스 초안 작성 직전 | [01-narrow-test-cases.md](../../knowledge/qa/01-narrow-test-cases.md) |
| 2 | 테스트 환경은 개발 환경과 분리 | Play Mode 만 확인하고 완료 보고하려 할 때 | [02-separate-environments.md](../../knowledge/qa/02-separate-environments.md) |
| 3 | Test Early, Test Often | 기능 완성 직후·매 `/task-done` | [03-test-early-often.md](../../knowledge/qa/03-test-early-often.md) |
| 4 | Requirements Traceability Matrix | 기획 조항 받은 직후 | [04-traceability-matrix.md](../../knowledge/qa/04-traceability-matrix.md) |
| 5 | 회귀 스크립트 자체부터 의심 | 회귀 실패 보고 받은 직후 | [05-regression-script-skepticism.md](../../knowledge/qa/05-regression-script-skepticism.md) |
| 6 | 탐색적 테스트 사이클 (Learn→Design→Execute) | 자동 회귀로 못 잡은 게임 버그 의심 시 | [06-exploratory-cycle.md](../../knowledge/qa/06-exploratory-cycle.md) |
| 7 | Unit → Integration → System 순서 엄수 | E2E 바로 돌리고 싶어질 때 | [07-testing-pyramid-order.md](../../knowledge/qa/07-testing-pyramid-order.md) |
| 8 | Functional vs Non-Functional 분리 | 성능·로드·신뢰성 평가가 필요할 때 | [08-functional-vs-nonfunctional.md](../../knowledge/qa/08-functional-vs-nonfunctional.md) |
| 9 | 버그 리포트: 재현 + 증거 + severity | QA 결과 보고 작성 시 | [09-bug-report-quality.md](../../knowledge/qa/09-bug-report-quality.md) |
| 10 | Localization: 초기부터 — 후기 = 재앙 | 다국어 출시 계획 있는 순간부터 | [10-localization-early.md](../../knowledge/qa/10-localization-early.md) |
