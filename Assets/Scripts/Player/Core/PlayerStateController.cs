using UnityEngine;

public enum PlayerState
{
    Idle,
    Move,
    Run,
    Attack,
    Guard,
    Dodge,
    Hit,
    Dead
}

public class PlayerStateController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public PlayerState PreviousState { get; private set; } = PlayerState.Idle;

    public bool IsState(PlayerState state)
    {
        return CurrentState == state;
    }

    public bool IsInActionState()
    {
        return CurrentState == PlayerState.Attack ||
               CurrentState == PlayerState.Guard ||
               CurrentState == PlayerState.Dodge ||
               CurrentState == PlayerState.Hit ||
               CurrentState == PlayerState.Dead;
    }

    public bool CanMove()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Move ||
               CurrentState == PlayerState.Run;
    }

    public bool CanRun()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Move ||
               CurrentState == PlayerState.Run;
    }

    public bool CanAttack()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Move ||
               CurrentState == PlayerState.Run;
    }

    public bool CanGuard()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Move ||
               CurrentState == PlayerState.Run;
    }

    public bool CanDodge()
    {
        return CurrentState == PlayerState.Idle ||
               CurrentState == PlayerState.Move ||
               CurrentState == PlayerState.Run;
    }

    public bool CanTransition(PlayerState nextState)
    {
        if (CurrentState == PlayerState.Dead)
            return false;

        if (CurrentState == nextState)
            return true;

        switch (CurrentState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
            case PlayerState.Run:
                return true;

            case PlayerState.Attack:
                return nextState == PlayerState.Hit || nextState == PlayerState.Dead || nextState == PlayerState.Idle;

            case PlayerState.Guard:
                return nextState == PlayerState.Idle || nextState == PlayerState.Hit || nextState == PlayerState.Dead;

            case PlayerState.Dodge:
                return nextState == PlayerState.Idle || nextState == PlayerState.Hit || nextState == PlayerState.Dead;

            case PlayerState.Hit:
                return nextState == PlayerState.Idle || nextState == PlayerState.Dead;

            default:
                return false;
        }
    }

    public bool ChangeState(PlayerState newState)
    {
        if (!CanTransition(newState))
            return false;

        if (CurrentState == newState)
            return true;

        PreviousState = CurrentState;
        CurrentState = newState;

        return true;
    }

    public bool IsBusy()
    {
        return CurrentState == PlayerState.Attack ||
               CurrentState == PlayerState.Guard ||
               CurrentState == PlayerState.Dodge ||
               CurrentState == PlayerState.Hit;
    }
}