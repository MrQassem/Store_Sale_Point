using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for InventoryViews.xaml
    /// </summary>
    public partial class InventoryViews : UserControl
    {
        storeEntities store = new storeEntities();

        public InventoryViews()
        {
            InitializeComponent();

            InventoryDataGrid.ItemsSource = store.products.ToList().Select(product => 
                new { product.description,
                    product.bar_code,
                    product.price,
                    product.quantity
                }
            ).ToList();
            //InventoryDataGrid.Columns[5].Visibility = Visibility.Collapsed;

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            store.SaveChanges();
        }

        
    }
}
