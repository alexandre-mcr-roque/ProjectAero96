namespace ProjectAero96.Helpers
{
    public class FormSummary(string alertStyle, string text)
    {
        public string AlertStyle { get; } = alertStyle;
        public string Text { get; } = text;
        public override string ToString() => Text;
        /**<summary>Blue BG</summary>*/     public static FormSummary Primary(string text) => new("primary", text);
        /**<summary>Gray BG</summary>*/     public static FormSummary Secondary(string text) => new("secondary", text);
        /**<summary>Green BG</summary>*/    public static FormSummary Success(string text) => new("success", text);
        /**<summary>Red BG</summary>*/      public static FormSummary Danger(string text) => new("danger", text);
        /**<summary>Yellow BG</summary>*/   public static FormSummary Warning(string text) => new("warning", text);
        /**<summary>Cyan BG</summary>*/     public static FormSummary Info(string text) => new("info", text);
        /**<summary>Light BG</summary>*/    public static FormSummary Light(string text) => new("light", text);
        /**<summary>Dark BG</summary>*/     public static FormSummary Dark(string text) => new("dark", text);
    }
}
