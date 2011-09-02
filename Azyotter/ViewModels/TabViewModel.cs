﻿using System.ComponentModel;
using System.Windows.Data;
using Azyobuzi.Azyotter.Models;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class TabViewModel : ViewModel
    {
        public TabViewModel(Tab model)
        {
            this.Model = model;

            var source = new CollectionViewSource();
            source.Source = ViewModelHelper.CreateReadOnlyNotificationDispatcherCollection(
                model.Items,
                item => new TimelineItemViewModel(item),
                DispatcherHelper.UIDispatcher
            );
            source.SortDescriptions.Add(new SortDescription("CreatedAt", ListSortDirection.Descending));
            this.Items = source.View;
        }

        public Tab Model { get; private set; }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public ICollectionView Items { get; private set; }
    }
}
