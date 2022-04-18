namespace ChromeThemeGen
{
    [AutoRegister(RegistrationType.SINGLETON)]
    public sealed class Header : HeaderBase
    {
        public Header(GlobalSettings globalSettings) : base(globalSettings) { }

        protected override string[] DisplayTitleImpl()
        {
            string[] headLines = {@"                      ____ _                                                  ",
                                  @"                     / ___| |__  _ __ ___  _ __ ___   ___                     ",
                                  @"                    | |   | '_ \| '__/ _ \| '_ ` _ \ / _ \                    ",
                                  @"                    | |___| | | | | | (_) | | | | | |  __/                    ",
                                  @"                     \____|_| |_|_|  \___/|_| |_| |_|\___|                    ",
                                  @"                 _____ _                          ____                        ",
                                  @"                |_   _| |__   ___ _ __ ___   ___ / ___| ___ _ __              ",
                                  @"                  | | | '_ \ / _ \ '_ ` _ \ / _ \ |  _ / _ \ '_ \             ",
                                  @"                  | | | | | |  __/ | | | | |  __/ |_| |  __/ | | |            ",
                                  @"                  |_| |_| |_|\___|_| |_| |_|\___|\____|\___|_| |_|            ",
                                  @"                                                                              ",
                                  @"          ** Theme Generator - github.com/Hyper-Dragon/ChromeThemeGen **      ",
                                  @"                                                                              "};


            /*
                                              


              */

            return headLines;
        }
    }
}


