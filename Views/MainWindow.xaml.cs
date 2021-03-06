using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        storeEntities store = new storeEntities();
        public ObservableCollection<product> Products { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            // initialize the Inventory View
            ContentSide.Content = new InventoryViews();
            // initialize the Products cart, and it is observable
            Products = new ObservableCollection<product>();
            ProductsListView.DataContext = this;
        }
        // add product to cart by bar code
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
            List<product> productsList = currentProductsItems.OfType<product>().ToList();
            // loop through the cart and check if the product exists, add quantity, else, add the product.
            // check if barcode exists in db products
            product foundProduct = store.products.SingleOrDefault(product => product.bar_code == bar_code);
            if (foundProduct != null)
            {
            }
            else
            {
                // EXIT THE FUNCTION, THE PRODUCT DOESN' EXIST
                MessageBox.Show(("No product with this bar code: " + bar_code));
                return;
            }
            // loop through the list, and if found add 1 to quantity
            // else add the product 
            Boolean productExist = false;
            for (int i = 0; i < productsList.Count; i++)
            {
                // the product is already in cart, so add 1 to quantity
                if (bar_code.ToString().Equals(productsList[i].bar_code.ToString()))
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
            if (!productExist)
            {
                if (updateProductQuantity(addOrRemoveQuantity: -1, productId: foundProduct.id))
                {
                    currentProductsItems.Add(new product { bar_code = bar_code, quantity = 1, price = foundProduct.price, description = foundProduct.description });
                }
                else
                {
                    return;
                }
            }
            UpdateTotal();
        }
        // remove products from cart.
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductsListView.Items.Clear();
            UpdateTotal();
        }
        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
        }
        // print receipt
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            ReciptPrinterView receipt = new ReciptPrinterView();
            receipt.printRecipt(ProductsListView);
        }
        // add or remove new quantity for a product
        // used either from inventory, or after custoemr bought a product
        private bool updateProductQuantity(int addOrRemoveQuantity, int productId)
        {
            var productToBeUpdated = store.products.Find(productId);
            // if therese 0 products in inventory, cannot be bought..
            if (productToBeUpdated.quantity <= 0)
            {
                MessageBox.Show("Quantity for: ( " + productToBeUpdated.description + " ) has a quantity of " + productToBeUpdated.quantity + "\nCannot be added");
                return false;
            }
            productToBeUpdated.quantity += addOrRemoveQuantity;
            store.SaveChanges();
            return true;
        }
        // update the total prices for the cart.
        private void UpdateTotal()
        {
            ItemCollection currentProductsItems = ProductsListView.Items;
            List<product> productsList = currentProductsItems.OfType<product>().ToList();
            decimal total = 0;
            foreach (var product in productsList)
            {
                total += product.subTotal;
            }

            decimal VAT = total * (decimal)0.15;
            decimal totalPriceWithVat = VAT + total;

            Total_TXT.Text = total + " SAR";
            VAT_TXT.Text = total * (decimal)0.15 + " SAR";
            Total_Price_TXT.Text = totalPriceWithVat + " SAR";
        }
    }
}
