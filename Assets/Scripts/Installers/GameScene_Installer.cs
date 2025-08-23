using Services.TicTacToeGrid;
using Zenject;

namespace Installers
{
   public class GameScene_Installer : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<TicTacToeGrid_Service>().AsSingle();
      }

      // It's not correct to initialize the game in the installer. Made it this way for simplicity
      public override void Start()
      {
         Container.Resolve<ITicTacToeGrid_Service>().GenerateGrid();
      }
   }
}