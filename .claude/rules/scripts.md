---
description: Assets/Scripts/ 하위 C# 파일 작성/수정 시 적용되는 기본 규칙 (프로젝트 고유 컨벤션 확정 전 기본값)
globs: ["Assets/Scripts/**/*.cs"]
---

# 스크립트 작성 규칙

> 신규 프로젝트라 아직 프로젝트 고유 컨벤션(싱글톤 체계, UI 프레임워크, 비동기 라이브러리 선택 등)이
> 정해지지 않았다. 아래는 그 확정 전까지 적용할 범용 기본값이다. 실제 컨벤션이 코드로 정립되면
> 이 파일을 그에 맞게 갱신한다.

## 컴포넌트 캐싱
- `GetComponent<T>()` 결과는 필드에 캐싱한다. `Update()`에서 매 프레임 호출 금지.
- VR/XR 타겟 가능성을 고려해 `Update`/`FixedUpdate`는 90Hz 기준으로 가볍게 유지한다.

## 비동기 처리
- `async` 메서드는 `CancellationToken` 파라미터를 받는 것을 기본으로 한다.
- 프레임 단위 대기(`WaitUntil`, `WaitForSeconds`)는 코루틴을, await 패턴이 필요한 경우는
  UniTask 등 프로젝트에서 채택한 async 라이브러리를 사용한다 — 실제 라이브러리가 정해지면 여기 기록.

## 컴포넌트 초기값은 코드로 설정한다

Inspector/Prefab 설정에 의존하지 않는다. `Awake()`에서 직접 설정해 씬·프리팹 세팅과 무관하게 항상 올바른 상태로 시작한다.

```csharp
// ✅ 권장 — Awake에서 코드로 확정
private void Awake()
{
    // Rigidbody
    var rb         = GetComponent<Rigidbody>();
    rb.isKinematic = true;
    rb.useGravity  = false;

    // Collider
    var col       = GetComponent<SphereCollider>();
    col.isTrigger = true;
    col.radius    = m_colliderRadius;
}
```

**적용 대상:**
- `Rigidbody` — `isKinematic`, `useGravity`, `constraints`
- `Collider` — `isTrigger`, `radius` / `size`
- `[RequireComponent]`로 추가를 강제하고, `Awake()`에서 `GetComponent<T>()`로 값 설정

## GameObject 활성화 상태는 Manager의 Awake에서 관리한다

초기에 비활성화가 필요한 오브젝트(UI, 이펙트 등)는 **해당 컴포넌트 자신의 `Awake()`에서 `SetActive(false)`를 호출하지 않는다.**

자신의 `Awake()`에서 `SetActive(false)`를 호출하면, Manager가 나중에 `SetActive(true)`를 호출할 때 `Awake()`가 그 시점에 처음 실행되며 즉시 다시 꺼버리는 버그가 생긴다.

**소유 Manager의 `Awake()`에서 관리하는 것이 맞다.** `Awake()`는 `Start()`·트리거·물리 이벤트보다 먼저 실행되므로 가장 안전하다.

```csharp
// ✅ 권장 — Manager.Awake()에서 초기 비활성화
private void Awake()
{
    _ = Instance;
    if (m_ui != null) m_ui.gameObject.SetActive(false);
}

// ❌ 금지 — 컴포넌트 자신의 Awake에서 SetActive(false)
// Manager가 SetActive(true) 호출 시 Awake가 그때 실행되어 다시 꺼버림
private void Awake()
{
    gameObject.SetActive(false); // 절대 금지
}
```

## 이벤트 구독 해제

- 이벤트 구독(`+=`) 직전에 동일 핸들러를 `-=`로 먼저 제거한다. 중복 구독을 방지하여
  안정성을 강화한다.

```csharp
// ✅ 권장
someEvent -= OnSomeEvent;
someEvent += OnSomeEvent;

// ❌ 금지
someEvent += OnSomeEvent;  // 재진입/재활성화 시 중복 구독 발생 가능
```

- 구독한 이벤트는 반드시 `OnDestroy()` 또는 `OnDisable()`에서 `-=` 해제한다.
- 싱글톤/정적 이벤트를 구독한 경우 특히 중요 — 씬 전환 후 파괴된 객체 참조로 인한
  NullRef 방지.

## UI 초기화 순서 원칙

Unity 초기화 순서(`Awake → OnEnable → Start`)는 같은 단계 내에서 GameObject 계층·생성
순서에 따라 달라지므로, 다른 오브젝트의 상태를 `OnEnable`·`Start`에서 읽어 초기화하는
패턴은 타이밍이 보장되지 않는다.

### 원칙
- **UI 컴포넌트는 스스로 초기화 시점을 결정하지 않는다.**
  데이터를 가진 쪽(Manager·Controller)이 준비된 시점에 UI에 직접 값을 밀어준다(Push).
- UI는 전달받은 값을 표시하는 역할만 하며, `OnEnable`·`Start`에서 외부 싱글톤이나
  다른 컴포넌트를 조회해 초기값을 세팅하지 않는다.

### 패턴

**✅ 권장 — Manager가 초기화 시점을 통제**
```csharp
// Manager.Start()에서 UI에 값을 직접 주입
void Start()
{
    foreach (var section in m_sections)
        section.SetupUI(m_config.cost);  // Manager가 준비된 후 Push
}

// UI는 받아서 표시만
public void SetupUI(int cost) { m_label.text = $"{cost}"; }
```

**✅ 불가피하게 UI에서 조회해야 할 때 — 코루틴으로 1프레임 대기**
```csharp
private void OnEnable()
{
    StartCoroutine(InitAfterFrame());
}
private IEnumerator InitAfterFrame()
{
    yield return null;  // 모든 Awake/Start가 끝난 뒤 실행 보장
    RefreshDisplay();
}
```

**❌ 금지 — OnEnable/Start에서 외부 싱글톤 직접 조회**
```csharp
private void OnEnable() { SomeManager.Instance.GetValue(); }  // Instance == null 가능
private void Start()    { SomeManager.Instance.GetValue(); }  // Start 순서 미보장
```

## 네임스페이스 참조

파일을 수정하거나 새 타입을 참조하는 코드를 작성하기 전에, **반드시 해당 파일 상단의 `using` 목록을 확인**하고 누락된 네임스페이스를 함께 추가한다.

## 주석
- 한글 주석 허용.
