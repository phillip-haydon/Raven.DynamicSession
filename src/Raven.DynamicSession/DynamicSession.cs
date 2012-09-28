using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Raven.Client;

namespace Raven.DynamicSession
{
    public class DynamicSession : DynamicObject, IDisposable
    {
        protected IDocumentSession Session { get; set; }

        public DynamicSession(IDocumentSession session)
        {
            Session = session;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new ChainBuilder(this, Session, binder.Name);
            return true;
        }

        private class ChainBuilder : DynamicObject
        {
            private dynamic OriginalObject { get; set; }
            private IDocumentSession Session { get; set; }
            private string CollectionName { get; set; }

            private static readonly Type documentSessionType = typeof(IDocumentSession);

            public ChainBuilder(DynamicObject originalObject, IDocumentSession session, string collection)
            {
                OriginalObject = originalObject;
                Session = session;
                CollectionName = collection;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                if (binder.Name.Equals("load", StringComparison.OrdinalIgnoreCase))
                {
                    var generic = GetLoadMethod();

                    var id = CollectionName.ToLower() + "/" + args[0];

                    result = generic.Invoke(Session, new object[] { id });
                    return true;
                }

                if (binder.Name.Equals("insert", StringComparison.OrdinalIgnoreCase))
                {
                    MethodInfo method = GetStoreMethod();

                    var id = CollectionName.ToLower() + "/" + args[1];
                    var objectToStore = args[0];

                    method.Invoke(Session, new[] { objectToStore, id });

                    var metadata = Session.Advanced.GetMetadataFor(objectToStore);
                    metadata["Raven-Entity-Name"] = CollectionName;

                    result = null;
                    return true;
                }

                return base.TryInvokeMember(binder, args, out result);
            }

            private static MethodInfo GetLoadMethod()
            {
                var method = documentSessionType.GetMethods()
                                                .FirstOrDefault(x => x.Name == "Load")
                                                .MakeGenericMethod(typeof(object));

                return method;
            }

            private static MethodInfo GetStoreMethod()
            {
                return documentSessionType.GetMethods()
                    .Where(x =>
                    {
                        if (x.Name != "Store")
                            return false;

                        var methodParams = x.GetParameters();
                        if (methodParams.Length != 2)
                            return false;

                        if (methodParams[1].ParameterType == typeof(string))
                            return true;

                        return false;
                    })
                    .FirstOrDefault();
            }
        }

        public void SaveChanges()
        {
            Session.SaveChanges();
        }

        public void Dispose()
        {
            Session.Dispose();
        }
    }
}