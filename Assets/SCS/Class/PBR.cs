using System.Collections.Generic;
using UnityEngine;

public class PBR : Monster
{
    private Dictionary<string, string> patternToAnimTrigger = new Dictionary<string, string>()
    {
        { "Shoot", "Shoot" },
        { "Reloed", "Reload" }
    };

    public string currentAnimTrigger;
    public bool isAttacking = false;
    public float chaseRange = 30f;
    public float shotingRange = 11f;
    public float ReloedTime = 3f;
    private float ReloedTimer = 0f;
    public SGShotCtrl shotCtrl;



    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (player == null || agent == null) return;

        if (isAttacking)
        {
            agent.isStopped = true;
            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
            animator.SetBool("IsShooting", true);

        }
        else
        {
            float distance = Vector3.Distance(transform.position, player.position);


            if (distance <= chaseRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else if (distance >= shotingRange)
            {
                agent.isStopped = true;
                ChangePattern("Shoot");
                Debug.Log("Shooting Pattern 진입"); // 이게 뜨는지 확인
                Debug.Log(distance); // 이게 뜨는지 확인
                //ReloedTimer += Time.deltaTime;
                //if (ReloedTime <= ReloedTimer) 
                //{
                //    ChangePattern("Reloed");
                //}

            }
            else
            {
                agent.isStopped = true;

            }
        }
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

    }

    public void ChangePattern(string patternName)
    {
        switch (patternName)
        {
            case "Shoot":
                currentParryThreshold = 1f;
                ParryStatus = "Block";
                shotCtrl.StartShotRoutine(); // 사격시작하는 코드
                ReloedTimer += Time.deltaTime;
                if (ReloedTime <= ReloedTimer)
                {
                    ChangePattern("Reload");
                }
                break;
            case "Reload":
                currentParryThreshold = 2f;
                ParryStatus = "Groggy";
                shotCtrl._shooting = false; // 멈추는 코드
                break;
        }

        if (patternToAnimTrigger.TryGetValue(patternName, out var animTrigger))
        {
            currentAnimTrigger = animTrigger;
            if (animator != null)
            {
                animator.SetTrigger(currentAnimTrigger);
            }
        }

        isAttacking = true;
    }

    // 애니메이션에서 호출
    public void EndAttack()
    {
        isAttacking = false;
    }
}
