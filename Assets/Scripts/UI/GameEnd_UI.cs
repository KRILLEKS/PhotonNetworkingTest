using System;
using TMPro;
using UnityEngine;

namespace UI
{
   public class GameEnd_UI : MonoBehaviour
   {
      [SerializeField] private GameObject content;
      [SerializeField] private TextMeshProUGUI textTMP;

      public void SetMenu(string text)
      {
         content.SetActive(true);
         textTMP.text = text;
      }
   }
}
