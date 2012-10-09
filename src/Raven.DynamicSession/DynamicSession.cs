using System;
using System.Dynamic;
using System.Linq;
using Raven.Client;

namespace Raven.DynamicSession
{
    public class DynamicSession : DynamicObject, IDisposable
    {
        protected IDocumentSession Session { get; set; }

        public static string DynamicClrTypePlaceHolder
        {
            get { return "Raven.DynamicSession.DynamicClrTypePlaceHolder"; }
        }

        public DynamicSession(IDocumentSession session)
        {
            Session = session;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new ChainBuilder(this, Session, binder.Name);
            return true;
        }
        
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            throw new MissingMethodException("There are no methods off the root Dynamic Session, first value"
                                         + " should be a member that represents the collection. e.g "
                                         + "session.posts.*method*");
        }

        private class ChainBuilder : DynamicObject
        {
            private dynamic OriginalObject { get; set; }
            private IDocumentSession Session { get; set; }
            private string CollectionName { get; set; }

            public ChainBuilder(DynamicObject originalObject, IDocumentSession session, string collection)
            {
                OriginalObject = originalObject;
                Session = session;
                CollectionName = collection;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                switch (binder.Name.ToLower())
                {
                    case "load":
                    case "get":
                    {
                        if (args.Length != 1)
                        {
                            throw new ArgumentException("Get/Load should have 1 parameter, 1:Id");
                        }

                        HandleLoad(args, out result);
                        return true;
                    }
                    case "insert":
                    {
                        if (args.Length != 2)
                        {
                            throw new ArgumentException("Insert should have 2 parameters, 1:ObjectToStore, 2:Id");
                        }

                        return HandleInsert(args, out result);
                    }
                    case "all":
                    {
                        result = Session.Advanced.LuceneQuery<dynamic>()
                                        .WhereEquals("@metadata.Raven-Entity-Name", CollectionName)
                                        .ToList();

                        return true;
                    }
                }

                throw new MissingMethodException("Method {0} does not exist. Allowed methods are: Get/Load, Insert, All");
            }

            private void HandleLoad(object[] args, out object result)
            {
                string id;
                var guidId = (args[0] as Guid?) ?? Guid.Empty;

                if (args[0] is Guid || Guid.TryParse(args[0] as string, out guidId))
                {
                    id = guidId.ToString();
                }
                else if (args[0] is int)
                {
                    id = CollectionName.ToLower() + "/" + args[0];
                }
                else
                {
                    id = (string) args[0];
                }

                result = Session.Load<dynamic>(id);
            }

            private bool HandleInsert(object[] args, out object result)
            {
                var objectToStore = args[0];
                string id;

                Guid guidId;

                if (Guid.TryParse(args[1].ToString(), out guidId))
                {
                    id = guidId.ToString();
                }
                else if (args[1] is int)
                {
                    id = CollectionName.ToLower() + "/" + args[1];
                }
                else
                {
                    id = (string) args[1];
                }

                Session.Store(objectToStore, id);

                var metadata = Session.Advanced.GetMetadataFor(objectToStore);
                metadata["Raven-Entity-Name"] = CollectionName;
                metadata[DynamicClrTypePlaceHolder] = CollectionName;

                result = objectToStore;
                return true;
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