using System.Collections.Generic;
using UnityEngine;

public class PBR : Monster
{
    private Dictionary<string, string> patternToAnimTrigger = new Dictionary<string, string>()
    {
        { "Slash", "SlashAnim" },
        { "CastSlash", "CastSlashAnim" }
    };

    public string currentAnimTrigger;
    public bool isAttacking = false;
    public float chaseDistance = 10f;


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
        }
        else
        {
            float distance = Vector3.Distance(transform.position, player.position);


            if (distance <= chaseDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
            }

            // 패턴 발동 시도 → 공격 중 아닐 때는 항상 시도
            // 여기에 탄막 코드하고 연동해야함
        }
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

    }

    public void ChangePattern(string patternName)
    {
        switch (patternName)
        {
            case "Slash":
                currentParryThreshold = 1f;
                ParryStatus = "Block";
                break;
            case "CastSlash":
                currentParryThreshold = 2f;
                ParryStatus = "Groggy";
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
