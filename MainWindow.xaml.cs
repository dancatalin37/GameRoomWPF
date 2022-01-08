using GameRoomModel;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace GameRoomWPF
{
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        GameRoomEntitiesModel ctx = new GameRoomEntitiesModel();
        CollectionViewSource customerVSource;
        CollectionViewSource gameVSource;
        CollectionViewSource packageVSource;
        CollectionViewSource customerPackagesVSource;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
    
 
        }

       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerVSource =
            ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerVSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            gameVSource =
           ((System.Windows.Data.CollectionViewSource)(this.FindResource("gameViewSource")));
            gameVSource.Source = ctx.Games.Local;
            ctx.Games.Load();

            packageVSource =
            ((System.Windows.Data.CollectionViewSource)(this.FindResource("packageViewSource")));
            packageVSource.Source = ctx.Packages.Local;
            ctx.Packages.Load();

            customerPackagesVSource =
            ((System.Windows.Data.CollectionViewSource)(this.FindResource("packageCustomerPackagesViewSource")));

            // customerPackagesVSource.Source = ctx.CustomerPackages.Local;
            ctx.CustomerPackages.Load();
            ctx.Packages.Load();
            cbCustomer.ItemsSource = ctx.Customers.Local;
             // cbCustomer.DisplayMemberPath = "FirstName";
            cbCustomer.SelectedValuePath = "CustomerID";
            cbPackage.ItemsSource = ctx.Packages.Local;
            // cbPackage.DisplayMemberPath = "PackageType";
            cbPackage.SelectedValuePath = "PackageID";
            BindDataGrid();

        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
           
        }
        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
          
        }
        private void btnDeleteO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToNext();
            gameVSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToPrevious();
            gameVSource.View.MoveCurrentToPrevious();
        }
        private void SaveCustomers()
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {

                    customer = new Customer()
                    {
                        CustomerID = int.Parse(customerIDTextBox.Text),
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                        Phone = phoneTextBox.Text.Trim(),
                        Registered_Date = registered_DateDatePicker.SelectedDate
                };
                
                    ctx.Customers.Add(customer);
                    customerVSource.View.Refresh();
                 
                    ctx.SaveChanges();
                }
              
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
           if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.CustomerID = int.Parse(customerIDTextBox.Text);
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    customer.Phone = phoneTextBox.Text.Trim();
                    customer.Registered_Date = registered_DateDatePicker.SelectedDate;
               
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerVSource.View.Refresh();
            }

        }

        private void SaveGames()
        {
            Game game = null;
            if (action == ActionState.New)
            {
                try
                {

                    game = new Game()
                    {
                        GameId = int.Parse(gameIdTextBox.Text),
                        Name = nameTextBox.Text.Trim(),
                        Category = categoryTextBox.Text.Trim(),
                        Type = typeTextBox.Text.Trim(),
                        Release_Date = release_DateDatePicker.SelectedDate
                };

                    ctx.Games.Add(game);
                    gameVSource.View.Refresh();

                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
           if (action == ActionState.Edit)
            {
                try
                {
                    game = (Game)gameDataGrid.SelectedItem;
                    game.GameId = int.Parse(gameIdTextBox.Text);
                    game.Name = nameTextBox.Text.Trim();
                    game.Type = typeTextBox.Text.Trim();       
                    game.Release_Date = release_DateDatePicker.SelectedDate;
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    game = (Game)gameDataGrid.SelectedItem;
                    ctx.Games.Remove(game);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                gameVSource.View.Refresh();
            }

        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.CustomerPackages
                             join cust in ctx.Customers on ord.CustomerID equals
                             cust.CustomerID
                             join inv in ctx.Packages on ord.PackageID
                 equals inv.PackageID
                             select new
                             {
                                 ord.CustomerPackageID,
                                 ord.PackageID,
                                 ord.CustomerID,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Price,
                                 inv.PackageType,
                                 inv.DailyTime,
                             };
            customerPackagesVSource.Source = queryOrder.ToList();
        }

        private void SavePackages()
        {
            Package package = null;
            if (action == ActionState.New)
            {
                try
                {

                    package = new Package()
                    {
                        PackageID = int.Parse(packageIDTextBox.Text),
                        PackageType = packageTypeTextBox.Text.Trim(),
                        Full_Night = full_NightTextBox.Text.Trim(),
                        NumberOfControllers = int.Parse(numberOfControllersTextBox.Text.Trim()),
                        DailyTime = dailyTimeTextBox.Text.Trim(),
                        Price = priceTextBox.Text.Trim()

                    };

                    ctx.Packages.Add(package);
                    packageVSource.View.Refresh();

                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
           if (action == ActionState.Edit)
            {
                try
                {
                    package = (Package)packageDataGrid.SelectedItem;
                    package.PackageID = int.Parse(packageIDTextBox.Text);
                    package.PackageType = packageTypeTextBox.Text.Trim();
                    package.NumberOfControllers = int.Parse(numberOfControllersTextBox.Text.Trim());
                    package.DailyTime = dailyTimeTextBox.Text.Trim();
                    package.Price = priceTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    package = (Package)packageDataGrid.SelectedItem;
                    ctx.Packages.Remove(package);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                packageVSource.View.Refresh();
            }

        }

       

        private void gbOperations_Click(object sender, RoutedEventArgs e)
        {
            Button SelectedButton = (Button)e.OriginalSource;
            Panel panel = (Panel)SelectedButton.Parent;

            foreach (Button B in panel.Children.OfType<Button>())
            {
                if (B != SelectedButton)
                    B.IsEnabled = false;
            }
            gbActions.IsEnabled = true;
          
        }


        private void SaveOrders()
        {
            CustomerPackage customerPackage = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cbCustomer.SelectedItem;
                    Package package = (Package)cbPackage.SelectedItem;
                   
                    customerPackage = new CustomerPackage()
                    {

                        CustomerID = customer.CustomerID,
                        PackageID = package.PackageID
                    };
                 
                    ctx.CustomerPackages.Add(customerPackage);
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
           if (action == ActionState.Edit)
            {
                dynamic selectedOrder = customerPackagesDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.CustomerPackageID;
                    var editedOrder = ctx.CustomerPackages.FirstOrDefault(s => s.CustomerPackageID == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustomerID = Int32.Parse(cbCustomer.SelectedValue.ToString());
                        editedOrder.PackageID = Convert.ToInt32(cbPackage.SelectedValue.ToString());
                       
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
               
                customerPackagesVSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = customerPackagesDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.CustomerPackages.FirstOrDefault(s => s.CustomerPackageID == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.CustomerPackages.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
       

        private void ReInitialize()
        {

            Panel panel = gbOperations.Content as Panel;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                B.IsEnabled = true;
            }
            gbActions.IsEnabled = true;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReInitialize();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tbCtlGameRoom.SelectedItem as TabItem;

            switch (ti.Header)
            {
                case "Customers":
                    SaveCustomers();
                    break;
                case "Games":
                    SaveGames();
                    break;
                case "Packages":
                    SavePackages();
                    break;
                case "Purchases":
                    SaveOrders();
                    break;

            }
            ReInitialize();
        }

       
    }
}
