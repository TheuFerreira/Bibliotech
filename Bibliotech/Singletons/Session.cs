using Bibliotech.Model.Entities;

namespace Bibliotech.Singletons
{
    public class Session
    {
        public static Session Instance
        {
            get
            {
                if (session == null)
                    session = new Session();

                return session;
            }
        }
        private static Session session;

        public User User { get; set; }
        public Server Server { get; set; }

        private Session()
        {
            Server = new Server();
        }
    }
}
