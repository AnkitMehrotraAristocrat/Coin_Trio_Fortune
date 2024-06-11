using Milan.FrontEnd.Bridge.Logging;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public static class GameIdLogger
    {
        public static ILogger Logger
        {
            get
            {
                if (_loggerInstance == null)
                {
                    _loggerInstance = LogManager.GetLogger("GAMEID");
                }

                return _loggerInstance;
            }
        }
        private static ILogger _loggerInstance;
    }
}