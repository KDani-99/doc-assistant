namespace Client.Utils
{
    public abstract class Config
    {
        private static readonly string Protocol = "http";
        private static readonly string IP = "localhost";
        private static readonly int Port = 5000;

        public static readonly string ServerAddress = Protocol + "://" + IP + ":" + Port + "/api";
    }
}