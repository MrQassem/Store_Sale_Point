using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for InventoryViews.xaml
    /// </summary>
    public partial class InventoryViews : UserControl
    {
        storeEntities store = new storeEntities();
        public ObservableCollection<product> Products { get; set; }
        public InventoryViews()
        {
            InitializeComponent();
            InventoryDataGrid.ItemsSource = getProductsList();
            Products = new ObservableCollection<product>();
            InventoryDataGrid.DataContext = this;

        }
        private IEnumerable getProductsList()
        {
            return store.products.ToList().Select(product => 
                new {
                    product.id,
                    product.description,
                    product.bar_code,
                    product.price,
                    product.quantity
                }
            ).ToList();
        }

        // save the changed fields for the inventory
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            product selectedProduct = getSelectedProduct();
            // check there's list item selected, then show error message box
            // in case missing values or incorrect values.
            if(selectedProduct != null)
            {
                product productToBeUpdated = store.products.Find(selectedProduct.id);
                try
                {
                productToBeUpdated.description = Description_TXT.Text ;
                productToBeUpdated.bar_code = Bar_Code_TXT.Text;
                productToBeUpdated.price = Decimal.Parse (Price_TXt.Text);
                productToBeUpdated.quantity = Int32.Parse( Quantity_TXt.Text );
                store.SaveChanges();
                MessageBox.Show("Changes Saved");
                UpdateInventoryDataGrid();
                }
                catch
                {
                    MessageBox.Show("price and quantity is numbers only!");
                }
            }
        }
        private void AddNewProductBtn_Click(object sender, RoutedEventArgs e)
        {
            // check there's no product with the given bar code
            try
            {
                product isProductExist = store.products.FirstOrDefault( p => p.bar_code == Bar_Code_TXT.Text );
                // the product with this barcode exist, don't add !
                if(isProductExist != null)
                {
                    MessageBox.Show("Product with this bar code already exist!\nCan not Add this product.");
                    return;
                }
                store.products.Add(new product
                {
                    description = Description_TXT.Text,
                    bar_code = Bar_Code_TXT.Text,
                    price = Decimal.Parse(Price_TXt.Text),
                    quantity = Int32.Parse(Quantity_TXt.Text)
                });
                store.SaveChanges();
                MessageBox.Show("Product Added Successfully!");
                UpdateInventoryDataGrid();
            }
            catch
            {
                MessageBox.Show("price and quantity is numbers only!");
            }
        }
        // delete a product from the inventory, confirm message is provided
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            product productToDelete = getSelectedProduct();
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete [ " +
                productToDelete.description + " ]"
                ,
                "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                store.products.Remove(productToDelete);
                store.SaveChanges();
                MessageBox.Show("Deleted Successfully");
                UpdateInventoryDataGrid();
            }
        }
        // if the user is selecting a list item, add the product information
        // to the text fields if he wants to update, or delete
        private void ProductSelctedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // the Mysql entity framework has a bug and it is crashing if I try to
            // get the selected item and cast it to product. So I decided to substring the id
            // from the object of the selcted item
            product selectedProduct = getSelectedProduct();
            if (selectedProduct != null)
            {
            Id_TXT.Text = selectedProduct.id.ToString();
            Description_TXT.Text = selectedProduct.description;
            Bar_Code_TXT.Text = selectedProduct.bar_code;
            Price_TXt.Text = selectedProduct.price.ToString();
            Quantity_TXt.Text = selectedProduct.quantity.ToString();
            }
            else
            {
                Id_TXT.Text = "";
                Description_TXT.Text = "";
                Bar_Code_TXT.Text = "";
                Price_TXt.Text = "";
                Quantity_TXt.Text = "";
            }
        }
        // return a product object for a selected item
        private product getSelectedProduct()
        {
            object selectedItem = InventoryDataGrid.SelectedValue;
            Console.WriteLine(selectedItem);
            if (selectedItem != null && selectedItem.ToString().Contains("id") || selectedItem is product)
            {
                if (selectedItem is product)
                {
                    product selectedProduct = (product)selectedItem;
                    Console.WriteLine(selectedProduct);
                    Console.WriteLine(selectedProduct.id);
                    return selectedProduct;
                }
                else
                {
                int indexStart = selectedItem.ToString().IndexOf("id = ") + 5;
                int indexEnd = selectedItem.ToString().IndexOf(", description");
                int length = indexEnd - indexStart;

                string id = selectedItem.ToString().Substring(indexStart, length);

                product selectedProduct = store.products.Find(Int32.Parse(id));
                Console.WriteLine(selectedItem);
                Console.WriteLine(id);
                return selectedProduct;
                }
            }
            else
            {
                return null;
            }
            
        }
        private void UpdateInventoryDataGrid()
        {
            InventoryDataGrid.ItemsSource = null;
            InventoryDataGrid.ItemsSource = getProductsList();
        }
    }
}
