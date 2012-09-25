using System.Collections.ObjectModel;
using WP71Demo.Model;

namespace WP71Demo.ViewModel
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
        }

        private ObservableCollection<Person> _itemViewModel = null;
        public ObservableCollection<Person> ItemViewModel
        {
            get 
            {
                if (_itemViewModel == null)
                {
                    _itemViewModel = new ObservableCollection<Person>();
                    LoadData();
                }
                return _itemViewModel;
            }
        }

        private void LoadData()
        {
            if (_itemViewModel.Count > 0)
            {
                _itemViewModel.Clear();
            }
            for (int i = 0; i < 10; i++)
            {
                Person p = new Person();
                p.Name = "Person " + i;
                p.Nickname = "Person nickname " + i;
                _itemViewModel.Add(p);
            }
        }
    }
}
