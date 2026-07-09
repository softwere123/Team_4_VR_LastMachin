# 2. 테스트 환경은 개발 환경과 분리

> Customize and execute test cases in an environment that is different from the one used for development.
> — Global App Testing, Ch 1

## Do
- Editor Play Mode 외에도 **Development Build / Release Build** 에서 재현 확인.
- 테스트 전용 씬(예: `Assets/Scenes/QA/*.unity`) 에서 최소 초기 조건만 세팅. 개발 중인 메인 씬에 테스트 코드·상태를 섞지 않는다.
- 다른 해상도 · 타겟 framerate · input 조합에서 스모크 — 모바일이면 저사양 + 고사양 2종.

## Don't
- "Editor Play 에서 잘 된다" = "빌드에서도 될 것" 으로 단정 금지. IL2CPP 와 Mono 는 reflection·generic·nullable 런타임 동작이 다르다.
- 테스트용으로 만든 특수 상태를 개발 씬에 남기지 않는다 — 다음 세션이 그 흔적을 정상 상태로 착각.
- 로컬 머신에서만 통과 = 통과 가 아니다. 빌드 머신·CI 환경에서도 재현돼야.

## Unity / Agent
- 정적 분석은 `Grep` / `LSP` 로 에디터와 무관하게 수행. 코드 검증 자체는 환경 불문.
- **빌드 스모크**는 주요 변경 후 최소 1회 — 실제 빌드 파이프라인이 정해지면 여기 커맨드를 채운다. Editor Play 에서 통과해도 IL2CPP 빌드에서 stripping·AOT 버그 발생 가능.
- 플랫폼 전용 API·SDK 분기, 실기기 전용 프레임/햅틱 항목이 생기면 "Editor 통과 = 실기기 통과" 로 단정 금지.
