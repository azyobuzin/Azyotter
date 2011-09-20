using System;
using Livet;

namespace Azyobuzi.Azyotter.Models.TwitterDataModels
{
    public class User : NotificationObject
    {
        
        #region Id変更通知プロパティ
        string _Id;

        public string Id
        {
            get
            { return _Id; }
            set
            {
                if (_Id == value)
                    return;
                _Id = value;
                RaisePropertyChanged("Id");
            }
        }
        #endregion
        
        #region ScreenName変更通知プロパティ
        string _ScreenName;

        public string ScreenName
        {
            get
            { return _ScreenName; }
            set
            {
                if (_ScreenName == value)
                    return;
                _ScreenName = value;
                RaisePropertyChanged("ScreenName");
            }
        }
        #endregion
        
        #region Name変更通知プロパティ
        string _Name;

        public string Name
        {
            get
            { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion
        
        #region CreatedAt変更通知プロパティ
        DateTime _CreatedAt;

        public DateTime CreatedAt
        {
            get
            { return _CreatedAt; }
            set
            {
                if (_CreatedAt == value)
                    return;
                _CreatedAt = value;
                RaisePropertyChanged("CreatedAt");
            }
        }
        #endregion
        
        #region Description変更通知プロパティ
        string _Description;

        public string Description
        {
            get
            { return _Description; }
            set
            {
                if (_Description == value)
                    return;
                _Description = value;
                RaisePropertyChanged("Description");
            }
        }
        #endregion
        
        #region Location変更通知プロパティ
        string _Location;

        public string Location
        {
            get
            { return _Location; }
            set
            {
                if (_Location == value)
                    return;
                _Location = value;
                RaisePropertyChanged("Location");
            }
        }
        #endregion
        
        #region Url変更通知プロパティ
        string _Url;

        public string Url
        {
            get
            { return _Url; }
            set
            {
                if (_Url == value)
                    return;
                _Url = value;
                RaisePropertyChanged("Url");
            }
        }
        #endregion
        
        #region FriendsCount変更通知プロパティ
        ulong _FriendsCount;

        public ulong FriendsCount
        {
            get
            { return _FriendsCount; }
            set
            {
                if (_FriendsCount == value)
                    return;
                _FriendsCount = value;
                RaisePropertyChanged("FriendsCount");
            }
        }
        #endregion
        
        #region FollowersCount変更通知プロパティ
        ulong _FollowersCount;

        public ulong FollowersCount
        {
            get
            { return _FollowersCount; }
            set
            {
                if (_FollowersCount == value)
                    return;
                _FollowersCount = value;
                RaisePropertyChanged("FollowersCount");
            }
        }
        #endregion
        
        #region StatusesCount変更通知プロパティ
        ulong _StatusesCount;

        public ulong StatusesCount
        {
            get
            { return _StatusesCount; }
            set
            {
                if (_StatusesCount == value)
                    return;
                _StatusesCount = value;
                RaisePropertyChanged("StatusesCount");
            }
        }
        #endregion
        
        #region FavoritesCount変更通知プロパティ
        ulong _FavoritesCount;

        public ulong FavoritesCount
        {
            get
            { return _FavoritesCount; }
            set
            {
                if (_FavoritesCount == value)
                    return;
                _FavoritesCount = value;
                RaisePropertyChanged("FavoritesCount");
            }
        }
        #endregion
        
        #region ListedCount変更通知プロパティ
        int _ListedCount;

        public int ListedCount
        {
            get
            { return _ListedCount; }
            set
            {
                if (_ListedCount == value)
                    return;
                _ListedCount = value;
                RaisePropertyChanged("ListedCount");
            }
        }
        #endregion
      
        #region Protected変更通知プロパティ
        bool _Protected;

        public bool Protected
        {
            get
            { return _Protected; }
            set
            {
                if (_Protected == value)
                    return;
                _Protected = value;
                RaisePropertyChanged("Protected");
            }
        }
        #endregion
        
        #region ProfileImageUrl変更通知プロパティ
        string _ProfileImageUrl;

        public string ProfileImageUrl
        {
            get
            { return _ProfileImageUrl; }
            set
            {
                if (_ProfileImageUrl == value)
                    return;
                _ProfileImageUrl = value;
                RaisePropertyChanged("ProfileImageUrl");
            }
        }
        #endregion
      
    }
}
