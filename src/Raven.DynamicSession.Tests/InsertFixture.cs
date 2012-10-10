using NUnit.Framework;

namespace Raven.DynamicSession.Tests
{
    [TestFixture]
    public class InsertFixture : EmbeddedRavenDBFixture
    {
        [Test]
        public void Can_insert_with_lowercase_insert_keyword()
        {
            //Arrange
            var store = GetStore();
            DynamicSession.AddClrType("Bananas", typeof(Banana));

            using (dynamic session = store.OpenDynamicSession())
            {
                session.Bananas.insert(new
                {
                    Colour = "Yellow",
                    Bunch = 14
                }, 1);

                session.SaveChanges();
            }

            Banana result;

            using (var session = store.OpenSession())
            {
                result = session.Load<Banana>("bananas/1");
            }

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Colour, Is.EqualTo("Yellow"));
        }

        [Test]
        public void Can_insert_with_uppercase_insert_keyword()
        {
            //Arrange
            var store = GetStore();
            DynamicSession.AddClrType("Bananas", typeof(Banana));

            using (dynamic session = store.OpenDynamicSession())
            {
                session.Bananas.INSERT(new
                {
                    Colour = "Red",
                    Bunch = 14
                }, 10);

                session.SaveChanges();
            }

            Banana result;

            using (var session = store.OpenSession())
            {
                result = session.Load<Banana>("bananas/10");
            }

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Colour, Is.EqualTo("Red"));
        }

        public class Banana
        {
            public string Id { get; set; }
            public string Colour { get; set; }
            public string Bunch { get; set; }
        }
    }
}
