using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Raven.DynamicSession.Tests;

namespace Raven.DynamicSession.TestConsole
{
    [TestFixture]
    public class DynamicSessionFixture : EmbeddedRavenDBFixture
    {
        [Test]
        [ExpectedException(typeof(MissingMethodException))]
        public void When_invoking_method_off_session_should_throw_excption()
        {
            var store = GetStore();

            using (dynamic session = store.OpenDynamicSession())
            {
                session.load(123);
            }
        }
    }

    [TestFixture]
    public class ChainBuilderFixture : EmbeddedRavenDBFixture
    {
        [Test]
        [ExpectedException(typeof(MissingMethodException))]
        public void When_invoking_unknown_method_should_throw_exception()
        {
            var store = GetStore();

            using (dynamic session = store.OpenDynamicSession())
            {
                session.posts.banana();
            }
        }
    }
}
