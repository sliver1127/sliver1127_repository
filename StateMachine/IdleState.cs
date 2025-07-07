using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEngine;



public class IdleState : IState
{
    private FSM manager;
    private Parameter parameter;
    private float timer;
    public IdleState(FSM manager, Parameter parameter)
    {
        this.manager = manager;
        this.parameter = parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Spider_idle");
    }
    
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (parameter.target != null &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.Chase);
        }
        if (timer >= parameter.IdleTime) 
        {
            manager.TransitionState(StateType.Patrol);
            
        }
    }
    public void OnExit()
    {
        timer = 0f;
    }


}
public class PatrolState : IState
{
    private FSM manager;
    private Parameter parameter;
    private int patrolPosition;
    private float waitTime;
    private float startWaitTime = 1f; // 可根据需要调整

    public PatrolState(FSM manager, Parameter parameter)
    {
        this.manager = manager;
        this.parameter = parameter;
        this.patrolPosition = 0;
        this.waitTime = startWaitTime;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Spider_walk");
    }

    public void OnUpdate()
    {
        if (parameter.patrolPoints == null || parameter.patrolPoints.Length == 0)
            return;
        if (parameter.target != null &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.Chase);
        }
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);
        manager.transform.position = Vector2.MoveTowards(
            manager.transform.position,
            parameter.patrolPoints[patrolPosition].position,
            parameter.MoveSpeed * Time.deltaTime);

        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 0.1f)
        {
            manager.TransitionState(StateType.Patrol); 
        }
        
    }

    public void OnExit()
    {
        patrolPosition = (patrolPosition + 1) % parameter.patrolPoints.Length; // 循环切换巡逻点
    }
}
public class AttackState : IState
{
    private FSM manager;
    private Parameter parameter;
    private AnimatorStateInfo info;
    public AttackState(FSM manager, Parameter parameter)
    {
        this.manager = manager;
        this.parameter = parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Spider_attack");

    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {

            if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
                manager.TransitionState(StateType.Chase);
        }
    }
    
    public void OnExit()
    {
        // 退出行走状态时的逻辑
        Debug.Log("Exiting Walk State");
    }

}


public class ChaseState : IState
{
    private FSM manager;
    private Parameter parameter;
    public ChaseState(FSM manager, Parameter parameter)
    {
        this.manager = manager;
        this.parameter = parameter;
    }
    public void OnEnter()
    {

        parameter.animator.Play("Spider_chase");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.target);
        if (parameter.target) 
            manager.transform.position = Vector2.MoveTowards(
                manager.transform.position,
                parameter.target.position,
                parameter.MoveSpeed * Time.deltaTime);
        if (parameter.target == null ||
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.Idle);
        }
        if(Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(StateType.Attack);
        }


    }
    public void OnExit()
    {
        // 退出行走状态时的逻辑
        Debug.Log("Exiting Walk State");
    }

}