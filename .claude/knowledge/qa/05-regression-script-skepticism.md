# 5. 회귀 테스트 — 스크립트 자체부터 의심하라

> Regression test scripts only test what you tell them to - automated or not. (...) Any time there is a change in your product or software, checking regression test scripts for accuracy is required. Otherwise, determining whether a failure is because of a bug in your code change or a poorly written test script will be impossible.
> — Global App Testing, Ch 2

## Do
- 회귀 실패가 뜨면 **먼저 스크립트를 의심한다**. 제품 코드를 고치기 전에 테스트 스크립트의 전제·기대값이 **현재 기획** 과 일치하는지 재검증.
- 기획 변경 PR 과 회귀 스크립트 갱신 PR 을 **같은 커밋** 에. 한 쪽만 바뀐 상태는 false signal 원천.
- 회귀 스위트는 **모든 변형·부작용·의존성** 커버. 커버리지 표를 PR 에 첨부.
- 회귀 스크립트도 코드다 — 리뷰·리팩터 대상.

## Don't
- 회귀 실패 = 제품 버그 로 즉단 금지. false positive 가능성이 항상 있다.
- 오래된 회귀 스크립트를 "한 번도 안 깨졌으니 안전" 으로 단정 금지 — 테스트 스크립트도 썩는다 (test rot): 기획이 바뀌었는데 스크립트는 옛 전제 그대로 통과 중일 수 있다.
- "자동 회귀가 통과했으니 문제없음" 만으로 릴리즈하지 않는다 — 스크립트가 커버하지 않는 경로는 영원히 안 보인다 (§6 탐색적 테스트 필요).

## Unity / Agent
- 회귀 기준은 **`.claude/domain/gdd/dave-collabo-gdd.md` 기획 조항**이 진리. 실패 시 조항과 구현 중 어느 쪽이 맞는지 아키텍트에게 판정 요청 후 한 쪽만 고친다.
- 정적 회귀 스캔: `Grep` 으로 RULE 위반·이벤트 누수·Dave 경로 규칙을 매 `/qa` 호출마다 동일하게 돌린다 — 이게 이 프로젝트의 기본 회귀 스위트.
- 기획 변경 시 관련 Grep 패턴도 같이 갱신 — 패턴이 옛 기획 기준이면 false pass 발생 (§5 핵심).
