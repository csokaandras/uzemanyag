using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace uzemanyag_elszamolas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        UserDataItem logged_user = null;
        dbClass db = new dbClass("localhost", "root", "", "uzemenyag_elszamolas");
        List<CarDataItem> cars_list = new List<CarDataItem>();
        List<FuelDataItem> fuels_list = new List<FuelDataItem>();
        List<RouteDataItem> routes_list = new List<RouteDataItem>();
        List<UserDataItem> users_list = new List<UserDataItem>();

        static int akt_car_ID = 0;

        internal class CarDataItem
        {
            public int ID { get; set; }
            public string Type { get; set; }
            public string License { get; set; }
            public double Consumption { get; set; }
            public FuelDataItem FuelID { get; set; }
            public int Enable { get; set; }
        }

        internal class FuelDataItem
        {
            public int ID { get; set; }
            public string Type { get; set; }
            public int Price { get; set; }
        }

        internal class RouteDataItem
        {
            public int ID { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public int Km { get; set; }
            public string Date { get; set; }
            public UserDataItem User { get; set; }
            public CarDataItem Car { get; set; }
        }

        internal class UserDataItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public int Perm { get; set; }
        }

        public MainWindow()
        {

            InitializeComponent();
            SetDefault();
            updateCarsGrid();
            updateFuelPrice();
            uodateRoutesCars();

            string aktdate = DateTime.Today.ToString("yyyy-MM-dd");
            routes_date_dtp.Text = aktdate;

            statistic_tab.Visibility = Visibility.Hidden;
            fuel_tab.Visibility = Visibility.Hidden;
            cars_tab.Visibility = Visibility.Hidden;
            routes_tab.Visibility = Visibility.Hidden;

        }
        private void SetDefault()
        {
            akt_car_ID = 0;
            cars_datagrid.SelectedItem = null;
            car_add_btn.IsEnabled = true;
            car_disable_btn.IsEnabled = false;
            car_edit_btn.IsEnabled = false;
            
            license_tbox.Text = null;
            type_tbox.Text = null;
            consumption_tbox.Text = null;
            benzin_rdbtn.IsChecked = false;
            diesel_rdbtn.IsChecked = false;
        }

        private void SelectCarsDatas()
        {
            SelectFuelsDatas();
            cars_list.Clear();
            db.selectAll("cars");
            while (db.results.Read())
            {
                CarDataItem new_car = new CarDataItem();
                new_car.ID = db.results.GetInt32("ID");
                new_car.Type = db.results.GetString("type");
                new_car.License = db.results.GetString("license");
                new_car.Consumption = Math.Round(db.results.GetFloat("consumption"), 2);
                new_car.FuelID = fuels_list.Find(x => x.ID == db.results.GetInt32("fuelID"));
                new_car.Enable = db.results.GetInt16("enable");

                cars_list.Add(new_car);
            }
        }

        private void SelectFuelsDatas()
        {
            fuels_list.Clear();
            db.selectAll("fuels");
            while (db.results.Read())
            {
                FuelDataItem new_fuel = new FuelDataItem();
                new_fuel.ID = db.results.GetInt32("ID");
                new_fuel.Type = db.results.GetString("type");
                new_fuel.Price = db.results.GetInt32("price");

                fuels_list.Add(new_fuel);
            }
        }

        private void SelectRoutesDatas(int id, int perm)
        {
            routes_list.Clear();
            if (perm == 0)
            {
                SelectUserDatas();
                db.selectAll("routes");
                while (db.results.Read())
                {
                    RouteDataItem new_route = new RouteDataItem();
                    new_route.ID = db.results.GetInt32("ID");
                    new_route.Start = db.results.GetString("start");
                    new_route.End = db.results.GetString("end");
                    new_route.Km = db.results.GetInt32("km");
                    new_route.Date = db.results.GetDateTime("date").ToString("yyyy-MM-dd");
                    new_route.User = users_list.Find(x => x.ID == db.results.GetInt32("userID"));
                    new_route.Car = cars_list.Find(x => x.ID == db.results.GetInt32("carID"));

                    routes_list.Add(new_route);
                }
            }
            else
            {
                db.select("routes", "userID", logged_user.ID.ToString());
                while (db.results.Read())
                {
                    RouteDataItem new_route = new RouteDataItem();
                    new_route.ID = db.results.GetInt32("ID");
                    new_route.Start = db.results.GetString("start");
                    new_route.End = db.results.GetString("end");
                    new_route.Km = db.results.GetInt32("km");
                    new_route.Date = db.results.GetDateTime("date").ToString("yyyy-MM-dd");
                    new_route.User = logged_user;
                    new_route.Car = cars_list.Find(x => x.ID == db.results.GetInt32("carID"));

                    routes_list.Add(new_route);
                }
            }
        }
        private void SelectUserDatas()
        {
            db.selectAll("users");
            while (db.results.Read())
            {
                UserDataItem new_user = new UserDataItem();

                new_user.ID = db.results.GetInt32("ID");
                new_user.Name = db.results.GetString("name");
                new_user.Password = db.results.GetString("pass");
                new_user.Perm = db.results.GetInt32("perm");

                users_list.Add(new_user);
            }
        }

        private void to_login_btn_Click(object sender, RoutedEventArgs e)
        {
            reg_grid.Visibility = Visibility.Hidden;
            login_grid.Visibility = Visibility.Visible;
            reg_name_tbox.Text = null;
            reg_passwd_tbox.Clear();
            reg_confirm_tbox.Clear();
        }

        private void to_reg_btn_Click(object sender, RoutedEventArgs e)
        {
            reg_grid.Visibility = Visibility.Visible;
            login_grid.Visibility = Visibility.Hidden;
            login_name_tbox.Text = null;
            login_passwd_tbox.Clear();
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            db.select("users", "name", login_name_tbox.Text);

            if (db.results.Read())
            {
                int id = db.results.GetInt32("ID");
                string name = db.results.GetString("name");
                string pass = db.results.GetString("pass");
                int perm = db.results.GetInt32("perm");

                if (login_passwd_tbox.Password == pass)
                {
                    user_grind.Visibility = Visibility.Visible;
                    login_grid.Visibility = Visibility.Hidden;

                    login_name_tbox.Text = null;
                    login_passwd_tbox.Password = null;

                    logged_user = new UserDataItem();
                    logged_user.ID = id;
                    logged_user.Name = name;
                    logged_user.Password = pass;
                    logged_user.Perm = perm;

                    if (logged_user.Perm == 0)
                    {
                        fuel_tab.Visibility = Visibility.Visible;
                        cars_tab.Visibility = Visibility.Visible;
                    }
                    statistic_tab.Visibility = Visibility.Visible;
                    routes_tab.Visibility = Visibility.Visible;

                    user_name_label.Content = logged_user.Name;

                    SelectRoutesDatas(logged_user.ID, logged_user.Perm);
                    updateRoutesGrid();
                }
                else
                {
                    MessageBox.Show("Nem megfelelő jelszó!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    login_passwd_tbox.Password = null;
                }
            }
            else
            {
                MessageBox.Show("Nincsen iyen felhasználó!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                login_name_tbox.Text = null;
                login_passwd_tbox.Password = null;
            }
        }

        private void updateRoutesGrid()
        {
            routes_datagrid.Items.Clear();
            SelectRoutesDatas(logged_user.ID, logged_user.Perm);
            foreach (RouteDataItem item in routes_list)
            {
                routes_datagrid.Items.Add(item);
            }
        }

        private void reg_btn_Click(object sender, RoutedEventArgs e)
        {
            if (reg_passwd_tbox.Password != "" && reg_name_tbox.Text != "" && reg_confirm_tbox.Password != "")
            {
                db.select("users", "name", reg_name_tbox.Text);

                if (db.results.Read())
                {
                    string name = db.results.GetString("name");

                    if (reg_name_tbox.Text == name)
                    {
                        MessageBox.Show("Ez a felhasználó név már regisztrálva van!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                        reg_name_tbox.Text = null;
                        reg_passwd_tbox.Clear();
                        reg_confirm_tbox.Clear();
                    }
                }
                else
                {
                    if (reg_passwd_tbox.Password != reg_confirm_tbox.Password)
                    {
                        MessageBox.Show("Nem egyezik a két jelszó!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string[] fields = { "name", "pass", "perm" };
                        string[] values = { reg_name_tbox.Text, reg_passwd_tbox.Password, "1" };
                        db.insert("users", fields, values);
                        MessageBox.Show("Sikeres a regisztráció, jelentkezzen be!", "Siker!", MessageBoxButton.OK, MessageBoxImage.Information);

                        reg_grid.Visibility = Visibility.Hidden;
                        login_grid.Visibility = Visibility.Visible;
                        reg_name_tbox.Text = null;
                        reg_passwd_tbox.Clear();
                        reg_confirm_tbox.Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Nem adott meg adatokat!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztosan ki szeretne jelentkezni?", "Kijelentkezés", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                login_grid.Visibility = Visibility.Visible;
                user_grind.Visibility = Visibility.Hidden;

                login_grid.Visibility = Visibility.Visible;

                statistic_tab.Visibility = Visibility.Hidden;
                fuel_tab.Visibility = Visibility.Hidden;
                cars_tab.Visibility = Visibility.Hidden;
                routes_tab.Visibility = Visibility.Hidden;

                cars_list.Clear();
                fuels_list.Clear();
                routes_list.Clear();
                users_list.Clear();
            }
        }

        private void updateCarsGrid()
        {
            SelectCarsDatas();
            cars_datagrid.Items.Clear();

            foreach (CarDataItem item in cars_list)
            {
                cars_datagrid.Items.Add(item);
            }
        }

        private void car_add_btn_Click(object sender, RoutedEventArgs e)
        {
            Regex rgx = new Regex(@"^\d+(\.\d+)*$");

            if ((benzin_rdbtn.IsChecked == true || diesel_rdbtn.IsChecked == true)
                && type_tbox.Text != null
                && license_tbox.Text != null
                && consumption_tbox.Text != null
                && type_tbox.Text != ""
                && license_tbox.Text != ""
                && consumption_tbox.Text != "")
            {
                if (rgx.IsMatch(consumption_tbox.Text))
                {
                    db.select("cars", "license", license_tbox.Text);
                    if (!db.results.Read())
                    {
                        string fuel_id = "0";
                        if (benzin_rdbtn.IsChecked == true)
                        {
                            fuel_id = "1";
                        }
                        else
                        {
                            fuel_id = "2";
                        }

                        string[] fields = { "type", "license", "consumption", "fuelID", "enable" };
                        string[] values = { type_tbox.Text, license_tbox.Text, consumption_tbox.Text, fuel_id, "0" };

                        db.insert("cars", fields, values);
                        SetDefault();
                        fuel_id = "0";

                        updateCarsGrid();
                    }
                    else
                    {
                        MessageBox.Show("Ez a rendszám már használatban van!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nem megfelelő a fogyasztás formátuma!\nKérem ponttal válassza el.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nem adott meg minden szükséges adatot!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void car_disable_btn_Click(object sender, RoutedEventArgs e)
        {
            CarDataItem akt_car = cars_list.Find(x => x.ID == akt_car_ID);
            string[] fields = { "enable" };

            if (akt_car.Enable == 0)
            {
                akt_car.Enable = 1;
            }
            else
            {
                akt_car.Enable = 0;
            }
            string[] values = { akt_car.Enable.ToString() };

            db.update("cars", "ID", akt_car.ID.ToString(), fields, values);
            
            updateCarsGrid();
        }

        private void cars_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cars_datagrid.SelectedItem != null)
            {

                CarDataItem selectedItem = (CarDataItem)cars_datagrid.SelectedItem;

                license_tbox.Text = selectedItem.License;
                type_tbox.Text = selectedItem.Type;
                consumption_tbox.Text = selectedItem.Consumption.ToString().Replace(",", ".");

                if (selectedItem.FuelID.ID == 1)
                {
                    benzin_rdbtn.IsChecked = true;
                }
                else
                {
                    diesel_rdbtn.IsChecked = true;
                }

                akt_car_ID = selectedItem.ID;
                car_add_btn.IsEnabled = false;
                car_disable_btn.IsEnabled = true;
                car_edit_btn.IsEnabled = true;



            }
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SetDefault();
            }
        }

        private void car_edit_btn_Click(object sender, RoutedEventArgs e)
        {
            CarDataItem akt_car = cars_list.Find(x => x.ID == akt_car_ID);
            string fuel_id = "0";
            if (benzin_rdbtn.IsChecked == true)
            {
                fuel_id = "1";
            }
            else
            {
                fuel_id = "2";
            }

            string[] fields = { "type", "license", "consumption", "fuelID", "enable" };
            string[] values = { type_tbox.Text, license_tbox.Text, consumption_tbox.Text, fuel_id, akt_car.Enable.ToString() };

            db.update("cars", "ID", akt_car.ID.ToString(), fields, values);

            updateCarsGrid();
            SetDefault();
        }

        private void fuel_price_save_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztosan módosítja az üzemanyag árakat?", "Üzemanyag árak módosítása.", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                
                try
                {
                    Convert.ToInt32(fuel_price_benzin.Text);
                    Convert.ToInt32(fuel_price_diesel.Text);

                    string[] fields = { "price" };

                    string[] values_benzin = { fuel_price_benzin.Text };
                    db.update("fuels", "ID", "1", fields, values_benzin);

                    string[] values_diesel = { fuel_price_diesel.Text };
                    db.update("fuels", "ID", "2", fields, values_diesel);

                    updateFuelPrice();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Nem jó formátumban adta meg az adatokat!\nKérem egész számra kerekítve adja meg!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                updateFuelPrice();
            }
        }

        private void updateFuelPrice()
        {
            SelectFuelsDatas();
            fuel_price_benzin.Text = fuels_list.Find(x => x.ID == 1).Price.ToString();
            fuel_price_diesel.Text = fuels_list.Find(x => x.ID == 2).Price.ToString();
        }

        private void uodateRoutesCars()
        {
            for (int i = 0; i < cars_list.Count; i++)
            {
                if (cars_list[i].Enable == 0)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = $"{cars_list[i].License} - {cars_list[i].Type}";
                    routes_cars_ddown.Items.Add(item);
                }
            }
        }

        private void routes_add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (routes_start_tbox.Text != null
                && routes_end_tbox.Text != null
                && routes_km_tbox != null
                && routes_cars_ddown.SelectedItem != null
                && routes_start_tbox.Text != ""
                && routes_end_tbox.Text != ""
                && routes_km_tbox.Text != ""
                && routes_date_dtp.Text != "")
            {
                try
                {
                    Convert.ToInt32(routes_km_tbox.Text);
                    string start = routes_start_tbox.Text;
                    string end = routes_end_tbox.Text;
                    string km = routes_km_tbox.Text;
                    string date = Convert.ToDateTime(routes_date_dtp.Text).ToString("yyyy-MM-dd");
                    string userID = logged_user.ID.ToString();
                    string carID = cars_list.Find(x => x.License == routes_cars_ddown.SelectionBoxItem.ToString().Split(" ")[0]).ID.ToString();

                    string[] fields = { "start", "end", "km", "date", "userID", "carID" };
                    string[] values = { start, end, km, date, userID, carID };

                    db.insert("routes", fields, values);

                    updateRoutesGrid();
                }
                catch (FormatException)
                {
                    MessageBox.Show("A kilóméternek egész számnak kell lennie!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nem adott meg minden szükséges adatot!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }
    }
}