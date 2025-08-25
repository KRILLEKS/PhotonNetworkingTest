using System;
using System.Collections.Generic;
using Plugins.Architecture.Extensions;
using UnityEngine;
using Zenject;

namespace Plugins.Architecture.StateMachine.StateMachineFactory
{
   public class StateMachineFactory_Service : IStateMachineFactory_Service
   {
      [Inject]
      private void Construct()
      {
      }

      /// <summary>
      /// </summary>
      /// <param name="states"></param>
      /// <param name="diContainer">diContainer defines binding scope</param>
      /// <typeparam name="TStateMachine"></typeparam>
      public void SetStateMachine<TStateMachine>(List<IBaseState> states, DiContainer diContainer)
      {
         diContainer.InjectAll(states);
         TStateMachine instance = (TStateMachine)Activator.CreateInstance(typeof (TStateMachine), states);
         diContainer.Bind<TStateMachine>().FromInstance(instance);
      }
   }
}