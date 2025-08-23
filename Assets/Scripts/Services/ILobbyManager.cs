using Fusion;

namespace Services
{
   public interface ILobbyManager
   {
      void StartGame(GameMode mode, string lobbyCode);
   }
}