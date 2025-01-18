namespace ScoreboardController.Data
{
    public class SoftKeyModel
    {
        public int Id { get; set; }
        public int SetId { get; set; }   // e.g. "1" for Hockey
        public string DisplayText { get; set; } = string.Empty;
        public string CommandText { get; set; } = string.Empty;

        // Potentially more fields like:
        // public string ElementName { get; set; } = string.Empty;
        // public string ValueToSend { get; set; } = string.Empty;
    }
}