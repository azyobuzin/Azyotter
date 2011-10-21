using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Azyobuzi.Azyotter.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TabViewModel : ViewModel
    {
        public TabViewModel(Tab model, InteractionMessenger mainWindowViewModelMessenger)
        {
            this.Model = model;
            this.Messenger = mainWindowViewModelMessenger;

            ViewModelHelper.BindNotifyChanged(model, this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Name":
                    case "IsRefreshing":
                        this.RaisePropertyChanged(e.PropertyName);
                        break;
                    case "LastErrorMessage":
                        this.RaisePropertyChanged(e.PropertyName);
                        this.RaisePropertyChanged(() => this.LastErrorMessageIsNotEmpty);
                        break;
                }
            });

            var source = new CollectionViewSource();
            source.Source = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                model.Items,
                (ITimelineItem item) =>
                {
                    var re = new TimelineItemViewModel(item);
                    ViewModelHelper.BindNotifyChanged(re, this, (sender, e) =>
                    {
                        if (e.PropertyName == "IsSelected")
                        {
                            var vm = (TimelineItemViewModel)sender;
                            if (vm.IsSelected)
                                this.SelectedItems.Add(vm);
                            else
                                this.SelectedItems.Remove(vm);
                        }
                    });
                    return re;
                },
                DispatcherHelper.UIDispatcher
            );
            source.SortDescriptions.Clear();
            source.SortDescriptions.Add(new SortDescription("CreatedAt", ListSortDirection.Descending));
            this.Items = source.View;

            this.SelectedItems = new ObservableCollection<TimelineItemViewModel>();
        }

        public Tab Model { get; private set; }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return this.Model.IsRefreshing;
            }
        }

        public ICollectionView Items { get; private set; }

        public ObservableCollection<TimelineItemViewModel> SelectedItems { get; private set; }

        public string LastErrorMessage
        {
            get
            {
                return "タイムラインの取得に失敗しました："
                    + this.Model.LastErrorMessage;
            }
        }
        
        #region ClearErrorMessageCommand
        ViewModelCommand _ClearErrorMessageCommand;

        public ViewModelCommand ClearErrorMessageCommand
        {
            get
            {
                if (_ClearErrorMessageCommand == null)
                    _ClearErrorMessageCommand = new ViewModelCommand(ClearErrorMessage);
                return _ClearErrorMessageCommand;
            }
        }

        private void ClearErrorMessage()
        {
            this.Model.ClearErrorMessage();
        }
        #endregion

        public bool LastErrorMessageIsNotEmpty
        {
            get
            {
                return !string.IsNullOrEmpty(this.Model.LastErrorMessage);
            }
        }

        #region EditCommand
        private ViewModelCommand _EditCommand;

        public ViewModelCommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                {
                    _EditCommand = new ViewModelCommand(Edit);
                }
                return _EditCommand;
            }
        }

        public void Edit()
        {
            this.Messenger.Raise(new TransitionMessage(new TabSettingWindowViewModel(this.Model.Setting), "ShowTabSettingWindow"));
        }
        #endregion

        #region CloseCommand
        private ViewModelCommand _CloseCommand;

        public ViewModelCommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new ViewModelCommand(Close);
                }
                return _CloseCommand;
            }
        }

        public void Close()
        {
            Settings.Instance.Tabs.Remove(this.Model.Setting);
            this.Model.Dispose();
            Settings.Instance.Save();
        }
        #endregion

    }
}
