using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.Architecture.StateMachine
{
   public abstract class BaseStateMachine
   {
      protected IBaseState currentState;
      protected Dictionary<Type, IBaseState> states;
      
      public BaseStateMachine(List<IBaseState> statesToSet)
      {
         states = statesToSet.ToDictionary(x => x.GetType(), x => x);
      }


      public void Enter<TState>()
         where TState : class, IState
      {
         IState state = ChangeStateTo<TState>();
         state.Enter();
      }

      public void Enter<TState, TPayload>(TPayload payload)
         where TState : class, IPayloadState<TPayload>
      {
         IPayloadState<TPayload> state = ChangeStateTo<TState>();
         state.Enter(payload);
      }

      public void Enter<TState, T1Payload, T2Payload>(T1Payload payload1, T2Payload payload2)
         where TState : class, IPayloadState<T1Payload, T2Payload>
      {
         IPayloadState<T1Payload, T2Payload> state = ChangeStateTo<TState>();
         state.Enter(payload1, payload2);
      }

      public virtual void ExitCurrentState()
      {
         currentState?.Exit();
         currentState = null;
      }

      protected virtual TState EnterNewState<TState>()
         where TState : class, IBaseState
      {
         TState newState = states[typeof (TState)] as TState;
         currentState = newState;

         return newState;
      }

      protected virtual TState ChangeStateTo<TState>()
         where TState : class, IBaseState
      {
         ExitCurrentState();
         return EnterNewState<TState>();
      }
   }
}