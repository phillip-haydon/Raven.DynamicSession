using System;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Embedded;

namespace Raven.DynamicSession.Tests
{
    public abstract class EmbeddedRavenDBFixture
    {
        protected IDocumentStore GetStore()
        {
            return (new EmbeddableDocumentStore
            {
                RunInMemory = true
            }).Initialize();
        }

        protected void Configure(IDocumentStore store, Type type, string bananas)
        {
            //TODO: Wrap up the conventions for Raven.DynamicSession
            store.Conventions.FindClrType = (id, doc, metadata) =>
            {
                var clrType = metadata.Value<string>(DynamicSession.DynamicClrTypePlaceHolder);

                if (clrType.Equals(bananas, StringComparison.OrdinalIgnoreCase))
                    return type.FullName;

                return metadata.Value<string>(Constants.RavenClrType);
            };
        }
    }
}