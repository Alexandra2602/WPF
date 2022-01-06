using AutoLotModel;
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

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
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
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerVSource;
        CollectionViewSource roomVSource;
        CollectionViewSource customerReservationsVSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerVSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            roomVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("roomViewSource")));
            roomVSource.Source = ctx.Rooms.Local;
            ctx.Rooms.Load();

            customerReservationsVSource= ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerReservationsViewSource")));
           //customerReservationsVSource.Source = ctx.Reservations.Local;
            ctx.Reservations.Load();

            BindDataGrid();

            ctx.Rooms.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustomerId";

            cmbRooms.ItemsSource = ctx.Rooms.Local;
            //cmbRooms.DisplayMemberPath = "Type";
            cmbRooms.SelectedValuePath = "RoomId";

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToNext();
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToPrevious();
        }
        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            roomVSource.View.MoveCurrentToNext();
        }
        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            roomVSource.View.MoveCurrentToPrevious();
        }
        private void SaveCustomers()
        {
            Customer customer = null;
            if(action==ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerVSource.View.Refresh();
                    //salvam modific
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action== ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modifc
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else if (action==ActionState.Delete)
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
        private void SaveRooms()
        {
            Room room = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Room entity
                    room = new Room()
                    {
                        Type = typeTextBox.Text.Trim(),
                        Style = styleTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Rooms.Add(room);
                    roomVSource.View.Refresh();
                    //salvam modific
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    room = (Room)roomDataGrid.SelectedItem;
                    room.Type = typeTextBox.Text.Trim();
                    room.Style = styleTextBox.Text.Trim();
                    //salvam modifc
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
                    room = (Room)roomDataGrid.SelectedItem;
                    ctx.Rooms.Remove(room);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
        private void Reinitialize()
        {
            Panel panel = gbOperations.Content as Panel;
            foreach (Button B in panel.Children.OfType<Button>())
            {
                B.IsEnabled = true;
            }
            gbActions.IsEnabled = false;
        }
        private void btnCancel_Click(object sender,RoutedEventArgs e)
        {
            Reinitialize();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tbCtrlAutoLot.SelectedItem as TabItem;
            switch (ti.Header)
            {
                case "Customers":
                    SaveCustomers();
                    break;
                case "Rooms":
                    SaveRooms();
                    break;
                case "Reservations":
                    SaveReservations();
                    break;
                    
            }
            Reinitialize();
        }

        private void SaveReservations()
        {
            Reservation reservation = null;
            if(action== ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Room room = (Room)cmbRooms.SelectedItem;

                    //instantiem Reservation entity
                    reservation = new Reservation()
                    {
                        CustomerId = customer.CustomerId,
                        RoomId = room.RoomId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Reservations.Add(reservation);
                    //salvam modific
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch (DataException ex)
                    {
                    MessageBox.Show(ex.Message);
                }
            }
            else if(action == ActionState.Edit)
            {
                dynamic selectedReservation = reservationsDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedReservation.ReservationId;
                    var editedReservation = ctx.Reservations.FirstOrDefault(s => s.ReservationId == curr_id);
                    if(editedReservation !=null )
                    {
                        editedReservation.CustomerId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedReservation.RoomId = Convert.ToInt32(cmbRooms.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                //pozitionarea pe item-ul curent
                customerReservationsVSource.View.MoveCurrentTo(selectedReservation);
            }
            else if (action==ActionState.Delete)
            {
                try
                {
                    dynamic selectedReservation = reservationsDataGrid.SelectedItem;
                    int curr_id = selectedReservation.ReservationId;
                    var deletedReservation = ctx.Reservations.FirstOrDefault(s => s.ReservationId == curr_id);
                    if(deletedReservation !=null)
                    {
                        ctx.Reservations.Remove(deletedReservation);
                        ctx.SaveChanges();
                        MessageBox.Show("Reservation Deleted Succesfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BindDataGrid()
        {
            var queryReservation = from res in ctx.Reservations join cust in ctx.Customers on res.CustomerId equals cust.CustomerId join room in ctx.Rooms on res.CustomerId equals room.RoomId select new { res.ReservationId, res.RoomId, res.CustomerId, cust.FirstName, cust.LastName, room.Type, room.Style };
            customerReservationsVSource.Source = queryReservation.ToList();                      
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerVSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerVSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }
   
    }
}
