using System;
using System.ComponentModel;

namespace WP71Demo.Model
{
    public class Person : MobelBase
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

        private int _age = 0;
        public int Age
        {
            set
            {
                if (value != _age)
                {
                    _age = value;
                    NotifyPropertyChanged("personAge");
                }
            }
            get
            {
                return _age;
            }
        }

    }
}
