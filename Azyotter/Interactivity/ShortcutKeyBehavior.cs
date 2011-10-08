using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Azyobuzi.Azyotter.Models.ShortcutKeys;

namespace Azyobuzi.Azyotter.Interactivity
{
    public class ShortcutKeyBehavior : Behavior<UIElement>
    {
        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == this.ShortcutKey.Key)
            {
                ModifierKeys modif = ModifierKeys.None;
                if (this.ShortcutKey.Ctrl)
                    modif |= ModifierKeys.Control;
                if (this.ShortcutKey.Shift)
                    modif |= ModifierKeys.Shift;
                if (this.ShortcutKey.Alt)
                    modif |= ModifierKeys.Alt;

                if (e.KeyboardDevice.Modifiers == modif && this.Command.CanExecute(e))
                {
                    e.Handled = true;
                    this.Command.Execute(e);
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewKeyDown += this.PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewKeyDown -= this.PreviewKeyDown;

            base.OnDetaching();
        }

        public ShortcutKey ShortcutKey
        {
            get { return (ShortcutKey)GetValue(ShortcutKeyProperty); }
            set { SetValue(ShortcutKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShortcutKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShortcutKeyProperty =
            DependencyProperty.Register("ShortcutKey", typeof(ShortcutKey), typeof(ShortcutKeyBehavior));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ShortcutKeyBehavior));

        
    }
}
