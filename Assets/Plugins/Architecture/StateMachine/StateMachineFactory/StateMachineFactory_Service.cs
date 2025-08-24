using System;
using System.Collections.Generic;
using Plugins.Architecture.Extensions;
using Zenject;

namespace Plugins.Architecture.StateMachine.StateMachineFactory
{
   public class StateMachineFactory_Service : IStateMachineFactory_Service
   {
      private DiContainer _diContainer;

      [Inject]
      private void Construct(DiContainer diContainer)
      {
         _diContainer = diContainer;
      }

      public void SetStateMachine<TStateMachine>(List<IBaseState> states)
      {
         _diContainer.InjectAll(states);
         TStateMachine instance = (TStateMachine)Activator.CreateInstance(typeof (TStateMachine), states);
         _diContainer.Bind<TStateMachine>().FromInstance(instance);
      }
   }
}