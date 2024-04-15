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
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    /// 

    public class Items
    {
        public string Gear { get; set; }
        public string Fault_type { get; set; }
        public string ID_order { get; set; }
        public string Status { get; set; }
        public string FIO { get; set; }
        public string Information { get; set; }
        public string Problem { get; set; }
        
    }
    public partial class Add : Page
    {
        public Add()
        {
            InitializeComponent();
            Gear_cmb.ItemsSource = viewGear();
            Type_cmb.ItemsSource = viewFault();
            DataGrid.ItemsSource = viewAll();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверенны, что хотите вернутья назад?", "Закрыть?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }

        public static List<Items> viewFault()
        {
            string sql = $"select [Fault_name] from dbo.[Fault_type]";
            List<Items> table = new List<Items>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                
                if (reader.HasRows)
                {
                    while (reader.Read()) { 
                        Items items = new Items();
                        items.Fault_type = reader.GetValue(0).ToString();
                        table.Add(items);
                    }
                    reader.Close();
                    conn.Close();
                }
                return table;
                
            }
        }

        public static List<Items> viewGear()
        {
            string sql = $"select [Name_Gear] from dbo.[Gear]";
            List<Items> table = new List<Items>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Items items = new Items();
                        items.Gear = reader.GetValue(0).ToString();
                        table.Add(items);
                    }
                    reader.Close();
                    conn.Close();
                }
                return table;

            }
        }

        public static List<Items> viewAll(string search = "")
        {
            string sql = $@"SELECT [Order].ID_order, [Fault_type].Fault_name, [Gear].Name_Gear, 
                    [Status].Name_status, [Order].FIO, [Order].Information, [Order].Problem 
                    FROM dbo.[Order] 
                    JOIN [Status] ON [Order].Status_id = [Status].ID 
                    JOIN [Fault_type] ON [Order].Fault_type_id = [Fault_type].ID 
                    JOIN [Gear] ON [Order].Gear_id = [Gear].ID";

            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE [Order].FIO LIKE @search OR [Order].Information LIKE @search OR [Order].Problem LIKE @search OR [Status].Name_status LIKE @search";
            }

            List<Items> table = new List<Items>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Items items = new Items()
                    {
                        ID_order = reader.GetValue(0).ToString(),
                        Fault_type = reader.GetValue(1).ToString(),
                        Gear = reader.GetValue(2).ToString(),
                        Status = reader.GetValue(3).ToString(),
                        FIO = reader.GetValue(4).ToString(),
                        Information = reader.GetValue(5).ToString(),
                        Problem = reader.GetValue(6).ToString(),
                    };
                    table.Add(items);
                }
                reader.Close();
            }
            return table;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Gear_cmb.SelectedValue == null || Type_cmb.SelectedValue == null || Problem_txt.Text == null || FIO_txt.Text == null || Tel_txt.Text == null)
            {
                MessageBox.Show("Не все поля заполнены!", "Заполните поля", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string sql = $"INSERT INTO [Order](Data_add, Fault_type_id, Gear_id, Status_id, FIO, Information, Problem) VALUES " +
                    $"(GETDATE(), '{Type_cmb.SelectedIndex + 1}', '{Gear_cmb.SelectedIndex + 1}', 1, '{FIO_txt.Text}', '{Tel_txt.Text}', '{Problem_txt.Text}')";
                            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
                            {
                                conn.Open();
                                SqlCommand cmd = new SqlCommand(sql, conn);
                                SqlDataReader reader = cmd.ExecuteReader();
                                conn.Close();
                            }
                DataGrid.ItemsSource = viewAll();
                MessageBox.Show("Заявка добвлена", "Заявка создана", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string search = search_txt.Text;
            DataGrid.ItemsSource = viewAll(search);
        }

        private void QR(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new QRcode());
        }
    }
}
