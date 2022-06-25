namespace MROCoatching.DataObjects.Models.Others
{
    public class AppMessage
    {
        public string Text { get; set; }
        public int Code { get; set; }
        public int Prompt { get; set; }
        public static int SucessCode { get { return 1; } }
        public static int ErrorCode { get { return -1; } }
        public static int PromptCode { get { return 2; } }
    }
}
