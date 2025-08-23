using UnityEngine;

namespace Services.ResourcesProvider
{
   public class ResourcesProviderService : IResourcesProvider_Service
   {
      public T LoadResource<T>(string path)
         where T : Object
         => Resources.Load<T>(path);

      public T[] LoadResources<T>(string path)
         where T : Object
         => Resources.LoadAll<T>(path);
   }
}