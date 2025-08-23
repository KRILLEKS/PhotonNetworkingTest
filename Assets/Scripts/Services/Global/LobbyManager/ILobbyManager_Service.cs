using Fusion;

namespace Services
{
   public interface ILobbyManager_Service
   {
      void StartGame(GameMode mode, string lobbyCode);
   }
}