using System;
using System.Collections.Generic;
using System.Globalization;
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
        static string aktdate = DateTime.Today.ToString("yyyy-MM-dd");
        static int km_price = 100;
        UserDataItem logged_user = null;
        dbClass db = new dbClass("localhost", "root", "", "uzemenyag_elszamolas");
        List<CarDataItem> cars_list = new List<CarDataItem>();
        List<FuelDataItem> fuels_list = new List<FuelDataItem>();
        List<RouteDataItem> routes_list = new List<RouteDataItem>();
        List<UserDataItem> users_list = new List<UserDataItem>();

        List<StatDataItem> stats_list = new List<StatDataItem>();
        static List<YearDataItem> years = new List<YearDataItem>();

        static int akt_car_ID = 0;
        static int akt_route_ID = 0;

        internal class StatDataItem
        {
            public UserDataItem User { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public double Price { get; set; }
            public int Km { get; set; }
        }

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
            public double Osszeg { get; set; }
        }

        internal class UserDataItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public int Perm { get; set; }
        }

        internal class YearDataItem
        {
            public int Year { get; set; }
            public List<MonthDataItem> Months { get; set; }
            public int Km { get; set; }
            public double Osszeg { get; set; }
        }

        internal class MonthDataItem
        {
            public int Month { get; set; }
            public List<DayDataItem> Days { get; set; }
            public int Km { get; set; }
            public double Osszeg { get; set; }
        }

        internal class DayDataItem
        {
            public int Day { get; set; }
            public int Km { get; set; }
            public double Osszeg { get; set; }
        }

        public MainWindow()
        {

            InitializeComponent();
            SetDefault();
            SelectCarsDatas();

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

            akt_route_ID = 0;
            routes_datagrid.SelectedItem = null;
            routes_start_tbox.Text = null;
            routes_end_tbox.Text = null;
            routes_km_tbox.Text = null;
            routes_date_dtp.Text = aktdate;

            
            updateRoutesCars();
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

        private void SelectRoutesDatas(int perm)
        {
            routes_list.Clear();
            stats_list.Clear();
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
                    new_route.Osszeg = Math.Round(new_route.Car.FuelID.Price * new_route.Km * (new_route.Car.Consumption * 0.1) + (new_route.Km * km_price), 2);

                    routes_list.Add(new_route);

                    StatDataItem new_stat = new StatDataItem();
                    new_stat.User = users_list.Find(x => x.ID == db.results.GetInt32("userID"));
                    new_stat.Year = db.results.GetDateTime("date").Year;
                    new_stat.Month = db.results.GetDateTime("date").Month;
                    new_stat.Day = db.results.GetDateTime("date").Day;
                    new_stat.Price = new_route.Osszeg;
                    new_stat.Km = db.results.GetInt32("km");

                    stats_list.Add(new_stat);
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
                    new_route.User = users_list.Find(x => x.ID == db.results.GetInt32("userID"));
                    new_route.Car = cars_list.Find(x => x.ID == db.results.GetInt32("carID"));
                    new_route.Osszeg = Math.Round(new_route.Car.FuelID.Price * new_route.Km * (new_route.Car.Consumption * 0.1) + (new_route.Km * km_price), 2);

                    routes_list.Add(new_route);

                    StatDataItem new_stat = new StatDataItem();
                    new_stat.User = users_list.Find(x => x.ID == db.results.GetInt32("userID"));
                    new_stat.Year = db.results.GetDateTime("date").Year;
                    new_stat.Month = db.results.GetDateTime("date").Month;
                    new_stat.Day = db.results.GetDateTime("date").Day;
                    new_stat.Price = new_route.Osszeg;
                    new_stat.Km = db.results.GetInt32("km");

                    stats_list.Add(new_stat);
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

                    SelectRoutesDatas(logged_user.Perm);
                    updateCarsGrid();
                    updateFuelPrice();
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
            SelectRoutesDatas(logged_user.Perm);
            foreach (RouteDataItem item in routes_list)
            {
                routes_datagrid.Items.Add(item);
            }
            updateStatTreeView();
            updateRoutesStat();
        }

        
        private void updateStatTreeView()
        {
            years.Clear();
            foreach (StatDataItem item in stats_list)
            {
                YearDataItem akt_year = years.Find(x => x.Year == item.Year);
                if (akt_year == null)
                {
                    YearDataItem new_year = new YearDataItem();
                    new_year.Year = item.Year;
                    new_year.Months = new List<MonthDataItem>();
                    new_year.Km = item.Km;
                    new_year.Osszeg = item.Price;

                    MonthDataItem akt_month = new_year.Months.Find(x => x.Month == item.Month);
                    if (akt_month == null)
                    {
                        MonthDataItem new_month = new MonthDataItem();
                        new_month.Month = item.Month;
                        new_month.Days = new List<DayDataItem>();
                        new_month.Km = item.Km;
                        new_month.Osszeg = item.Price;

                        DayDataItem akt_day = new_month.Days.Find(x => x.Day == item.Day);
                        if (akt_day == null)
                        {
                            DayDataItem new_day = new DayDataItem();
                            new_day.Day = item.Day;
                            new_day.Km = item.Km;
                            new_day.Osszeg = item.Price;

                            new_month.Days.Add(new_day);
                        }
                        akt_day = null;
                        new_year.Months.Add(new_month);
                    }
                    akt_month = null;
                    years.Add(new_year);
                }
                else
                {
                    MonthDataItem akt_month = akt_year.Months.Find(x => x.Month == item.Month);
                    if (akt_month == null)
                    {
                        MonthDataItem new_month = new MonthDataItem();
                        new_month.Month = item.Month;
                        new_month.Days = new List<DayDataItem>();
                        new_month.Km = item.Km;
                        new_month.Osszeg = item.Price;

                        DayDataItem akt_day = new_month.Days.Find(x => x.Day == item.Day);
                        if (akt_day == null)
                        {
                            DayDataItem new_day = new DayDataItem();
                            new_day.Day = item.Day;
                            new_day.Km = item.Km;
                            new_day.Osszeg = item.Price;

                            new_month.Days.Add(new_day);
                        }
                        else
                        {
                            akt_day.Km += item.Km;
                            akt_day.Osszeg += item.Price;
                            
                        }
                        akt_day = null;
                        akt_year.Months.Add(new_month);
                    }
                    else
                    {
                        DayDataItem akt_day = akt_month.Days.Find(x => x.Day == item.Day);
                        if (akt_day == null)
                        {
                            DayDataItem new_day = new DayDataItem();
                            new_day.Day = item.Day;
                            new_day.Km = item.Km;
                            new_day.Osszeg = item.Price;

                            akt_month.Days.Add(new_day);
                        }
                        else
                        {
                            akt_day.Km += item.Km;
                            akt_day.Osszeg += item.Price;

                        }
                        akt_day = null;

                        akt_month.Km += item.Km;
                        akt_month.Osszeg += item.Price;
                    }
                    akt_month = null;

                    akt_year.Km += item.Km;
                    akt_year.Osszeg += item.Price;
                }
                akt_year = null;
            }
            
            List<YearDataItem> SortedYears = years.OrderBy(o => o.Year).ToList();
            foreach (YearDataItem year in SortedYears)
            {
                List<MonthDataItem> SoertedMonths = year.Months.OrderBy(o => o.Month).ToList();
                year.Months = SoertedMonths;
                foreach (MonthDataItem month in year.Months)
                {
                    List<DayDataItem> SoertedDays = month.Days.OrderBy(o => o.Day).ToList();
                    month.Days = SoertedDays;
                }
            }
            years = SortedYears;
            
            stat_tree.Items.Clear();
            foreach (YearDataItem year in years)
            {
                TreeViewItem year_branch = new TreeViewItem() { Header = $"{year.Year} - {year.Km}Km {year.Osszeg} zseton" };
                stat_tree.Items.Add(year_branch);

                foreach (MonthDataItem month in year.Months)
                {
                    TreeViewItem month_branch = new TreeViewItem() { Header = $"{month.Month} - {month.Km}Km {month.Osszeg} zseton" };
                    year_branch.Items.Add(month_branch);

                    foreach (DayDataItem day in month.Days)
                    {
                        TreeViewItem day_branch = new TreeViewItem() { Header = $"{day.Day} - {day.Km}Km {day.Osszeg} zseton" };
                        month_branch.Items.Add(day_branch);
                    }
                }
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

                routes_list.Clear();

                SetDefault();
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
            updateRoutesCars();
            updateRoutesGrid();
        }

        private void updateRoutesStat()
        {
            int ossz_km = 0;
            double ossz_fiz = 0;

            foreach (RouteDataItem item in routes_list)
            {
                ossz_km += item.Km;
                ossz_fiz += item.Osszeg;
            }

            routes_osszkm_tbox.Text = $"{ossz_km} Km";
            routes_osszfiz_tbox.Text = ossz_fiz.ToString("### ### Ft");
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
            updateCarsGrid();
        }

        private void updateRoutesCars()
        {
            routes_cars_ddown.Items.Clear();
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
                    SetDefault();
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

        private void routes_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (routes_datagrid.SelectedItem != null)
            {

                RouteDataItem selectedItem = (RouteDataItem)routes_datagrid.SelectedItem;

                routes_start_tbox.Text = selectedItem.Start;
                routes_end_tbox.Text = selectedItem.End;
                routes_km_tbox.Text = selectedItem.Km.ToString();
                routes_date_dtp.Text = selectedItem.Date;
                //routes_cars_ddown.SelectedIndex = routes_cars_ddown.Find($"{selectedItem.Car.License} - {selectedItem.Car.Type}");

                akt_route_ID = selectedItem.ID;
            }
        }

        private void routes_delete_btn_Click(object sender, RoutedEventArgs e)
        {
            db.delete("routes", "ID", akt_route_ID.ToString());
            updateRoutesGrid();
        }
    }
}