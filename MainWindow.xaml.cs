using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        storeEntities store = new storeEntities();
        public ProductListingViewModel  ProductListing { get; set; }
        public ObservableCollection<product> Products{ get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ProductsListView.Items.Add((new product { id = 1, description = "test", bar_code = "t", price = 10, quantity = 1 }));
            // initialize the Products cart, and it is observable
            Products = new ObservableCollection<product>();
            ProductsListView.DataContext = this;
        }

       
        

        private void AddProductBtn(object sender, RoutedEventArgs e)
        {
            string description = Bar_Code_TXT.Text.ToString();
            ItemCollection currentProductsItems = ProductsListView.Items;
            List<product> productsList = currentProductsItems.OfType< product>().ToList();
            // loop through the cart and check if the product exists, add quantity, else, add the product.
            Boolean productExist = false;
            for (int i = 0; i < productsList.Count  ; i++)
            {
                // the product is already in cart, so add 1 to quantity
                if (description.ToString().Equals( productsList[i].description.ToString()))
                {
                    productExist = true;
                    productsList[i].quantity++;
                    currentProductsItems.Remove(productsList[i]);
                    currentProductsItems.Add(productsList[i]);
                }
                Console.WriteLine(productsList[i].description);
                Console.WriteLine(productsList[i].quantity);
            }
            if(!productExist)
            {
                currentProductsItems.Add(new product { bar_code = "1111", quantity = 1, price = 111, description = description });
            }
            UpdateTotal();
            Console.WriteLine("productsList: " + productsList.Count);
            Console.WriteLine("currentProductsItems: " + currentProductsItems.Count);
            Console.WriteLine("currentProductsItems: " + currentProductsItems.Count);
            
        }
        private void UpdateTotal()
        {
            ItemCollection currentProductsItems = ProductsListView.Items;
            List<product> productsList = currentProductsItems.OfType<product>().ToList();
            decimal total = 0;
            foreach (var product in productsList)
            {
                total += product.subTotal;
            }
                Console.WriteLine(total);
            Total_TXT.Text = total + " SAR";
            VAT_TXT.Text = total * (decimal)0.15 + " SAR";
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            //ItemCollection currentProductsItems = ProductsListView.Items;
            ProductsListView.Items.Clear();
            UpdateTotal();
        }
    }
}
