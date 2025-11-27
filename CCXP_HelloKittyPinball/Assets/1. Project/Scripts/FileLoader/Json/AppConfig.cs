using Newtonsoft.Json;

namespace RealGames
{
    [System.Serializable]
    public class AppConfig
    {
        public string applicationName;
        public int maxInactiveTime;
        public bool RankingDiario;
        public int TempoDeJogo;
        public int TempoGameOver;
        public int TempoRanking;
    }
}
