using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Azyobuzi.Azyotter.Interactivity
{
    //http://d.hatena.ne.jp/griefworker/20100217/silverlight_listbox_selecteditems
    public static class ListBoxBehavior
    {
        private static readonly DependencyProperty SelectedItemsBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItemsBehavior",
                typeof(SelectedItemsBehavior),
                typeof(ListBox),
                null);

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(ListBoxBehavior),
                new PropertyMetadata(null, ItemsPropertyChanged));

        public static void SetSelectedItems(ListBox listBox, IList list)
        {
            listBox.SetValue(SelectedItemsProperty, list);
        }

        public static IList GetSelectedItems(ListBox listBox)
        {
            return listBox.GetValue(SelectedItemsProperty) as IList;
        }

        private static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ListBox;
            if (target != null)
            {
                GetOrCreateBehavior(target, e.NewValue as IList);
            }
        }

        private static SelectedItemsBehavior GetOrCreateBehavior(ListBox target, IList list)
        {
            var behavior = target.GetValue(SelectedItemsBehaviorProperty) as SelectedItemsBehavior;
            if (behavior == null)
            {
                behavior = new SelectedItemsBehavior(target, list);
                target.SetValue(SelectedItemsBehaviorProperty, behavior);
            }

            return behavior;
        }

        private class SelectedItemsBehavior
        {
            private readonly ListBox _listBox;
            private readonly IList _boundList;
            private bool _listBoxSelectionChanging = false;
            private bool _collectionChanging = false;

            public SelectedItemsBehavior(ListBox listBox, IList boundList)
            {
                _boundList = boundList;
                if (_boundList is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)_boundList).CollectionChanged += SelectedItemsBehavior_CollectionChanged;
                }

                _listBox = listBox;
                _listBox.SelectionChanged += OnSelectionChanged;
            }

            private void SelectedItemsBehavior_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (_listBoxSelectionChanging == false)
                {
                    _collectionChanging = true;

                    _listBox.SelectedItems.Clear();
                    foreach (var item in _boundList)
                    {
                        _listBox.SelectedItems.Add(item);
                    }

                    _collectionChanging = false;
                }
            }

            private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (_collectionChanging == false)
                {
                    _listBoxSelectionChanging = true;

                    _boundList.Clear();
                    foreach (var item in _listBox.SelectedItems)
                    {
                        _boundList.Add(item);
                    }

                    _listBoxSelectionChanging = false;
                }
            }
        }
    }
}
