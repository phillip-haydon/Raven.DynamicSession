using System;
using NUnit.Framework;
using Raven.Abstractions.Data;
using Raven.Client.Embedded;

namespace Raven.DynamicSession.Tests
{
    [TestFixture]
    public class QueryGetFixture
    {
        [Test]
        public void Can_query_post_after_dynamic_insert()
        {
            //Arrange
            var store = (new EmbeddableDocumentStore
            {
                RunInMemory = true
            }).Initialize();

            //TODO: Wrap up the conventions for Raven.DynamicSession
            store.Conventions.FindClrType = (id, doc, metadata) =>
            {
                var clrType = metadata.Value<string>(DynamicSession.DynamicClrTypePlaceHolder);
                
                if (clrType.Equals("Posts", StringComparison.OrdinalIgnoreCase)) 
                    return "Raven.DynamicSession.Tests.QueryGetFixture.Post, Raven.DynamicSession.Tests";

                return metadata.Value<string>(Constants.RavenClrType);
            };

            using (dynamic session = store.OpenDynamicSession())
            {
                session.posts.insert(new
                {
                    Name = "Hello World"
                }, 123);

                session.SaveChanges();
            }

            //Act
            Post result;

            using (var session = store.OpenSession())
            {
                result = session.Load<Post>("posts/123");
            }

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Hello World"));
        }

        public class Post
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
