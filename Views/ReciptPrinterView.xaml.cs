using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for ReciptPrinterView.xaml
    /// </summary>
    public partial class ReciptPrinterView : UserControl
    {
        public ObservableCollection<product> Products { get; set; }

        public ReciptPrinterView()
        {
            InitializeComponent();
            //printRecipt();
            Products = new ObservableCollection<product>();
            ProductsDetailsGrid.DataContext = this;

        }

        public void printRecipt(ListView o)
        {
            // products that are the receipt, and clear it first
            ItemCollection productsDetailsGridCollection = ProductsDetailsGrid.Items;
            productsDetailsGridCollection.Clear();
            // the products that the customer bought
            ItemCollection currentProductsItems = o.Items;
            List<product> productsInCartList = currentProductsItems.OfType<product>().ToList();
            // add the products from cart to receipt
            decimal totalPrice = 0;
            foreach (var productInCart in productsInCartList)
            {
                productsDetailsGridCollection.Add(productInCart);
                totalPrice += productInCart.subTotal;
            }
            decimal VAT = totalPrice * decimal.Parse("0.15");
            decimal totalPriceWithVat = VAT + totalPrice;
            // set the values for the receipt, such as 
            // date, time, cachire, ets
            Date_TXT_Block.Text = DateTime.Now.ToShortDateString();
            Time_TXT_Block.Text = DateTime.Now.ToShortTimeString();
            Receipt_Number_TXT_Block.Text = "111111";
            Cashire_Number_TXT_Block.Text = "344444";
            Price_TXT_Blcok.Text = totalPrice.ToString() + " SAR";
            VAT_TXT_Blcok.Text = (VAT).ToString() + " SAR";
            Total_Price_TXT_Blcok.Text = (totalPriceWithVat).ToString() + " SAR";

            // ask user for choosing printer and print,
            // in case of errors, show message that it cannot be
            // printed
            try
            {
                //this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    //ReciptGrid is the name of the grid for the recipt
                    printDialog.PrintVisual(ReciptGrid, "Receipt");
                }
            }
            catch
            {
                //this.IsEnabled = true;
                MessageBox.Show("Error! Receipt coudn't be printed");
            }
        }
    }
}
