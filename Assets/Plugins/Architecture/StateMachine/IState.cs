namespace Plugins.Architecture.StateMachine
{
   public interface IBaseState
   {
      void Exit();
   }
   
   public interface IBaseStateWithLocalStates : IBaseState
   {
   }
   
   public interface IState : IBaseState
   {
      void Enter();
   }
   
   /// <summary>
   /// preferably use this state with DTO as payload instead of state with 2 payloads unless you want to separate 1_parameter and DTO
   /// </summary>
   /// <typeparam name="TPayload"></typeparam>
   public interface IPayloadState<TPayload> : IBaseState
   { 
      void Enter(TPayload payload);
   }
   
   /// <summary>
   /// use in case when you want separate any parameter and DTO (for example, info how dto should be handled)
   /// usually it's not needed
   /// </summary>
   /// <typeparam name="T1Payload"></typeparam>
   /// <typeparam name="T2Payload"></typeparam>
   public interface IPayloadState<T1Payload, T2Payload> : IBaseState
   { 
      void Enter(T1Payload payload1, T2Payload payload2);
   }
}