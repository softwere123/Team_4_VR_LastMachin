 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* 참고: 애니메이션은 캐릭터와 캡슐 모두에서 애니메이터 null 체크를 통해 컨트롤러에서 호출됩니다
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController_HG : MonoBehaviour
    {
        [Header("플레이어 설정")]
        [Tooltip("캐릭터의 이동 속도 (m/s)")]
        public float MoveSpeed = 2.0f;

        [Tooltip("캐릭터의 달리기 속도 (m/s)")]
        public float SprintSpeed = 5.335f;

        [Tooltip("이동 방향으로 회전하는 속도")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("가속 및 감속 속도")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("플레이어가 점프할 수 있는 높이")]
        public float JumpHeight = 1.2f;

        [Tooltip("캐릭터가 사용하는 중력 값 (기본 엔진 중력: -9.81f)")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("다시 점프할 수 있기까지 걸리는 시간 (0으로 설정하면 바로 점프 가능)")]
        public float JumpTimeout = 0.50f;

        [Tooltip("낙하 상태로 전환되기까지 걸리는 시간 (계단 내려갈 때 유용)")]
        public float FallTimeout = 0.15f;

        [Header("플레이어 접지 상태")]
        [Tooltip("캐릭터가 접지되어 있는지 여부 (CharacterController 기본 기능과는 다름)")]
        public bool Grounded = true;

        [Tooltip("울퉁불퉁한 지형을 위한 접지 오프셋")]
        public float GroundedOffset = -0.14f;

        [Tooltip("접지 판정에 사용되는 반지름 (CharacterController의 반지름과 일치해야 함)")]
        public float GroundedRadius = 0.28f;

        [Tooltip("지면으로 인식할 레이어")]
        public LayerMask GroundLayers;

        [Header("시네머신 카메라")]
        [Tooltip("Cinemachine 가상 카메라에서 따라갈 타겟 오브젝트")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("카메라가 위로 움직일 수 있는 최대 각도")]
        public float TopClamp = 70.0f;

        [Tooltip("카메라가 아래로 움직일 수 있는 최대 각도")]
        public float BottomClamp = -30.0f;

        [Tooltip("카메라 각도를 추가로 조정하는 값 (카메라 위치 고정 시 미세 조정용)")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("모든 축에서 카메라 위치를 고정할지 여부")]
        public bool LockCameraPosition = false;


        // 시네머신 관련 변수
        private float _cinemachineTargetYaw;    // 카메라 좌우 회전 각도
        private float _cinemachineTargetPitch;  // 카메라 상하 회전 각도

        // 플레이어 관련 변수
        private float _speed;                   // 현재 속도
        private float _animationBlend;          // 애니메이션 블렌딩 값
        private float _targetRotation = 0.0f;   // 목표 회전 각도
        private float _rotationVelocity;        // 회전 속도
        private float _verticalVelocity;        // 수직 속도 (점프/낙하 등)
        private float _terminalVelocity = 53.0f; // 최대 낙하 속도

        // 타임아웃 딜레이 관련 변수
        private float _jumpTimeoutDelta;        // 점프 쿨타임 타이머
        private float _fallTimeoutDelta;        // 낙하 쿨타임 타이머

        // 애니메이션 파라미터 ID
        private int _animIDSpeed;               // 속도 애니메이션 파라미터
        private int _animIDGrounded;            // 접지 상태 파라미터
        private int _animIDJump;                // 점프 파라미터
        private int _animIDFreeFall;            // 자유 낙하 파라미터
        private int _animIDMotionSpeed;         // 이동 속도 파라미터


#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            //"우리의 메인 카메라에 대한 참조 가져오기"
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError(" Starter Assets 패키지에 누락된 종속성이 있습니다. Tools/Starter Assets/Reinstall Dependencies를 사용하여 수정하세요.");
#endif

            AssignAnimationIDs();

            // 시작할 때 타임아웃을 초기화합니다.
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // 구체(스피어)의 위치를 오프셋과 함께 설정합니다.
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // 캐릭터에 애니메이터가 있는 경우 애니메이터를 업데이트합니다.
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // 입력이 있고 카메라 위치가 고정되지 않은 경우 실행됩니다.
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // 마우스 입력을 Time.deltaTime으로 곱하지 마세요.
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // 회전을 360도 범위 내로 제한합니다.
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine이 이 타겟을 따라가도록 설정합니다.
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // 이동 속도, 질주 속도 및 질주 입력 여부에 따라 목표 속도를 설정합니다.
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // 쉽게 제거, 교체 또는 개선할 수 있도록 단순한 가속 및 감속을 구현합니다.

            // 참고: Vector2의 == 연산자는 근사치를 사용하므로 부동 소수점 오류에 영향을 받지 않으며, magnitude를 사용하는 것보다 성능이 더 좋습니다.
            // 입력이 없으면 목표 속도를 0으로 설정합니다.
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // 플레이어의 현재 수평 속도를 참조합니다.
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // 목표 속도로 가속하거나 감속합니다.
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // 선형적인 속도 변화 대신 자연스러운 속도 변화를 위한 곡선을 생성합니다.
                // Lerp 함수의 T 값은 이미 클램핑되므로 별도로 속도를 제한할 필요가 없습니다.
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // 속도를 소수점 3자리까지 반올림합니다.
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // 입력 방향을 정규화합니다.
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // 참고: Vector2의 != 연산자는 근사치를 사용하므로 부동 소수점 오류에 영향을 받지 않으며, magnitude를 사용하는 것보다 성능이 더 좋습니다.
            // 이동 입력이 있는 경우 플레이어가 움직일 때 회전합니다.
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // 카메라 위치를 기준으로 입력 방향을 바라보도록 회전합니다.
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // 플레이어를 이동시킵니다.  
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // 캐릭터에 애니메이터가 있는 경우 애니메이터를 업데이트합니다. 
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // 낙하 타임아웃 타이머를 초기화합니다. 
                _fallTimeoutDelta = FallTimeout;

                // 캐릭터에 애니메이터가 있는 경우 애니메이터를 업데이트합니다. 
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // 플레이어가 땅에 닿아 있을 때 속도가 무한히 감소하는 것을 방지합니다.  
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // 점프
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // 점프 높이를 결정하는 공식: H * -2 * G의 제곱근 = 원하는 높이에 도달하기 위한 속도
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // 캐릭터에 애니메이터가 있는 경우 애니메이터를 업데이트합니다.  
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // 점프 타임아웃 설정  
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // 점프 타임아웃 타이머를 초기화합니다.
                _jumpTimeoutDelta = JumpTimeout;

                // 낙하 타임아웃 설정 
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // 캐릭터에 애니메이터가 있는 경우 애니메이터를 업데이트합니다.
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // 땅에 닿아 있지 않으면 점프하지 않습니다.
                _input.jump = false;
            }

            // 종단 속도 이하일 때 중력을 적용 (시간이 지남에 따라 중력이 점진적으로 증가하도록 Time.deltaTime을 두 번 곱함) 
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // 선택된 경우, 플레이어의 위치와 충돌체 반경에 맞는 기즈모를 그림  
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}