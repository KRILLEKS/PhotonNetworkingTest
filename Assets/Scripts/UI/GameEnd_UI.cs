using System;
using Services.GameScene.NetworkGameLoop_Service;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
   public class GameEnd_UI : MonoBehaviour
   {
      [SerializeField] private GameObject content;
      [SerializeField] private TextMeshProUGUI textTMP;
      [SerializeField] private Button rematchButton;

      private GameLoop_Network _gameLoopNetwork;

      [Inject]
      private void Construct(GameLoop_Network gameLoopNetwork)
      {
         _gameLoopNetwork = gameLoopNetwork;
      }

      private void Start()
      {
         HideMenu();
         
         rematchButton.onClick.AddListener(VoteForRematch);
         
         _gameLoopNetwork.BeforeRematch.Subscribe(_ => HideMenu())
                         .AddTo(this);
      }

      private void HideMenu()
      {
         content.SetActive(false);
      }

      public void SetMenu(string text)
      {
         content.SetActive(true);
         textTMP.text = text;
      }

      public void VoteForRematch()
      {
         _gameLoopNetwork.RPC_VoteForRematch();
      }
   }
}