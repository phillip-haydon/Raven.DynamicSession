using System;
using Raven.Client.Document;

namespace Raven.DynamicSession.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var documentStore = (new DocumentStore
            {
                DefaultDatabase = "Testing",
                Url = "http://localhost:8080"
            }).Initialize();

            using (dynamic session = documentStore.OpenDynamicSession())
            {
                session.Posts.insert(new
                {
                    Name = "Rabbit"
                }, "909");
                session.Posts.Insert(new
                {
                    Name = "Banana"
                }, "123");
                session.People.insert(new
                {
                    FirstName = "Phillip"
                }, "1");
                session.People.Insert(new
                {
                    FirstName = "Prabir"
                }, "2");

                session.SaveChanges();
            }

            using (dynamic session = documentStore.OpenDynamicSession())
            {
                dynamic post1 = session.Posts.load(123);
                Console.WriteLine(post1.Name);

                dynamic post2 = session.Posts.Load(909);
                Console.WriteLine(post2.Name);

                dynamic person1 = session.People.load(1);
                Console.WriteLine(person1.FirstName);

                dynamic person2 = session.People.Load(2);
                Console.WriteLine(person2.FirstName);
            }

            Console.ReadKey();
        }
    }
}
