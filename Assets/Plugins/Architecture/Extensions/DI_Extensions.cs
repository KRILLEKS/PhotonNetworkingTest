using System.Collections;
using Zenject;

namespace Plugins.Architecture.Extensions
{
   public static class DI_Extensions
   {
      public static void InjectAll<T>(this DiContainer container, T collection)
         where T : IEnumerable
      {
         foreach (var item in collection)
         {
            container.QueueForInject(item);
         }
      }
   }
}