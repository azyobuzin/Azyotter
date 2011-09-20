using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Azyobuzi.Azyotter.Views
{
    public class TimelineItemRichText : RichTextBox
    {
        public TimelineItemRichText()
        {
            this.IsReadOnly = true;
            this.BorderThickness = new Thickness(0d);
            this.Background = Brushes.Transparent;
        }

        public IEnumerable<Inline> Inlines
        {
            get { return (IEnumerable<Inline>)GetValue(InlinesProperty); }
            set { SetValue(InlinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Inlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.Register("Inlines", typeof(IEnumerable<Inline>), typeof(TimelineItemRichText), new UIPropertyMetadata(null, (sender, e) =>
            {
                if (e.NewValue == null)
                    ((TimelineItemRichText)sender).Document = null;

                var paragraph = new Paragraph();
                paragraph.Inlines.AddRange(e.NewValue as IEnumerable<Inline>);
                ((TimelineItemRichText)sender).Document = new FlowDocument(paragraph);
            }));
    }
}
