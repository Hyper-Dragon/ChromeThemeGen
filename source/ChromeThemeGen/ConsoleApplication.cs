using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace ChromeThemeGen
{
    [AutoRegister(RegistrationType.SINGLETON)]
    public sealed class ConsoleApplication : ConsoleApplicationBase
    {
        const int WIDTH = 2560;
        const int THEME_FRAME_OVERLAY_HEIGHT = 40;
        const int THEME_NTP_BACKGROUND_HEIGHT = 1440;
        const int THEME_TOOLBAR_HEIGHT = 120;
        const int COLOUR_GRADIENT_ANGLE = 45;

        const string GENERATOR = "github.com/Hyper-Dragon/ChromeThemeGen";
        const string MANIFEST_TEMPLATE = "ChromeThemeGen.Templates.default.json";

        string baseDirectory = "";
        string imageDirectory = "";
        string manifestFile = "";

        Color colBodyLeft;
        Color colBodyRight;
        Color colBarLeft;
        Color colBarRight;


        string manifestTemplate = "";
        string themeVersionNumber = "0";

        public ConsoleApplication(Parser parser, Header customProcessor, GlobalSettings globalSettings, Helpers helpers) : base(parser, customProcessor, globalSettings, helpers)
        {
            // Load embedded resource MANIFEST_TEMPLATE
            using Stream? resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(MANIFEST_TEMPLATE);

            if (resourceStream != null)
            {
                using StreamReader reader = new(resourceStream);
                manifestTemplate = reader.ReadToEnd();
            }
        }

        protected override async Task<bool> PreRunImplAsync(CommandLineOptions commandLineOptions)
        {
            Debug.Assert(commandLineOptions != null);
            Debug.Assert(commandLineOptions.OutputDirectory != null);

            var result = await Task<bool>.Run(() =>
            {
                // Expand path to base directory
                Console.WriteLine($">> Directory Setup");
                try
                {
                    baseDirectory = Path.GetFullPath(commandLineOptions.OutputDirectory);
                    Console.WriteLine($"   Output Directory: {baseDirectory}");

                    manifestFile = Path.Join(baseDirectory, @"manifest.json");
                    Console.WriteLine($"   Manifest File   : {manifestFile}");

                    imageDirectory = Path.Join(baseDirectory, @"images");
                    Console.WriteLine($"   Image Directory : {imageDirectory}");

                    if (!commandLineOptions.Force && Directory.Exists(baseDirectory))
                    {
                        Console.WriteLine($"   ERROR::Output Directory Already Exists - Please choose a different directory or use --force");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ERROR::Directory setup failed [{ex.Message}]");
                    return false;
                }
                Console.WriteLine($"   DONE");


                // Make sure we have valid colour strings   HelpersStatic.
                Console.Write(">> Parsing Colour Values...");
                try
                {
                    colBodyLeft = ColorTranslator.FromHtml((commandLineOptions!.BodyColLeft ?? "").GetStringWithChar('#'));
                    colBodyRight = ColorTranslator.FromHtml((commandLineOptions.BodyColRight ?? "").GetStringWithChar('#'));
                    colBarLeft = ColorTranslator.FromHtml((commandLineOptions.BarColLeft ?? "").GetStringWithChar('#'));
                    colBarRight = ColorTranslator.FromHtml((commandLineOptions.BarColRight ?? "").GetStringWithChar('#'));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ERROR::Can't Parse Colour Values [{ex.Message}]");
                    return false;
                }
                Console.WriteLine($"   DONE");

                // Calculate the template version number - must be higher than the last generated value
                Console.Write(">> Generating Template Version Number...");
                var verNo = (((int)(DateTime.Now - (new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds)).ToString().PadRight(9, '0');
                themeVersionNumber = $"{int.Parse(verNo[..3])}.{int.Parse(verNo.Substring(3, 3))}.{int.Parse(verNo.Substring(6, 3))}";
                Console.WriteLine($"DONE [{themeVersionNumber}]");

                return true;
            });

            return result;
        }


        protected override async Task<int> RunImplAsync(CommandLineOptions cmdOpts)
        {
            await Task.Run(() =>
            {
                Debug.Assert(OperatingSystem.IsWindows());
                Debug.Assert(cmdOpts != null);
                Debug.Assert(cmdOpts.FontName != null);
                Debug.Assert(cmdOpts.FontSize != null);
                Debug.Assert(cmdOpts.FontSizeBar != null);

                _helpers.StartTimedSection(">> Generating Theme");

                //create empty directory
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                    Directory.CreateDirectory(imageDirectory);
                }

                Console.Write("   Saving theme_frame...");
                SaveBitmap("", WIDTH, 30, new SolidBrush(colBodyLeft), Path.Join(imageDirectory, "theme_frame.png"), cmdOpts.FontName);
                Console.WriteLine("DONE");

                Console.Write("   Saving theme_frame_overlay...");
                System.Drawing.Drawing2D.LinearGradientBrush lgb = new(new Rectangle(0, 0, WIDTH, THEME_FRAME_OVERLAY_HEIGHT + THEME_NTP_BACKGROUND_HEIGHT), colBodyLeft, colBodyRight, COLOUR_GRADIENT_ANGLE);
                var fullBackground = MakeFullBackground(cmdOpts.WatermarkText ?? "", WIDTH, THEME_FRAME_OVERLAY_HEIGHT + THEME_NTP_BACKGROUND_HEIGHT, lgb, new SolidBrush(colBodyRight), cmdOpts.FontName, cmdOpts.FontSize.Value);
                SaveBitmap(fullBackground, 0, THEME_FRAME_OVERLAY_HEIGHT, Path.Join(imageDirectory, "theme_frame_overlay.png"));
                Console.Write("DONE\n   Saving theme_ntp_background.png...");
                SaveBitmap(fullBackground, THEME_FRAME_OVERLAY_HEIGHT, THEME_FRAME_OVERLAY_HEIGHT + THEME_NTP_BACKGROUND_HEIGHT, Path.Join(imageDirectory, "theme_ntp_background.png"));
                Console.WriteLine("DONE");

                Console.Write("   Saving theme_toolbar...");
                System.Drawing.Drawing2D.LinearGradientBrush lgb2 = new(new Rectangle(0, 0, WIDTH, THEME_FRAME_OVERLAY_HEIGHT + THEME_NTP_BACKGROUND_HEIGHT), colBarLeft, colBarRight, COLOUR_GRADIENT_ANGLE);
                var fullBackground2 = MakeFullBackground(cmdOpts.WatermarkText ?? "", WIDTH, THEME_FRAME_OVERLAY_HEIGHT + THEME_NTP_BACKGROUND_HEIGHT, lgb2, new SolidBrush(colBarRight), cmdOpts.FontName, cmdOpts.FontSizeBar.Value, WatermarkImage.WatermarkDirection.TopLeftToBottomRight);
                SaveBitmap(fullBackground2, 0, THEME_TOOLBAR_HEIGHT, Path.Join(imageDirectory, "theme_toolbar.png"));
                Console.Write("DONE\n   Saving theme_ntp_attribution.png...");
                SaveBitmap(GENERATOR, 1024, 25, Brushes.Transparent, Path.Join(imageDirectory, "theme_ntp_attribution.png"), "Arial");
                Console.WriteLine("DONE");

                //Inactive Tab
                Console.Write("   Saving theme_tab_background...");
                SaveBitmap("", WIDTH, 65, new SolidBrush(colBarRight), Path.Join(imageDirectory, "theme_tab_background.png"), cmdOpts.FontName);
                Console.WriteLine("DONE");

                //Set manifest values
                Console.Write("   Writing Manifest File...");
                manifestTemplate = manifestTemplate.Replace("{THEME_NAME}", $"Profile for {cmdOpts.ProfileName}");
                manifestTemplate = manifestTemplate.Replace("\"{TOOLBAR}\"", $"{colBodyLeft.R},{colBodyLeft.G},{colBodyLeft.B}");
                manifestTemplate = manifestTemplate.Replace("\"{TAB_TEXT}\"", $"{colBodyLeft.R},{colBodyLeft.G},{colBodyLeft.B}");
                manifestTemplate = manifestTemplate.Replace("\"{TAB_BACKGROUND_TEXT}\"", $"{colBodyLeft.R},{colBodyLeft.G},{colBodyLeft.B}");
                manifestTemplate = manifestTemplate.Replace("\"{BUTTON_BACKGROUND}\"", $"{colBodyLeft.R},{colBodyLeft.G},{colBodyLeft.B}");
                manifestTemplate = manifestTemplate.Replace("{VERSION}", $"{themeVersionNumber}");
                File.WriteAllText(manifestFile, manifestTemplate, Encoding.UTF8);
                Console.WriteLine("DONE");

                _helpers.EndTimedSection(">> Theme Generation Completed");
            });

            return 0;
        }

        protected override async Task<bool> PostRunImplAsync(CommandLineOptions commandLineOptions)
        {
            await Task.Run(() =>
            {
                Console.WriteLine(">>>> RUN COMPLETE");
            });

            return true;
        }


        private static Bitmap MakeFullBackground(string text, int width, int height, Brush bkgBrush, Brush txtBrush, string fontName, float fontSize, WatermarkImage.WatermarkDirection direction = WatermarkImage.WatermarkDirection.BottomLeftToTopRight)
        {
            Debug.Assert(OperatingSystem.IsWindows());

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);


            graphics.FillRectangle(bkgBrush, 0, 0, bitmap.Width, bitmap.Height);
            WatermarkImage.Watermark(bitmap, direction, txtBrush, text, fontName, fontSize);

            return bitmap;
        }

        private static void SaveBitmap(string text, int width, int height, Brush brush, string filename, string fontName)
        {
            Debug.Assert(OperatingSystem.IsWindows());

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);

            graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawString(text, new Font(fontName, (int)(bitmap.Height * .5)), Brushes.White, new PointF(0, 0));

            bitmap.Save(filename, ImageFormat.Png);
        }

        private static void SaveBitmap(Bitmap srcBitmap, int fromHeight, int toHeight, string filename)
        {
            Debug.Assert(OperatingSystem.IsWindows());

            var destinationBitmap = new Bitmap(srcBitmap.Width, toHeight, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(destinationBitmap);

            //graphics.DrawImage(srcBitmap, 0, fromHeight, srcBitmap.Width, toHeight);
            graphics.DrawImage(srcBitmap, 0, 0, new Rectangle(0, fromHeight, srcBitmap.Width, toHeight), GraphicsUnit.Pixel);
            destinationBitmap.Save(filename, ImageFormat.Png);
        }
    }
}