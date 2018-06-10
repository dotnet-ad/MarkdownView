namespace Markdown
{
    using System.Linq;
    using Markdig.Syntax;
    using Markdig.Syntax.Inlines;
    using Xamarin.Forms;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class MarkdownView : ContentView
    {
        public static MarkdownTheme Global = new LightMarkdownTheme();

        public string Markdown
        {
            get { return (string)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(nameof(Markdown), typeof(string), typeof(MarkdownView), null, propertyChanged: OnMarkdownChanged);

        public MarkdownTheme Theme
        {
            get { return (MarkdownTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly BindableProperty ThemeProperty = BindableProperty.Create(nameof(Theme), typeof(MarkdownTheme), typeof(MarkdownView), Global, propertyChanged: OnMarkdownChanged);

        private bool isQuoted;

        private List<View> queuedViews = new List<View>();

        static void OnMarkdownChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as MarkdownView;
            view.RenderMarkdown();
        }

        private StackLayout stack;

        private void RenderMarkdown()
        {
            var parsed = Markdig.Markdown.Parse(this.Markdown);

            stack = new StackLayout()
            {
                Margin = 10.0f,
            };

            this.Render(parsed.AsEnumerable());

            this.Content = stack;
        }

        private void Render(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                this.Render(block);
            }
        }

        #region Rendering blocks

        private void Render(Block block)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    Render(heading);
                    break;

                case ParagraphBlock paragraph:
                    Render(paragraph);
                    break;

                case QuoteBlock quote:
                    Render(quote);
                    break;

                case CodeBlock code:
                    Render(code);
                    break;

                case ListBlock list:
                    Render(list);
                    break;

                case ThematicBreakBlock thematicBreak:
                    Render(thematicBreak);
                    break;

                default:
                    Debug.WriteLine($"Can't render {block.GetType()} blocks.");
                    break;
            }

            if(queuedViews.Any())
            {
                foreach (var view in queuedViews)
                {
                    this.stack.Children.Add(view);
                }
                queuedViews.Clear();
            }
        }

        private int listScope;

        private void Render(ThematicBreakBlock block)
        {
            var style = this.Theme.Separator;

            if (style.BorderSize > 0)
            {
                stack.Children.Add(new BoxView
                {
                    HeightRequest = style.BorderSize,
                    BackgroundColor = style.BorderColor,
                });
            }
        }

        private void Render(ListBlock block)
        {
            listScope++;

            for (int i = 0; i < block.Count(); i++)
            {
                var item = block.ElementAt(i);

                if (item is ListItemBlock itemBlock)
                {
                    this.Render(block, i + 1, itemBlock);
                }
            }

            listScope--;
        }

        private void Render(ListBlock parent, int index, ListItemBlock block)
        {
            var initialStack = this.stack;

            this.stack = new StackLayout();

            this.Render(block.AsEnumerable());

            var horizontalStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(listScope * 10, 0, 0, 0),
            };

            View bullet;

            if (parent.IsOrdered)
            {
                bullet = new Label
                {
                    Text = $"{index}.",
                    FontSize = this.Theme.Paragraph.FontSize,
                    TextColor = this.Theme.Paragraph.ForegroundColor,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                };
            }
            else
            {
                bullet = new BoxView
                {
                    WidthRequest = 4,
                    HeightRequest = 4,
                    Margin = new Thickness(0, 6, 0, 0),
                    BackgroundColor = this.Theme.Paragraph.ForegroundColor,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center,
                };
            }

            horizontalStack.Children.Add(bullet);


            horizontalStack.Children.Add(this.stack);
            initialStack.Children.Add(horizontalStack);

            this.stack = initialStack;
        }

        private void Render(HeadingBlock block)
        {
            MarkdownStyle style;

            switch (block.Level)
            {
                case 1:
                    style = this.Theme.Heading1;
                    break;
                case 2:
                    style = this.Theme.Heading2;
                    break;
                case 3:
                    style = this.Theme.Heading3;
                    break;
                case 4:
                    style = this.Theme.Heading4;
                    break;
                case 5:
                    style = this.Theme.Heading5;
                    break;
                default:
                    style = this.Theme.Heading6;
                    break;
            }

            var foregroundColor = isQuoted ? this.Theme.Quote.ForegroundColor : style.ForegroundColor;

            stack.Children.Add(new Label
            {
                FormattedText = CreateFormatted(block.Inline, style.FontFamily, style.Attributes, foregroundColor, style.BackgroundColor, style.FontSize),
            });

            if (style.BorderSize > 0)
            {
                stack.Children.Add(new BoxView
                {
                    HeightRequest = style.BorderSize,
                    BackgroundColor = style.BorderColor,
                });
            }
        }

        private void Render(ParagraphBlock block)
        {
            var style = this.Theme.Paragraph;
            var foregroundColor = isQuoted ? this.Theme.Quote.ForegroundColor : style.ForegroundColor;
            var label = new Label
            {
                FormattedText = CreateFormatted(block.Inline, style.FontFamily, style.Attributes, foregroundColor, style.BackgroundColor, style.FontSize),
            };
            this.stack.Children.Add(label);
        }

        private void Render(QuoteBlock block)
        {
            var initialIsQuoted = this.isQuoted;
            var initialStack = this.stack;

            this.isQuoted = true;
            this.stack = new StackLayout();

            var style = this.Theme.Quote;

            if (style.BorderSize > 0)
            {
                var horizontalStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                };

                horizontalStack.Children.Add(new BoxView()
                {
                    WidthRequest = style.BorderSize,
                    BackgroundColor = style.BorderColor,
                });

                horizontalStack.Children.Add(this.stack);
                initialStack.Children.Add(horizontalStack);
            }
            else
            {
                initialStack.Children.Add(this.stack);
            }

            this.Render(block.AsEnumerable());

            this.isQuoted = initialIsQuoted;
            this.stack = initialStack;
        }

        private void Render(CodeBlock block)
        {
            var style = this.Theme.Code;
            var label = new Label
            {
                Margin = 10,
                TextColor = style.ForegroundColor,
                FontAttributes = style.Attributes,
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                Text = string.Join(Environment.NewLine, block.Lines),
            };
            stack.Children.Add(new ContentView()
            {
                BackgroundColor = style.BackgroundColor,
                Content = label
            });
        }

        private FormattedString CreateFormatted(ContainerInline inlines, string family, FontAttributes attributes, Color foregroundColor, Color backgroundColor, float size)
        {
            var fs = new FormattedString();

            foreach (var inline in inlines)
            {
                var spans = CreateSpans(inline, family, attributes, foregroundColor, backgroundColor, size);
                if (spans != null)
                {
                    foreach (var span in spans)
                    {
                        fs.Spans.Add(span);
                    }
                }
            }

            return fs;
        }

        private Span[] CreateSpans(Inline inline, string family, FontAttributes attributes, Color foregroundColor, Color backgroundColor, float size)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    return new[]
                    {
                        new Span
                        {
                            Text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length),
                            FontAttributes = attributes,
                            ForegroundColor = foregroundColor,
                            BackgroundColor = backgroundColor,
                            FontSize = size,
                        }
                    };

                case EmphasisInline emphasis:
                    var childAttributes = attributes | (emphasis.IsDouble ? FontAttributes.Bold : FontAttributes.Italic);
                    return emphasis.SelectMany(x => CreateSpans(x, family, childAttributes, foregroundColor, backgroundColor, size)).ToArray();

                case LineBreakInline breakline:
                    return new[] { new Span { Text = "\n" } };

                case LinkInline link:

                    if(link.IsImage)
                    {
                        queuedViews.Add(new Image()
                        {
                            Source = link.Url,
                        });
                        return new Span[0];
                    }
                    else
                    {
                        return link.SelectMany(x => CreateSpans(x, family, attributes, this.Theme.Link.ForegroundColor, backgroundColor, size)).ToArray();
                    }

                case CodeInline code:
                    return new[]
                    {
                        new Span
                        {
                            Text = code.Content,
                            FontAttributes = this.Theme.Code.Attributes,
                            FontSize = size,
                            FontFamily = this.Theme.Code.FontFamily,
                            ForegroundColor = this.Theme.Code.ForegroundColor,
                            BackgroundColor = this.Theme.Code.BackgroundColor
                        }
                    };


                default:
                    Debug.WriteLine($"Can't render {inline.GetType()} inlines.");
                    return null;
            }
        }

        #endregion
    }
}
