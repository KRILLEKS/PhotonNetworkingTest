using System.Collections.Generic;

namespace Plugins.Architecture.StateMachine.StateMachineFactory
{
   public interface IStateMachineFactory_Service
   {
      void SetStateMachine<TStateMachine>(List<IBaseState> states);
   }
}