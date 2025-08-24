using System.Collections.Generic;
using Plugins.Architecture.StateMachine;

namespace StateMachine
{
   public class GameLoop_StateMachine : BaseStateMachine 
   {
      public GameLoop_StateMachine(List<IBaseState> statesToSet) : base(statesToSet)
      {
      }
   }
}