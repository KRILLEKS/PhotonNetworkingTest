using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class Lobby_UI : MonoBehaviour
{
   [SerializeField] private Button CreateButton;
   [SerializeField] private TMP_InputField CreateInputField;
   [SerializeField] private Button JoinButton;
   [SerializeField] private TMP_InputField JoinInputField;

   private ILobbyManager _lobbyManager;

   [Inject]
   private void Construct(ILobbyManager lobbyManager)
   {
      _lobbyManager = lobbyManager;
   }

   private void Start()
   {
      CreateButton.onClick.AddListener(CreateRoom);
      JoinButton.onClick.AddListener(JoinRoom);
   }

   public void CreateRoom()
   {
      _lobbyManager.StartGame(GameMode.Host, CreateInputField.text);
   }

   public void JoinRoom()
   {
      _lobbyManager.StartGame(GameMode.Client, JoinInputField.text);
   }
}