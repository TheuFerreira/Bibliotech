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

        public Server Server { get; set; }

        private Session()
        {
            Server = new Server();
        }
    }
}
