---
name: make-assets
description: Unity 에셋(UI Prefab, ParticleSystem, Material, ScriptableObject 등)을 Editor 스크립트로 생성한다. .prefab/.asset 직접 편집(RULE-02) 없이 Unity가 직접 만들게 한다.
---

# Skill: /make-assets

Unity Editor 스크립트를 작성해 에셋을 생성한다.  
**절대 `.prefab` / `.asset` / `.unity` / `.meta` 파일을 직접 편집하지 않는다 (RULE-02).**  
코드로 표현 가능한 모든 에셋 — UI Prefab, ParticleSystem, Material, ScriptableObject — 이 대상이다.

---

## 지원 에셋 유형

| 유형 | Unity API | 비고 |
|---|---|---|
| UI Prefab | `PrefabUtility.SaveAsPrefabAsset` | 프로젝트 고유 UI 베이스 클래스가 정해지면 그것을 상속하도록 갱신 |
| ParticleSystem | `go.AddComponent<ParticleSystem>()` + module 설정 | |
| Material | `new Material(shader)` + `AssetDatabase.CreateAsset` | |
| ScriptableObject | `ScriptableObject.CreateInstance<T>()` + `AssetDatabase.CreateAsset` | |
| 범용 Prefab | `PrefabUtility.SaveAsPrefabAsset` | 어떤 컴포넌트든 |

**불가능한 것 (직접 생성 X):** Texture, Sprite, Audio, AnimationClip(복잡한 커브), .unity 씬, 3D 모델

---

## 절차

### STEP 1 — 요구 파악

사용자 입력에서 다음을 추출한다. 누락 시 질문한다.

- **에셋 유형**: UI / Particle / Material / ScriptableObject / 기타
- **이름**: 생성할 GameObject 또는 에셋 이름
- **저장 경로**: `Assets/Scripts/...` 또는 `Assets/Prefabs/...` — 프로젝트 폴더 구조가 정해지지 않았으면 사용자에게 확인
- **컴포넌트 목록 및 초기값**: 있다면 명세
- **UI면 추가로**: 버튼 수, 텍스트 라벨, 레이아웃 방향

### STEP 2 — 기존 패턴 확인

생성 전 `Grep`으로 비슷한 기존 파일을 찾아 컨벤션을 맞춘다.

```
# UI 컴포넌트 예시
Grep "MonoBehaviour" Assets/Scripts --type cs -l (head 5)
```

찾은 파일을 `Read`로 열어 네임스페이스, using, 필드 명명 컨벤션을 확인한다. 프로젝트 고유 UI 베이스 클래스가 아직 없으므로 기본값은 표준 UGUI(`UnityEngine.UI.Button`, `UnityEngine.UI.Text` 또는 TextMeshPro)다 — 프로젝트가 자체 UI 프레임워크를 채택하면 이 스킬과 `rules/scripts.md`를 그에 맞게 갱신한다.

### STEP 3 — 파일 작성

두 파일을 작성한다:

#### 3a. 컴포넌트 스크립트 (런타임)

경로: `Assets/Scripts/{적절한 경로}/{Name}.cs`

```csharp
// 네임스페이스는 기존 파일 패턴에서 확인
using UnityEngine;
using UnityEngine.UI;

public class {Name} : MonoBehaviour
{
    // SerializeField — Inspector 연결용
    // 버튼 이벤트는 OnEnable에서 -= 후 +=, OnDisable에서 -=
}
```

**UI 컴포넌트 기본 규칙 (프로젝트 고유 컨벤션 확정 전):**
- 버튼은 기본 `UnityEngine.UI.Button` 사용 (프로젝트가 커스텀 버튼 컴포넌트를 채택하면 그것으로 대체)
- 이벤트 구독: `OnEnable`에서 `-=` 후 `+=`, `OnDisable`에서 `-=`
- 초기 비활성화가 필요하면 Manager의 `Awake()`에서 처리 (컴포넌트 자신의 Awake에서 `SetActive(false)` 금지)

#### 3b. Editor 스크립트 (에셋 생성기)

경로: `Assets/Scripts/Editor/{Name}Factory.cs`

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class {Name}Factory
{
    private const string k_path = "Assets/{경로}/{Name}.prefab";
    private static bool s_force;

    // 이미 존재하면 건너뜀
    [MenuItem("Tools/Make Assets/{Name}")]
    public static void Create() => CreateInternal(force: false);

    // 이미 존재해도 덮어씀
    [MenuItem("Tools/Make Assets/{Name} (Force Recreate)")]
    public static void ForceRecreate() => CreateInternal(force: true);

    private static void CreateInternal(bool force)
    {
        if (!force && AssetDatabase.LoadAssetAtPath<GameObject>(k_path) != null)
        {
            Debug.LogWarning($"[MakeAssets] 이미 존재합니다. Force Recreate 메뉴를 사용하세요: {k_path}");
            return;
        }

        // 1. 루트 GameObject 생성
        var root = new GameObject("{Name}");

        // 2. 컴포넌트 추가
        var comp = root.AddComponent<{ComponentType}>();

        // 3. 자식 오브젝트 구성 (아래 "Prefab 비주얼 원칙" 참조)
        // var child = new GameObject("...");
        // child.transform.SetParent(root.transform, false);
        // child.AddComponent<SomeScript>();
        // AddVisual(child, PrimitiveType.Cube, new Vector3(0.1f, 0.1f, 0.1f));

        // 4. SerializedObject로 필드 연결
        // var so = new SerializedObject(comp);
        // so.FindProperty("m_field").objectReferenceValue = someRef;
        // so.ApplyModifiedPropertiesWithoutUndo();

        // 5. 저장
        PrefabUtility.SaveAsPrefabAsset(root, k_path);
        Object.DestroyImmediate(root);
        AssetDatabase.Refresh();
        Debug.Log($"[MakeAssets] {Name} 생성 완료: {k_path}");
    }

    /// <summary>
    /// 자식 Visual 오브젝트를 붙인다.
    /// Collider는 부모의 것만 사용하므로 Primitive 자동 추가 Collider를 제거한다.
    /// </summary>
    private static GameObject AddVisual(GameObject parent, PrimitiveType shape, Vector3 scale,
                                         Vector3 localPos = default, Quaternion rotation = default)
    {
        var visual = GameObject.CreatePrimitive(shape);
        visual.name = "Visual";
        visual.transform.SetParent(parent.transform, false);
        visual.transform.localPosition = localPos;
        visual.transform.localRotation = rotation == default ? Quaternion.identity : rotation;
        visual.transform.localScale    = scale;
        Object.DestroyImmediate(visual.GetComponent<Collider>());
        return visual;
    }
}
#endif
```

**Material 생성 패턴:**
```csharp
var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
mat.color = Color.white;
AssetDatabase.CreateAsset(mat, "Assets/.../MyMat.mat");
```

**ParticleSystem 패턴:**
```csharp
var ps = go.AddComponent<ParticleSystem>();
var main = ps.main;
main.startLifetime = 1f;
main.startSpeed = 3f;
var emission = ps.emission;
emission.rateOverTime = 20f;
```

**ScriptableObject 패턴:**
```csharp
var so = ScriptableObject.CreateInstance<{DataType}>();
so.someField = value;
AssetDatabase.CreateAsset(so, "Assets/.../data.asset");
```

### STEP 4 — 사용자 안내

파일 작성 완료 후 반드시 아래 안내를 출력한다:

```
✅ 파일 생성 완료:
  - {컴포넌트 스크립트 경로}
  - {Editor 스크립트 경로}

▶ Unity에서 실행:
  Unity 상단 메뉴 → Tools > Make Assets > {Name}

  실행 후 {저장경로}/{Name}.prefab 이 생성됩니다.
  Console에 "[MakeAssets] ... 생성 완료" 로그가 뜨면 성공.

⚠️ 컴파일 오류가 뜨면 Console 메시지를 붙여넣어 주세요.
```

---

## 체크리스트

작성 전:
- [ ] `Grep`으로 기존 유사 파일 확인 (네임스페이스, using 패턴)
- [ ] 저장 경로가 프로젝트 폴더 구조와 맞는지 확인 (불확실하면 사용자에게 확인)
- [ ] RULE-02: `.prefab`/`.asset` 직접 편집 없음 확인

작성 후:
- [ ] 이벤트 구독 해제 (`OnDisable`에서 `-=`)
- [ ] Editor 스크립트에 `#if UNITY_EDITOR` 가드
- [ ] `MenuItem` 경로가 `"Tools/Make Assets/{Name}"` 형식
- [ ] 사용자에게 Unity 실행 방법 안내 출력
- [ ] Prefab 생성 시 Visual 자식 `localScale = Vector3.one`, 크기는 부모 스케일로

---

## Layout Group 수동 검사 규칙

`HorizontalLayoutGroup`, `VerticalLayoutGroup`, `ContentSizeFitter`는 배치 편의용으로 사용하되, **런타임에 남겨두면 매 프레임 레이아웃 재계산으로 성능을 갉아먹는다.**

**원칙:**
- 배치 완료 후 `HorizontalLayoutGroup` / `VerticalLayoutGroup` / `ContentSizeFitter`는 **삭제하거나 비활성화**한다.
- 예외: 텍스트 길이에 따라 **실시간으로 크기가 동적으로 변해야 하는** UI는 유지 허용.

**Factory 스크립트 작성 시:**
- Layout Group을 배치 보조용으로 추가했다면, `PrefabUtility.SaveAsPrefabAsset` 호출 직전에 반드시 제거:
```csharp
// 배치 완료 후 Layout Group 제거 — 런타임 성능
Object.DestroyImmediate(go.GetComponent<HorizontalLayoutGroup>());
Object.DestroyImmediate(go.GetComponent<VerticalLayoutGroup>());
Object.DestroyImmediate(go.GetComponent<ContentSizeFitter>());
```
- 동적 크기 변화가 필요한 경우만 남기고, 이유를 주석으로 명시.

---

## 금지 사항

- `.prefab` / `.asset` / `.unity` / `.meta` 파일 직접 생성 또는 편집 (RULE-02)
- 컴포넌트 자신의 `Awake()`에서 `SetActive(false)` 호출
- Editor 스크립트를 `Editor/` 폴더 밖에 배치 (빌드 에러 발생)
- Visual 오브젝트 `localScale`을 `Vector3.one` 이외로 생성 — 크기는 부모 스케일로
- Image/Text에 불필요하게 `raycastTarget = true` 설정 — 배경·버튼 이외는 false
- Text에 `supportRichText = true` 남기기 — 불필요한 파싱 비용, 기본 false