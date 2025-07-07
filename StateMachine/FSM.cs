using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,
    Patrol,
    Attack,
    Chase
}
[System.Serializable] public class Parameter
{
    public int health ;
    public float MoveSpeed;
    public float ChaseSpeed;
    public float IdleTime;
    public Transform[] patrolPoints;
    public Transform[] chasePoints;
    public Animator animator;
    public Transform target;
    public LayerMask targetLayer;
    public Transform attackPoint;
    public float attackArea;
}

public class FSM : MonoBehaviour
{
    public Parameter parameter;
    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    void Start()
    {
        if (parameter == null)
            parameter = new Parameter();

        parameter.animator = GetComponent<Animator>();

        states.Add(StateType.Idle, new IdleState(this, parameter));
        states.Add(StateType.Patrol, new PatrolState(this, parameter));
        states.Add(StateType.Attack, new AttackState(this, parameter));
        states.Add(StateType.Chase, new ChaseState(this, parameter));

        TransitionState(StateType.Idle);
    }

    void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionState(StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter(); // 进入新状态时调用
    }

    public void FlipTo(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
}
