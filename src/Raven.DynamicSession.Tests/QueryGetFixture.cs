using NUnit.Framework;

namespace Raven.DynamicSession.Tests
{
    [TestFixture]
    public class QueryGetFixture : EmbeddedRavenDBFixture
    {
        [Test]
        public void Can_query_post_after_dynamic_insert()
        {
            //Arrange
            var store = GetStore();
            Configure(store, typeof(Post), "Posts");

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
