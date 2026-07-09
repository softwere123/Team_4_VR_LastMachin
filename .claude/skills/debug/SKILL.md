---
name: debug
description: 버그 리포트·예외·오작동을 만났을 때 에이전트가 직접 소스코드를 읽고 추적·재현해 원인을 짚는 툴 사용 지침. 사용자에게 "브레이크포인트 걸어 주세요 / 플레이해 보세요"를 시키지 않는다.
---

# Skill: /debug (Agent-driven Debugging)

**전제:** 디버깅의 책임은 에이전트에게 있다. 사용자에게 Unity Editor 에서 수동으로 브레이크포인트를 걸거나, Play 해서 눈으로 확인하라고 요청하지 않는다. 에이전트가 갖고 있는 툴(소스 정적 평가)로 직접 추적한다.

이 문서는 **어떤 툴을 어느 순간 쓸지**만 다룬다. 작업 착수·종료 절차는 `/task-start`, `/task-done` 참고.

---

## 1. 툴 매트릭스 — 증상 종류별 1순위 툴

| 증상 유형 | 1순위 (소스 평가) | 2순위 | 금지 |
|---|---|---|---|
| `NullReferenceException` / 미할당 참조 | `LSP.findReferences` + `LSP.goToDefinition` 으로 대입 지점 전수 조회 | Grep 으로 대입·초기화 지점 수동 추적 | 사용자에게 "인스펙터 열어 확인해 달라" |
| 의도치 않은 값 전이 / 잘못된 상태 | `LSP.incomingCalls` / `outgoingCalls` 로 호출 그래프 추적 | Grep 으로 해당 필드 write 지점 전수 조회 | 사용자에게 "Play 해서 값 보세요" |
| 이벤트 중복 호출 / 구독 누수 | `Grep` 으로 `+=`·`-=` 쌍 전수 매칭 + `LSP.findReferences` | `rules/scripts.md` 구독·해제 패턴 재확인 | "리스너 한 번 지워 보세요" |
| 물리 버그 (점프 2회 / 미끄러짐) | `Grep` 으로 `AddForce`/`MovePosition`/`velocity` 호출 위치 + RULE-03 확인 | `LSP.incomingCalls` 로 호출자 맥락 확인 | "패드로 여러 번 눌러 보세요" |
| 도메인 리로드 / static 상태 오염 | `Grep` 으로 `static` 필드 + `[RuntimeInitializeOnLoadMethod]` 쌍 확인 (RULE-01) | Read 로 해당 클래스 전체 흐름 추적 | "Play 두 번 눌러 보세요" |
| 컴파일 에러 | `Read` + `LSP.hover` 로 타입 확인 | Grep 으로 네임스페이스·using 누락 확인 | "콘솔 오류 복붙해 주세요" |

---

## 2. 정적 평가 — LSP 중심

소스를 직접 평가해 원인을 짚는다. **추측 금지**, 호출·정의·참조는 LSP 가 진리다.

### 2-0. LSP 가용성 사전 점검 (필수)

세션 시작 시 LSP 가 실제로 `.cs` 를 처리하는지 먼저 1회 검증.

```
# 아무 .cs 파일로 probe (예: Assets/Scripts/Test.cs)
LSP.documentSymbol filePath=Assets/Scripts/Test.cs line=1 character=1
```

- **연결됨** → 아래 LSP 조합을 1순위로.
- **연결 안 됨** → [`/lsp-setup`](../lsp-setup/SKILL.md) 스킬 자동 호출해 세팅 시도. 세팅 중엔 Grep 폴백으로 진행.
- 모든 LSP 조합은 Grep 대체 쿼리가 있다 (본 섹션 하단 "Grep 으로 빠르게 훑을 때").

### 자주 쓰는 LSP 조합

```
증상 라인 발견
  → LSP.hover        (그 심볼의 타입·선언 확인)
  → LSP.goToDefinition (정의처 본체 읽기)
  → LSP.findReferences (모든 사용처 = 범인 후보 전수 조회)
  → LSP.incomingCalls  (함수가 버그라면 누가 부르는지)
  → LSP.outgoingCalls  (함수가 의심이라면 내부에서 뭘 부르는지)
```

### 사용 예

- **NRE 위치에서 역추적**: 스택트레이스의 파일·라인 확인 → `LSP.hover` 로 대상 심볼 → `LSP.findReferences` 로 대입처 전수 → 초기화 누락 후보 짚기
- **호출 순서 의심**: `LSP.prepareCallHierarchy` → `incomingCalls` 재귀로 진입 경로 트리
- **인터페이스 구현 확인**: `LSP.goToImplementation` 으로 실제 구현체 식별 (다형성 버그 고정)

### Grep 으로 빠르게 훑을 때

LSP 가 무거울 만한 광범위 패턴은 Grep. **파일 목록만** 뽑아 LSP 로 상세 점검.

```
Grep "static\s+\w+\s+\w+\s*=" --type cs --glob "Assets/Scripts/**"   # static 필드 전수
Grep "\+=\s*.*(?:\.)" --type cs                                        # 이벤트 구독
Grep "AddForce|MovePosition|\.velocity\s*=|\.linearVelocity\s*=" --type cs  # 물리 호출 지점
```

---

## 3. 금지 사항

- **사용자에게 Play·브레이크포인트·인스펙터 확인을 시키지 않는다.** "화면에서 어떻게 보이나요?" 금지. 에이전트가 직접 소스를 읽고 보고한다.
- **추측으로 원인을 단정하지 않는다.** LSP·Grep 으로 전수 확인한 증거가 있어야 원인 보고.
- **정적 평가 없이 결론으로 직행하지 않는다.** LSP 로 호출 그래프부터 좁히고, 좁혀진 범위에서만 판단한다.
- **한 번의 코드 읽기로 단정하지 않는다.** 간헐 버그는 최소 케이스 재현 가능성 + 복수 호출 경로 일치해야 확정.
- **Unity 로그 전문을 대화창에 붙이지 않는다.** `error CS`·stack trace 요약만.

---

## 4. Adragna 디버깅 원칙 (Do/Don't)

소스: P. Adragna, *Software debugging techniques*, CERN School of Computing 2007, pp. 71–86.

방법론 원칙 10개. **각 규칙은 별도 파일** — 작업 중 해당 상황을 만나면 펼쳐 읽는다. 여기선 인덱스만.

| # | 원칙 | 언제 펼칠 것인가 | 파일 |
|---|---|---|---|
| 1 | 가정을 먼저 의심하라 | 디버깅 착수 직후 | [01-doubt-assumptions.md](../../knowledge/debugging/01-doubt-assumptions.md) |
| 2 | 버그를 먼저 분류하라 | 증상을 처음 받았을 때 | [02-classify-first.md](../../knowledge/debugging/02-classify-first.md) |
| 3 | 증상과 원인을 혼동하지 마라 | NRE·예외 라인을 원인으로 단정하고 싶을 때 | [03-symptom-vs-cause.md](../../knowledge/debugging/03-symptom-vs-cause.md) |
| 4 | 완전히 이해한 뒤 고쳐라 | 수정 PR 올리기 직전 | [04-understand-before-fix.md](../../knowledge/debugging/04-understand-before-fix.md) |
| 5 | 버그 저널을 남겨라 | `/task-done` STEP 5 에서 | [05-bug-journal.md](../../knowledge/debugging/05-bug-journal.md) |
| 6 | 정적 분석·경고를 최대한 살려라 | 경고 억제가 유혹될 때 | [06-static-analysis.md](../../knowledge/debugging/06-static-analysis.md) |
| 7 | print/Debug.Log 남용 금지 | `Debug.Log` 박으려는 순간 | [07-no-print-debug-abuse.md](../../knowledge/debugging/07-no-print-debug-abuse.md) |
| 8 | 어서션으로 가정을 코드에 박아라 | Awake/진입점·상태 전이 직전 | [08-assertions.md](../../knowledge/debugging/08-assertions.md) |
| 9 | 막히면 관점을 바꿔라 (ACI / 다시 읽기) | 30분 이상 진전 없을 때 | [09-reframe-when-stuck.md](../../knowledge/debugging/09-reframe-when-stuck.md) |
| 10 | 이진 탐색으로 범위를 쪼개라 | 의심 범위 100줄 이상일 때 | [10-binary-split.md](../../knowledge/debugging/10-binary-split.md) |

> **원저 요지 (Abstract / Introduction):** 디버깅은 "옳다고 믿었던 전제가 실은 틀렸다" 를 확인하는 과정이다. 은총알은 없다. 경험·창의성과 함께 **규율 잡힌 도구 사용** 이 전부다. 이 10 원칙은 그 규율의 최소 집합이다.