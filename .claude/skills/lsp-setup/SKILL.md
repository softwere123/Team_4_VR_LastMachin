---
name: lsp-setup
description: Claude Code 의 LSP 툴이 Unity 프로젝트의 `.cs` 를 인식하도록 OmniSharp 를 설치·연결하는 자동 설정 스킬. `No LSP server available for file type: .cs` 응답을 받았거나, 처음 세팅할 때 에이전트가 자동 호출한다.
---

# Skill: /lsp-setup (LSP 자동 세팅)

프로그래밍을 몰라도 되게 설계됨. 아래 절차는 **에이전트가 실행**, 사용자는 설치 권한 프롬프트 1회 + Claude Code 재시작 1회만 눌러 주면 된다.

---

## 1. 왜 이게 필요한가

`/debug` 와 `/qa` 는 LSP 로 소스 구조 (심볼·참조·호출자) 를 읽는다. LSP 가 연결돼 있지 않으면 Grep 폴백으로 돌아가지만 정확도·속도가 떨어진다. Unity C# (.cs) 은 자동 연결 안 되므로 1회 세팅 필요.

> **핑거프린트:** `LSP.documentSymbol` 호출 시 `No LSP server available for file type: .cs` 응답 → 세팅 안 됨.

---

## 2. 에이전트 자동 실행 순서

아래 순서를 **그대로** 돌린다. 중간 실패 시 다음 스텝으로 넘어가지 말고 사용자에게 한 줄 보고.

### Step 1 — 현재 상태 점검

```bash
# 1-a) 이미 OmniSharp 설치됐는가
which omnisharp || echo "MISSING:omnisharp"

# 1-b) settings.json 에 LSP 활성화 플래그 있는가
grep -q '"ENABLE_LSP_TOOL"' .claude/settings.json 2>/dev/null || echo "MISSING:flag"
```

### Step 2 — OmniSharp 설치

`MISSING:omnisharp` 나왔으면:

- **macOS**: `brew install omnisharp`
- **Windows**: [공식 릴리즈](https://github.com/OmniSharp/omnisharp-roslyn/releases) 에서 바이너리 다운로드 후 PATH 에 추가 — 사용자에게 링크 제시 후 위임.
  - 권장 경로: `C:\tools\omnisharp\` 또는 `%USERPROFILE%\AppData\Local\omnisharp\`
  - PATH 추가 후 PowerShell 재시작 필요

### Step 3 — `.claude/settings.json` 에 LSP 활성화

파일이 없으면 새로 생성, 있으면 기존 키와 병합. `settings.local.json` 이 아니라 **`settings.json`** 에 쓴다 (프로젝트 공유 설정).

```json
{
  "env": {
    "ENABLE_LSP_TOOL": "1"
  },
  "enabledPlugins": {
    "omnisharp-lsp@claude-plugins-official": true
  }
}
```

> 플러그인 이름이 다를 수 있음. Step 4 에서 설치 실패 시 `claude plugin search omnisharp` 또는 `claude plugin search csharp` 로 대체 이름 확인.

### Step 4 — 플러그인 설치

```bash
claude plugin install omnisharp-lsp@claude-plugins-official
```

실패하면:
```bash
claude plugin search omnisharp    # 대체 이름 탐색
claude plugin search csharp
```

찾은 이름으로 재시도, 그에 맞춰 `settings.json` 의 `enabledPlugins` 키도 수정.

### Step 5 — Unity `.sln` 생성 확인

OmniSharp 는 `.sln` 또는 `.csproj` 가 있어야 프로젝트 인덱싱. Unity 는 Editor 가 열리면 자동 생성.

```bash
# Windows PowerShell
Get-ChildItem *.sln -ErrorAction SilentlyContinue | Select-Object Name
```

`.sln` 없으면: Unity Hub 에서 프로젝트 1회 열기 → `{프로젝트명}.sln` 자동 생성됨.

### Step 6 — Claude Code 재시작

설정 반영 위해 **사용자가 Claude Code 재시작**. 에이전트는 이 시점에 작업 일시 중단하고 "LSP 활성화를 위해 Claude Code 를 재시작해 주세요. 재시작 후 이어서 진행합니다." 로 안내.

### Step 7 — 검증

재시작 후 첫 작업에서:
```
LSP.documentSymbol
  filePath=Assets/Scripts/Test.cs
  line=1  character=1
```
- 심볼 목록 반환 → ✅ 세팅 성공
- `No LSP server available for file type: .cs` 여전 → Step 1 로 복귀, 어느 스텝이 안 먹혔는지 재점검.

---

## 3. 실패 시 대응 — Grep 폴백

세팅이 실패하거나 사용자가 재시작을 미루면 LSP 대신 **Grep 을 정적 평가 1순위로** 사용. `debug.md` §2 와 `qa.md` §3 의 Grep 스니펫이 모든 LSP 경로를 커버한다. 이 때 에이전트는 "LSP 미연결 상태 — Grep 폴백으로 진행" 을 세션 시작 시 1회 고지.

---

## 4. 금지 사항

- **사용자에게 개별 명령을 복붙 시키지 않는다.** 에이전트가 Bash/PowerShell 툴로 직접 돌린다. 사용자는 권한 프롬프트만 본다.
- **`settings.local.json` 에 쓰지 않는다** — 그 파일은 개인 설정 (.gitignore). LSP 는 프로젝트 공유 설정이 맞으므로 `settings.json`.
- **`.sln` 없다고 수동으로 만들지 않는다.** Unity 에디터가 생성하게 한다. 수동 생성은 어셈블리 참조가 어긋난다.
