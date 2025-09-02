using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using StateMachine.States;
using StaticData.Enums;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class TurnIndicator_UI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI yourTurnTMP;
   [SerializeField] private GameObject circleGO;
   [SerializeField] private GameObject crossGO;

   private YourTurn_State _yourTurnState;
   private SessionData_Model _sessionDataModel;

   [Inject]
   private void Construct(YourTurn_State yourTurnState, SessionData_Model sessionDataModel)
   {
      _yourTurnState = yourTurnState;
      _sessionDataModel = sessionDataModel;
   }

   private void Start()
   {
      SetTurnState(false);

      _sessionDataModel.OnMarkChange.Subscribe(SetMarkImage).AddTo(this);

      _yourTurnState.OnYourTurnStart.Subscribe(_ => SetTurnState(true))
                    .AddTo(this);

      _yourTurnState.OnYourTurnEnd.Subscribe(_ => SetTurnState(false))
                    .AddTo(this);
   }

   private void SetMarkImage(Marks_Enum mark)
   {
      if (mark == Marks_Enum.Circle)
      {
         circleGO.SetActive(true);
         crossGO.SetActive(false);
      }
      else
      {
         circleGO.SetActive(false);
         crossGO.SetActive(true);
      }
   }

   public void SetTurnState(bool state)
   {
      yourTurnTMP.gameObject.SetActive(state);
   }
}