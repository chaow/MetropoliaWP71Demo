using System.ComponentModel;
using System;

namespace WP71Demo.Model
{
    public class Person : INotifyPropertyChanged
    {
        private string _name = "";
        public string name
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

        // TODO add the coorespoding sets and gets same as the property name
        private string _nickName = "";



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