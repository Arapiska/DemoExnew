using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace DemoEx
{
    /// <summary>
    /// Логика взаимодействия для auto.xaml
    /// </summary>
    public partial class auto : Page
    {
        public auto()
        {
            InitializeComponent();
        }

        private void Auto(object sender, RoutedEventArgs e)
        {
            var logUser=log_txt.Text;
            var passUser = pass_txt.Password;

            string connection = "Data Source=localhost;Initial Catalog=demoex;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connection);
            conn.Open();
            SqlCommand cmd = new SqlCommand($"select [ID], [ID_role] from dbo.[User] where Login='{logUser}' and Password='{passUser}'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            
            if (!reader.HasRows)
            {
                MessageBox.Show("Проверте логин и пароль!", "Аккаунта не существует!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int id = int.Parse(reader["ID"].ToString());
                IdForUser.user_id = id;
                int Role_id = int.Parse(reader["ID_role"].ToString());
                switch(Role_id)
                {
                    case 1:
                        NavigationService.Navigate(new Add());
                        break;
                    case 2:
                        NavigationService.Navigate(new Work());
                        break;
                    case 3:
                        NavigationService.Navigate(new edit());
                        break;
                }
            }
            reader.Close();
            conn.Close();
        }
    }
}
