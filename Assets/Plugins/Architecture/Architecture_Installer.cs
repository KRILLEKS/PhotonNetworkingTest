using Plugins.Architecture.StateMachine.StateMachineFactory;
using Zenject;

namespace Plugins.Architecture
{
   public class Architecture_Installer : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<StateMachineFactory_Service>().AsSingle();
      }
   }
}