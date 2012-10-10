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
            var store = (new EmbeddableDocumentStore
            {
                RunInMemory = true
            }).Initialize();

            store.ConfigureDynamimcSession();

            return store;
        }
    }
}