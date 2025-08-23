using UnityEngine;

namespace Services.ResourcesProvider
{
   public interface IResourcesProvider_Service
   {
      T LoadResource<T>(string path)
         where T : Object;

      T[] LoadResources<T>(string path)
         where T : Object;
   }
}