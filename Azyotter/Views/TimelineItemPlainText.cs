using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Azyobuzi.Azyotter.Views
{
    public class TimelineItemPlainText : TextBlock
    {
        public TimelineItemPlainText()
        {
            this.TextWrapping = TextWrapping.Wrap;
            this.Margin = new Thickness(5, 0, 5, 0);
        }

        public new IEnumerable<Inline> Inlines
        {
            get { return (IEnumerable<Inline>)GetValue(InlinesProperty); }
            set { SetValue(InlinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Inlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.Register("Inlines", typeof(IEnumerable<Inline>), typeof(TimelineItemPlainText), new UIPropertyMetadata(null, (senderObj, e) =>
            {
                var sender = (TextBlock)senderObj;

                sender.Inlines.Clear();

                if (e.NewValue != null)
                    sender.Inlines.AddRange(e.NewValue as IEnumerable<Inline>);
            }));
    }
}
