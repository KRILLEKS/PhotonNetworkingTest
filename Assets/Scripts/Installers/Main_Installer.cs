using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Zenject;

public class Main_Installer : MonoInstaller 
{
   public override void InstallBindings()
   {
      Container.BindInterfacesTo<LobbyManager>().AsSingle();
   }
}
