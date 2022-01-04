using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ProductListingViewModel : INotifyPropertyChanged
    {
        storeEntities store = new storeEntities();
        //private List<product> products ;
        public List<product> products { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ProductListingViewModel()
        {
            products = store.products.ToList();
        }
    }
}
