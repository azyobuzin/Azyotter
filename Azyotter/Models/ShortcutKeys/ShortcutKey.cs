using System.Linq;
using System.Windows.Input;

namespace Azyobuzi.Azyotter.Models.ShortcutKeys
{
    public struct ShortcutKey
    {
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public Key Key { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ShortcutKey))
                return false;

            var other = (ShortcutKey)obj;
            return this.Ctrl == other.Ctrl
                && this.Shift == other.Shift
                && this.Alt == other.Alt
                && this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return (int)this.Key +
                (int)new[]
                {
                    this.Ctrl.GetHashCode(),
                    this.Shift.GetHashCode(),
                    this.Alt.GetHashCode()
                }
                .Average();
        }

        public static bool operator ==(ShortcutKey x, ShortcutKey y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ShortcutKey x, ShortcutKey y)
        {
            return !x.Equals(y);
        }
    }
}
