using System.Collections;
using System.Collections.Generic;
using Services;
using Services.ResourcesProvider;
using UnityEngine;
using Zenject;

public class Main_Installer : MonoInstaller 
{
   public override void InstallBindings()
   {
      Container.BindInterfacesTo<LobbyManagerService>().AsSingle();
      Container.BindInterfacesTo<ResourcesProviderService>().AsSingle();
   }
}
