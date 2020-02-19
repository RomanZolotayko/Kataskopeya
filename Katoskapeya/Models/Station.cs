namespace Kataskopeya.Models
{
    public class Station
    {
        public Station(string message, bool isLastStation = false)
        {
            Message = message;
            IsLastStation = isLastStation;
        }

        public string Message { get; set; }

        public bool IsLastStation { get; set; }
    }
}
