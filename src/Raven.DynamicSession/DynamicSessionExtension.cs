using Raven.Client;

namespace Raven.DynamicSession
{
    public static class DynamicSessionExtension
    {
        public static DynamicSession OpenDynamicSession(this IDocumentStore store)
        {
            return new DynamicSession(store.OpenSession());
        }
    }
}
