using System;
using Raven.Client.Document;

namespace Raven.DynamicSession.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var documentStore = (new DocumentStore()
            {
                DefaultDatabase = "Testing",
                Url = "http://localhost:8080"
            }).Initialize();

            using (dynamic chainer = documentStore.OpenDynamicSession())
            {
                chainer.Posts.insert(new
                {
                    Name = "Rabbit"
                }, "909");
                chainer.Posts.Insert(new
                {
                    Name = "Banana"
                }, "123");
                chainer.People.insert(new
                {
                    FirstName = "Phillip"
                }, "1");
                chainer.People.Insert(new
                {
                    FirstName = "Prabir"
                }, "2");

                chainer.SaveChanges();
            }

            using (dynamic chainer = documentStore.OpenDynamicSession())
            {
                dynamic result = chainer.Posts.load(123);
                Console.WriteLine(result.Name);

                dynamic result2 = chainer.Posts.Load(909);
                Console.WriteLine(result2.Name);

                dynamic result3 = chainer.People.load(1);
                Console.WriteLine(result3.FirstName);

                dynamic result4 = chainer.People.Load(2);
                Console.WriteLine(result4.FirstName);
            }

            Console.ReadKey();
        }
    }
}
