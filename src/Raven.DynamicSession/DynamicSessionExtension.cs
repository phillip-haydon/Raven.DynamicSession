using System;
using Raven.Abstractions.Data;
using Raven.Client;

namespace Raven.DynamicSession
{
    public static class DynamicSessionExtension
    {
        public static DynamicSession OpenDynamicSession(this IDocumentStore store)
        {
            return new DynamicSession(store.OpenSession());
        }

        public static void ConfigureDynamimcSession(this IDocumentStore store)
        {
            store.Conventions.FindClrType = (id, doc, metadata) =>
            {
                var clrType = (metadata.Value<string>(DynamicSession.DynamicClrTypePlaceHolder) ?? "").ToLower();

                if (DynamicSession.ClrTypeConversions.ContainsKey(clrType))
                    return DynamicSession.ClrTypeConversions[clrType];

                return metadata.Value<string>(Constants.RavenClrType);
            };
        }
    }
}
