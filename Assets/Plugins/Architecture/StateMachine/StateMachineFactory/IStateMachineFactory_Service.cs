using System.Collections.Generic;
using Zenject;

namespace Plugins.Architecture.StateMachine.StateMachineFactory
{
   public interface IStateMachineFactory_Service
   {
      void SetStateMachine<TStateMachine>(List<IBaseState> states, DiContainer diContainer);
   }
}