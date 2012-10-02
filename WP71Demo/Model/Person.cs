using System.ComponentModel;
using System;

namespace WP71Demo.Model
{
    public class Person : INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("personName");
                }
            }
            get
            {
                return _name;
            }
        }

        private string _nickName = "";
        public string Nickname
        {
            set
            {
                if (value != _nickName)
                {
                    _nickName = value;
                    NotifyPropertyChanged("personName");
                }
            }
            get
            {
                return _nickName;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}