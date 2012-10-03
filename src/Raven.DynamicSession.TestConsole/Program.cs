using System;
using System.Collections.Generic;
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
                Console.WriteLine("----- insert tests");

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
                Console.WriteLine("----- all tests");

                IEnumerable<dynamic> posts = session.Posts.all();
                IEnumerable<dynamic> people = session.People.all();

                foreach (var post in posts)
                {
                    Console.WriteLine(post.Name);
                }

                foreach (var person in people)
                {
                    Console.WriteLine(person.FirstName);
                }
            }

            using (dynamic session = documentStore.OpenDynamicSession())
            {
                Console.WriteLine("----- load tests");

                dynamic post1 = session.Posts.load(123);
                Console.WriteLine(post1.Name);

                dynamic post2 = session.Posts.Load(909);
                Console.WriteLine(post2.Name);

                dynamic person1 = session.People.load(1);
                Console.WriteLine(person1.FirstName);

                dynamic person2 = session.People.Load(2);
                Console.WriteLine(person2.FirstName);
            }

            using (dynamic session = documentStore.OpenDynamicSession())
            {
                Console.WriteLine("----- get tests");

                dynamic post1 = session.Posts.gET(123);
                Console.WriteLine(post1.Name);

                dynamic post2 = session.Posts.get(909);
                Console.WriteLine(post2.Name);

                dynamic person1 = session.People.GET(1);
                Console.WriteLine(person1.FirstName);

                dynamic person2 = session.People.Get(2);
                Console.WriteLine(person2.FirstName);
            }

            Console.ReadKey();
        }
    }
}
