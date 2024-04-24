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

namespace uzemanyag_elszamolas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        UsersClass logged_user = null;
        dbClass db = new dbClass("localhost", "root", "", "uzemenyag_elszamolas");
        public MainWindow()
        {
            InitializeComponent();
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
                    username_field.Visibility = Visibility.Visible;
                    logged_user = new UsersClass(id, name, pass, perm);

                    username_field.Header = logged_user.Name;
                }
                else
                {
                    MessageBox.Show("Nem megfelelő jelszó!");
                }
            }
            else
            {
                MessageBox.Show("Nincsen iyen felhasználó!");
            }
        }

        private void reg_btn_Click(object sender, RoutedEventArgs e)
        {
            db.select("users", "name", reg_name_tbox.Text);

            if (db.results.Read())
            {
                string name = db.results.GetString("name");

                if (reg_name_tbox.Text == name)
                {
                    MessageBox.Show("Ez a felhasználó név már regisztrálva van!");
                    reg_name_tbox.Text = null;
                    reg_passwd_tbox.Clear();
                    reg_confirm_tbox.Clear();
                }
            }
            else
            {
                if (reg_passwd_tbox.Password != reg_confirm_tbox.Password)
                {
                    MessageBox.Show("Nem egyezik a két jelszó!");
                }
                else
                {
                    string[] fields = { "name", "pass", "perm" };
                    string[] values = { reg_name_tbox.Text, reg_passwd_tbox.Password , "1"};
                    db.insert("users", fields, values);
                    MessageBox.Show("Sikeres a regisztráció, jelentkezzen be!");

                    reg_grid.Visibility = Visibility.Hidden;
                    login_grid.Visibility = Visibility.Visible;
                    reg_name_tbox.Text = null;
                    reg_passwd_tbox.Clear();
                    reg_confirm_tbox.Clear();
                }
            }
        }

        private void username_field_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult Result = MessageBox.Show("This Seat is Available, Would you like to pick it?", "Would you like this seat?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
