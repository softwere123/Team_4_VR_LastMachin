# RULES.md — 불변 제약

> 이 파일에는 위반 시 시스템이 실제로 망가지는 규칙만 담는다.
> "코드가 지저분해진다"는 망가진 것이 아니다.
> "에디터가 3분 멈추거나, GUID 참조가 깨지거나, 빌드가 실패하거나, 
>  유저 저장본이 로드 불가능해지는 것"이 망가진 것이다.

---

### RULE-01 Domain Reload를 트리거하지 않는다

**위반 시:** 에디터가 3분 이상 멈추고 모든 static 상태가 초기화된다.

금지 행위:
- `[InitializeOnLoad]` 어트리뷰트 신규 추가
- `.asmdef` 파일의 `autoReferenced` 값을 `true`로 변경
- `.asmdef` 파일 구조 임의 변경 (의존성 순환 유발)

위반 여부 확인:

```bash
grep -rE '\[InitializeOnLoad\]' Assets/ --include="*.cs"
grep -r  '"autoReferenced": true' Assets/
```

---

### RULE-02 Unity 자산 파일을 직접 편집하거나 생성하지 않는다

**위반 시:** GUID 충돌, 직렬화 깨짐, 씬·프리팹 참조 손실. 
         복구하려면 Unity Editor에서 수동 재작업 필요.

Claude Code가 직접 다루지 않는 파일:
- `.meta` — Unity가 자동 관리. GUID 수동 편집 금지.
- `.prefab`, `.unity`(씬), `.asset`, `.mat`, `.anim`, `.controller` — 
  Unity Editor에서만 수정·생성. Claude가 텍스트로 직접 작성하지 않는다.
- `.fbx`, `.png`, `.wav` 등 바이너리 에셋 — 당연히 텍스트 편집 불가.

프리팹/머티리얼/씬이 필요한 작업은 **사용자에게 Unity Editor에서 
직접 만들도록 요청**하고, Claude는 그 위에 붙일 스크립트만 작성한다.

---

### RULE-03 물리 연산은 FixedUpdate에서만 수행한다

**위반 시:** 프레임 레이트에 따라 물리 시뮬레이션이 불안정해진다.
         VR 90Hz 환경에서 점프/이동이 기기마다 달라진다.

- `Rigidbody.AddForce`, `MovePosition`, `velocity` 직접 조작은 
  `FixedUpdate()`에서만 호출한다.
- `Update()`에서 물리 API를 호출하지 않는다.

---

### RULE-04 ProjectSettings는 Claude가 직접 수정하지 않는다

**위반 시:** 바이너리 파일(GlobalGameManagers 등) 충돌이 발생하여
         수동 머지가 불가능하다. 프로젝트 전체 설정이 꼬일 수 있다.

- Claude Code는 `ProjectSettings/` 하위 파일을 직접 수정하지 않는다.
- 설정 변경이 필요하면 Unity Editor에서 수동으로 변경할 것을 제안한다.
- 변경 전 반드시 현재 값과 제안 값을 보고하고 확인을 받는다.

---

> 저장 데이터 스키마, 플랫폼 심볼 분기 등 프로젝트 고유 시스템이 생기면
> 그 시점에 새 RULE-NN을 여기에 추가한다. 지금은 신규 프로젝트라 해당 없음.