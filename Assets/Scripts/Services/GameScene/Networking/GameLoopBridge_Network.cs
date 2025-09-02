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
      private TicTacToeGame_Model _ticTacToeGameModel;
      
      [Inject]
      private void Construct(TicTacToeGame_Model ticTacToeGameModel)
      {
         _ticTacToeGameModel = ticTacToeGameModel;
      }
      
      [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
      public void RPC_RequestPlaceMark(int x, int y, Marks_Enum mark)
      {
         RPC_PlaceMark(x, y, mark);
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_PlaceMark(int x, int y, Marks_Enum mark)
      {
         _ticTacToeGameModel.SetMark(x, y, mark);
      }
   }
}