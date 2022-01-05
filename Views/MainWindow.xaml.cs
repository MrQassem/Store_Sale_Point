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
using WpfApp1.Views;

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

        ReciptPrinterView r = new ReciptPrinterView();


        public MainWindow()
        {
            InitializeComponent();
            ProductsListView.Items.Add((new product { id = 1, description = "test", bar_code = "t", price = 10, quantity = 1 }));
            // initialize the Products cart, and it is observable
            Products = new ObservableCollection<product>();
            ProductsListView.DataContext = this;
        }

       
        
        // when the bar
        private void AddProductBtn(object sender, RoutedEventArgs e)
        {
            string bar_code = Bar_Code_TXT.Text.ToString();
            // if empty bar code, stop the function.
            if (bar_code.Length == 0)
            {
                MessageBox.Show("Bar code cannot be empty!");
                return;
            }

            ItemCollection currentProductsItems = ProductsListView.Items;
            List<product> productsList = currentProductsItems.OfType< product>().ToList();
            // loop through the cart and check if the product exists, add quantity, else, add the product.
            // check if barcode exists in db products
            product foundProduct = store.products.SingleOrDefault(product => product.bar_code == bar_code);
            if (foundProduct != null)
            {
                Console.WriteLine("product found, price is  : " + foundProduct.price);
            }
            else
            {
                MessageBox.Show(("No product with this bar code: " + bar_code));
                // EXIT THE FUNCTION, THE PRODUCT DOESN' EXIST
                return;
            }
            Boolean productExist = false;
            for (int i = 0; i < productsList.Count; i++)
            {
                // the product is already in cart, so add 1 to quantity
                if (bar_code.ToString().Equals( productsList[i].bar_code.ToString()))
                {
                    if (updateProductQuantity(addOrRemoveQuantity: -1, productId: foundProduct.id))
                    {
                        productExist = true;
                        productsList[i].quantity++;
                        currentProductsItems.Remove(productsList[i]);
                        currentProductsItems.Add(productsList[i]);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if(!productExist)
            {
                if (updateProductQuantity(addOrRemoveQuantity: -1, productId: foundProduct.id))
                {
                    currentProductsItems.Add(new product { bar_code = bar_code, quantity = 1, price = foundProduct.price, description = foundProduct.description});
                }
                else
                {
                    return;
                }
            }
            UpdateTotal();
        }
        private bool updateProductQuantity(int addOrRemoveQuantity, int productId)
        {
            var productToBeUpdated = store.products.Find(productId);
            if (productToBeUpdated.quantity<=0)
            {
                MessageBox.Show("Quantity for: ( " + productToBeUpdated.description + " ) has a quantity of " + productToBeUpdated.quantity + "\nCannot be added");
                return false;
            }
            productToBeUpdated.quantity+= addOrRemoveQuantity;
            store.SaveChanges();
            return true;
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
            Total_TXT.Text = total + " SAR";
            VAT_TXT.Text = total * (decimal)0.15 + " SAR";
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            r.printRecipt(ProductsListView);

            //{ System.Windows.Controls.ListView Items.Count: 2}
            //r.IsEnabled = true;
            //r.IsEnabled = false;

            //ProductsListView.Items.Clear();
            //UpdateTotal();


        }


        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentSide.Content= new InventoryViews();
            

        }
    }
}
