---
name: task-start
description: 작업 시작 전 범위와 제약을 확정하는 브리핑 루틴. 먼저 `.claude/INDEX.md`를 스캔해 필요한 지침만 선별 로드하여 토큰과 시간을 아낍니다.
---

# Skill: /task-start (Task Briefing)

작업을 시작하기 전에 반드시 이 브리핑을 수행한다. **파일을 열기 전에 범위를 먼저 확정한다.**

---

### STEP 0 — 인덱스 기반 선별 로드 (토큰 절약)

[`.claude/INDEX.md`](../INDEX.md)를 먼저 읽는다. 이 파일만 로드하면 몇 백 토큰이다.

1. 작업 프롬프트에서 핵심 명사·동사·심볼을 뽑는다.
   예: "인벤토리 UI 버그" → `Inventory`, `UI`, `InventoryPanel`
2. 인덱스 각 항목의 `keywords`와 매칭한다.
3. **매칭된 파일만** `Read`/`Grep`으로 연다. 매칭이 없으면 읽지 않는다.
4. `RULES.md`는 항상 스캔. `CLAUDE.md`와 `rules/scripts.md`는 `.cs` 수정 시 항상 로드.

선별 로드 결과를 한 줄로 선언:

```
[STEP 0] 매칭된 지침: RULES.md, rules/scripts.md
         건너뜀: knowledge/csharp-dotnet.md, knowledge/unity-scripting-gotchas.md
```

이 선언은 이후 STEP에서 실제로 읽은 범위를 검증할 근거가 된다.

---

### STEP 1 — RULES.md 재확인

방금 스캔한 `RULES.md`에서 **이번 작업과 관련된 규칙**을 골라 명시한다.

이 프로젝트의 불변 제약 중 관련 항목:
- **RULE-01**: Domain Reload 트리거 금지 (`[InitializeOnLoad]`, `autoReferenced: true`)
- **RULE-02**: Unity 에셋 파일 직접 편집 금지 (`.meta`/`.prefab`/`.unity`/`.asset`)
- **RULE-03**: 물리 API는 `FixedUpdate`에서만
- **RULE-04**: `ProjectSettings/`는 Claude가 직접 수정하지 않음

위반 위험이 있는 규칙은 대응책과 함께 선언한다.

---

### STEP 2 — 대상 파악

`Grep`으로 실제 파일 위치를 확인한다. **추측하지 않는다.**

```
Grep "class TargetClass" --type cs --glob "Assets/Scripts/**" -l
Grep "TargetMethod"      --type cs --glob "Assets/Scripts/**" -l
```

관련 파일·심볼을 모두 나열한다.

---

### STEP 3 — 작업 범위 선언

- 수정할 파일과 심볼을 명시한다.
- 수정하지 않는 파일도 명시한다 (호출부 확인만 등).
- 범위 밖 작업이 발견되면 **즉시 멈추고 아키텍트에게 보고**한다.

---

### STEP 4 — 작업 시작 선언

모든 사전 조건이 확인됐음을 선언하고 작업 내용을 한 줄로 요약한다.

---

## 출력 형식

```
[STEP 0] 인덱스 선별 로드
매칭된 지침: {파일 목록}
건너뜀: {파일 목록}

[STEP 1] 관련 규칙
{RULE-NN: 이번 작업과의 연관, 위반 위험 여부}

[STEP 2] 대상 파악
{Grep 결과와 파일 위치}

[STEP 3] 작업 범위
수정할 파일:
- {파일} → {심볼}
수정하지 않는 파일:
- {파일} (사유)

[STEP 4] 작업 시작: {한 줄 요약}
```

## 금지 사항

- **지침 전체를 매번 통째로 로드하지 않는다.** 인덱스가 존재하는 이유다.
- **매칭 안 된 파일을 "혹시 몰라서" 읽지 않는다.** 작업 진행 중 실제로 필요해지면 그때 연다.
- **파일 경로를 추측하지 않는다.** Grep으로 확인하거나 `{TODO: verify}` 마크한다.
