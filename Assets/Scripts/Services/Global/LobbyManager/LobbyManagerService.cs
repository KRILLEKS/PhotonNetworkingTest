using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Services
{
   public class LobbyManagerService : ILobbyManager_Service,
                                      INetworkRunnerCallbacks
   {
      private NetworkRunner _runner;

      public async void StartGame(GameMode mode, string lobbyCode)
      {
         if (_runner != null)
            Shutdown();

         // Create the Fusion runner and let it know that we will be providing user input
         GameObject networkRunnerGO = new GameObject("Network runner");
         _runner = networkRunnerGO.AddComponent<NetworkRunner>();
         _runner.ProvideInput = true;

         _runner.AddCallbacks(this);

         // Create the NetworkSceneInfo from the current scene
         var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

         // Start or join (depends on gamemode) a session with a specific name
         var result = await _runner.StartGame(new StartGameArgs()
         {
            GameMode = mode,
            SessionName = lobbyCode,
            Scene = scene,
            SceneManager = networkRunnerGO.AddComponent<NetworkSceneManagerDefault>()
         });

         if (!result.Ok)
         {
            Debug.LogError($"Failed to start: {result.ErrorMessage}");
            Shutdown();
         }
      }

      public void Shutdown()
      {
         if (_runner == null)
            return;

         _runner.RemoveCallbacks(this);
         _runner.Shutdown();
         _runner = null;
      }

      public void ChangeScene(string sceneName)
      {
         if (_runner != null && _runner.IsServer)
            _runner.LoadScene(sceneName);
      }


      public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
      {
      }

      public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
      {
      }

      public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
      {
         Debug.Log("Player joined!");
         int playerCount = _runner.SessionInfo.PlayerCount;

         if (playerCount == Constants_Record.AutostartPlayerCount)
            ChangeScene(Constants_Record.GameScene);
      }

      public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
      {
      }

      public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
      {
      }

      public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
      {
      }

      public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
      {
      }

      public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
      {
      }

      public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
      {
      }

      public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
      {
      }

      public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
      {
      }

      public void OnInput(NetworkRunner runner, NetworkInput input)
      {
      }

      public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
      {
      }

      public void OnConnectedToServer(NetworkRunner runner)
      {
      }

      public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
      {
      }

      public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
      {
      }

      public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
      {
      }

      public void OnSceneLoadDone(NetworkRunner runner)
      {
      }

      public void OnSceneLoadStart(NetworkRunner runner)
      {
      }
   }
}