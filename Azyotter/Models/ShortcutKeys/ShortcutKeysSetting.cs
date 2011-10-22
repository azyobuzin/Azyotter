using System.Collections.Generic;
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
                if (EqualityComparer<ShortcutKey>.Default.Equals(_Post, value))
                    return;
                _Post = value;
                RaisePropertyChanged("Post");
            }
        }
        #endregion

        #region PostWithoutFooter変更通知プロパティ
        private ShortcutKey _PostWithoutFooter = new ShortcutKey() { Ctrl = true, Shift = true, Key = Key.Return };

        public ShortcutKey PostWithoutFooter
        {
            get
            { return _PostWithoutFooter; }
            set
            { 
                if (EqualityComparer<ShortcutKey>.Default.Equals(_PostWithoutFooter, value))
                    return;
                _PostWithoutFooter = value;
                RaisePropertyChanged("PostWithoutFooter");
            }
        }
        #endregion

    }
}
