namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.WriteLine($"Error: {message}");
            }
            else
            {
                Console.WriteLine($"Info: {message}");
            }
        }
    }
}
