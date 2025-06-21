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

        /// <summary>
        /// Creates a <see cref="FormSummary"/> instance with a style corresponding to the specified style code.
        /// </summary>
        /// <param name="style">An integer representing the desired style. Valid values are: <list type="bullet"> <item><description>0:
        /// Primary</description></item> <item><description>1: Secondary</description></item> <item><description>2:
        /// Success</description></item> <item><description>3: Danger</description></item> <item><description>4:
        /// Warning</description></item> <item><description>5: Info</description></item> <item><description>6:
        /// Light</description></item> <item><description>7: Dark</description></item> </list> If the value is outside
        /// the valid range, the method defaults to the "Success" style.</param>
        /// <param name="text">The text content to be included in the <see cref="FormSummary"/>.</param>
        /// <returns>A <see cref="FormSummary"/> instance styled according to the specified <paramref name="style"/> and
        /// containing the provided <paramref name="text"/>.</returns>
        public static FormSummary FromCode(int style, string text)
        {
            switch (style)
            {
                case 0: return Primary(text);
                case 1: return Secondary(text);
                case 2: return Success(text);
                case 3: return Danger(text);
                case 4: return Warning(text);
                case 5: return Info(text);
                case 6: return Light(text);
                case 7: return Dark(text);
                default: return Success(text);
            }
        }
    }
}
