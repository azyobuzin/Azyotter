using System.Windows.Input;
using Livet;

namespace Azyobuzi.Azyotter.Models.ShortcutKeys
{
    public class ShortcutKeysSetting : NotificationObject
    {
        
        #region Post変更通知プロパティ
        ShortcutKey _Post = new ShortcutKey() { Ctrl = true, Key = Key.Return };

        public ShortcutKey Post
        {
            get
            { return _Post; }
            set
            {
                if (_Post == value)
                    return;
                _Post = value;
                RaisePropertyChanged("Post");
            }
        }
        #endregion
      
    }
}
