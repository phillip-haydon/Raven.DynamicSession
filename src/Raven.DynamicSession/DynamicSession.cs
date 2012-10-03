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
                        Guid guidId;
                        if (Guid.TryParse(args[0].ToString(), out guidId))
                        {
                            result = Session.Load<dynamic>(guidId);
                        }
                        else
                        {
                            var id = CollectionName.ToLower() + "/" + args[0];
                            result = Session.Load<dynamic>(id);
                        }

                        return true;
                    }
                    case "insert":
                    {
                        var objectToStore = args[0];
                        var id = CollectionName.ToLower() + "/" + args[1];

                        Session.Store(objectToStore, id);

                        var metadata = Session.Advanced.GetMetadataFor(objectToStore);
                        metadata["Raven-Entity-Name"] = CollectionName;
                        metadata[DynamicClrTypePlaceHolder] = CollectionName;

                        result = objectToStore;
                        return true;
                    }
                    case "all":
                    {
                        result = Session.Advanced.LuceneQuery<dynamic>()
                                        .WhereEquals("@metadata.Raven-Entity-Name", CollectionName)
                                        .ToList();

                        return true;
                    }
                }

                return base.TryInvokeMember(binder, args, out result);
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