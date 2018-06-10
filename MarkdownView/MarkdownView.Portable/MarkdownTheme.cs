
namespace Markdown
{
    using Xamarin.Forms;

    public class MarkdownTheme
    {
        public MarkdownStyle Paragraph { get; set; } 

        public MarkdownStyle Heading1 { get; set; } 

        public MarkdownStyle Heading2 { get; set; } 

        public MarkdownStyle Heading3 { get; set; } 

        public MarkdownStyle Heading4 { get; set; } 

        public MarkdownStyle Heading5 { get; set; }

        public MarkdownStyle Heading6 { get; set; }

        public MarkdownStyle Quote { get; set; }

        public MarkdownStyle Separator { get; set; }

        public MarkdownStyle Link { get; set; }

        public MarkdownStyle Code { get; set; } 

    }

    public class LightMarkdownTheme : MarkdownTheme
    {
        public LightMarkdownTheme()
        {
            this.Paragraph = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                ForegroundColor = DefaultTextColor,
                FontSize = 12,
            };

            this.Heading1 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                BorderSize = 1,
                BorderColor = DefaultSeparatorColor,
                FontSize = 26,
            };

            this.Heading2 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                BorderSize = 1,
                BorderColor = DefaultSeparatorColor,
                FontSize = 22,
            };

            this.Heading3 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                FontSize = 20,
            };

            this.Heading4 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                FontSize = 18,
            };

            this.Heading5 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                FontSize = 16,
            };

            this.Heading6 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                ForegroundColor = DefaultTextColor,
                FontSize = 14,
            };

            this.Link = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                ForegroundColor = DefaultAccentColor,
                FontSize = 12,
            };

            this.Code = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                BackgroundColor = DefaultCodeBackground,
                FontSize = 12,
            };

            this.Quote = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                ForegroundColor = DefaultQuoteTextColor,
                BorderSize = 4,
                BorderColor = DefaultQuoteBorderColor,
                FontSize = 12,
            };

            this.Separator = new MarkdownStyle
            {
                BorderSize = 2,
                BorderColor = DefaultSeparatorColor,
            };

            // Platform specific properties
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Code.FontFamily = "Courier";
                    break;

                case Device.Android:
                    Code.FontFamily = "monospace";
                    break;
            }
        }

        public static readonly Color DefaultAccentColor = Color.FromHex("#0366d6");

        public static readonly Color DefaultTextColor = Color.FromHex("#24292e");

        public static readonly Color DefaultCodeBackground = Color.FromHex("#f6f8fa");

        public static readonly Color DefaultSeparatorColor = Color.FromHex("#eaecef");

        public static readonly Color DefaultQuoteTextColor = Color.FromHex("#6a737d");

        public static readonly Color DefaultQuoteBorderColor = Color.FromHex("#dfe2e5");
    }
}
