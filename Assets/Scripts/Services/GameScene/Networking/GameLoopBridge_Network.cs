using Fusion;
using Models;
using Services.GameScene.TicTacToeGameController;
using StaticData.Enums;
using UI;
using Zenject;

namespace Services.GameScene.NetworkGameLoop_Service
{
   // Controls only communication between players. Doesn't control game loop
   public class GameLoopBridge_Network : NetworkBehaviour
   {
      private ITicTacToeGame_Service _ticTacToeGameService;
      
      [Inject]
      private void Construct(ITicTacToeGame_Service ticTacToeGameService)
      {
         _ticTacToeGameService = ticTacToeGameService;
      }
      
      [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
      public void RPC_RequestPlaceMark(int x, int y, Marks_Enum mark)
      {
         RPC_PlaceMark(x, y, mark);
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_PlaceMark(int x, int y, Marks_Enum mark)
      {
         _ticTacToeGameService.PlaceMark(x,y,mark);
      }
   }
}