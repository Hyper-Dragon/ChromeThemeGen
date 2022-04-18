namespace ChromeThemeGen
{
    public sealed class CommandLineOptions : CommandLineOptionsBase
    {
        //[Value(index: 0, Required = true, HelpText = "Image file Path to analyze.")]
        //public string? Path { get; set; } = "blah";

        //[Option(shortName: 'c', longName: "confidence", Required = false, HelpText = "Minimum confidence.", Default = 0.9f)]
        //public float Confidence { get; set; }



        [Option(longName: "body-col-left", Required = false, HelpText = "Left Side Body Colour (Hex Value #)", Default = "#A83A03")]
        public string? BodyColLeft { get; set; }

        [Option(longName: "body-col-right", Required = false, HelpText = "Right Side Body Colour (Hex Value #)", Default = "#FF6D24")]
        public string? BodyColRight { get; set; }

        [Option(longName: "bar-col-left", Required = false, HelpText = "Left Side Bar Colour (Hex Value #)", Default = "#00A89D")]
        public string? BarColLeft { get; set; }

        [Option(longName: "bar-col-right", Required = false, HelpText = "Right Side Bar Colour (Hex Value #)", Default = "#0AF5E4")]
        public string? BarColRight { get; set; }

        [Option(shortName: 'n', longName: "profile-name", Required = false, HelpText = "Profile Name", Default = "Demo")]
        public string? ProfileName { get; set; }

        [Option(shortName: 'w', longName: "profile-text", Required = false, HelpText = "Text Watermark", Default = "DEMO")]
        public string? WatermarkText { get; set; }

        [Option(shortName: 'o', longName: "output-directory", Required = false, HelpText = "Output Directory", Default = @".\DemoTheme")]
        public string? OutputDirectory { get; set; }

        [Option(shortName: 'f', longName: "force", Required = false, HelpText = "Overwrite output directory", Default = false)]
        public bool Force { get; set; }

        [Option(longName: "font", Required = false, HelpText = "The font to use", Default = "Arial")]
        public string? FontName { get; set; }

        [Option(longName: "font-size", Required = false, HelpText = "The font to use for the body", Default = 14f)]
        public float? FontSize { get; set; }
        
        [Option(longName: "font-size-bar", Required = false, HelpText = "The font to use for the top bar", Default = 14f)]
        public float? FontSizeBar { get; set; }
    }
}