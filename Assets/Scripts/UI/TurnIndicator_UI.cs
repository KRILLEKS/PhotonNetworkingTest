using System;
using System.Collections;
using System.Collections.Generic;
using StaticData.Enums;
using TMPro;
using UnityEngine;

public class TurnIndicator_UI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI yourTurnTMP;
   [SerializeField] private GameObject circleGO;
   [SerializeField] private GameObject crossGO;

   private void Start()
   {
      SetTurnState(false);
   }

   public void SetMarkImage(Marks mark)
   {
      if (mark == Marks.Circle)
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
