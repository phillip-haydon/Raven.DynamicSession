using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Raven.Client.Linq;

namespace Raven.DynamicSession.Tests
{
    [TestFixture]
    public class QueryGetFixture
    {
        public class DynamicQueries : EmbeddedRavenDBFixture
        {
            public class TestObject
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
            }

            [Test]
            public void Can_call_load_to_query_doc_stored_by_typed_insert()
            {
                //Arrange
                var store = GetStore();
                
                using (var session = store.OpenSession())
                {
                    session.Store(new Post
                    {
                        Name = "Test Document 1"
                    });
                    session.SaveChanges();
                }

                dynamic result;

                Thread.Sleep(1000);

                using (dynamic session = store.OpenDynamicSession())
                {
                    //Act
                    result = session.posts.load("posts/1");
                }

                //Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.EqualTo("Test Document 1"));
            }

            [Test]
            public void Can_call_get_to_query_doc_stored_by_typed_insert()
            {
                //Arrange
                var store = GetStore();

                using (var session = store.OpenSession())
                {
                    session.Store(new Post
                    {
                        Name = "Test Document 2"
                    });
                    session.SaveChanges();
                }

                using (dynamic session = store.OpenDynamicSession())
                {
                    //Act
                    dynamic result = session.posts.get("posts/1");

                    //Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Name, Is.EqualTo("Test Document 2"));
                }
            }

            [Test]
            public void Can_call_load_to_query_doc_stored_by_dynamic_insert()
            {
                //Arrange
                var store = GetStore();
                var id = Guid.NewGuid();

                using (dynamic session = store.OpenDynamicSession())
                {
                    session.posts.insert(new
                    {
                        Name = "Test Document 3"
                    }, id);
                    session.SaveChanges();
                }

                using (dynamic session = store.OpenDynamicSession())
                {
                    //Act
                    dynamic result = session.posts.load(id);

                    //Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Name, Is.EqualTo("Test Document 3"));
                }
            }

            [Test]
            public void Can_call_get_to_query_doc_stored_by_dynamic_insert()
            {
                //Arrange
                var store = GetStore();
                var id = Guid.NewGuid();

                using (dynamic session = store.OpenDynamicSession())
                {
                    session.posts.insert(new
                    {
                        Name = "Test Document 3"
                    }, id);
                    session.SaveChanges();
                }

                using (dynamic session = store.OpenDynamicSession())
                {
                    //Act
                    dynamic result = session.posts.get(id);

                    //Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Name, Is.EqualTo("Test Document 3"));
                }
            }
        }

        public class TypedQueries : EmbeddedRavenDBFixture
        {
            [Test]
            public void Can_query_post_after_dynamic_insert()
            {
                //Arrange
                var store = GetStore();
                DynamicSession.AddClrType("Posts", typeof(Post));

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
            
        }

        public class Post
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
