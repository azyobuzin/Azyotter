using System;
using Azyobuzi.Azyotter.Models;
using Livet;

namespace Azyobuzi.Azyotter.ViewModels
{
    public class AccountViewModel : ViewModel
    {
        public AccountViewModel(Account model)
        {
            this.Model = model;

            ViewModelHelper.BindNotifyChanged(model, this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "ScreenName":
                        this.RaisePropertyChanged(() => this.ScreenName);
                        this.RaisePropertyChanged(() => this.ProfileImageUri);
                        break;
                    case "UserId":
                        this.RaisePropertyChanged(() => this.UserId);
                        break;
                }
            });
        }

        public Account Model { get; private set; }

        public string ScreenName
        {
            get
            {
                return this.Model.ScreenName;
            }
        }

        public Uri ProfileImageUri
        {
            get
            {
                return new Uri(string.Format("http://api.twitter.com/1/users/profile_image?screen_name={0}", this.ScreenName));
            }
        }

        public long UserId
        {
            get
            {
                return this.Model.UserId;
            }
        }
    }
}
