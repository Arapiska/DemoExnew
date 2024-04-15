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
    /// Логика взаимодействия для Work.xaml
    /// </summary>
    /// 
    public class ItemsWork
    {
        public string Gear { get; set; }
        public string Fault_type { get; set; }
        public string ID_order { get; set; }
        public string Status { get; set; }
        public string Problem { get; set; }
        public string Date_end { get; set; }
        public string Comment { get; set;}
    }
    public partial class Work : Page
    {
        public Work()
        {
            InitializeComponent();
            Id_cmb.ItemsSource = viewID();
            Status_cmb.ItemsSource = viewStatus();
            DataGrid.ItemsSource = viewAll();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверенны, что хотите вернутья назад?", "Закрыть?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }

        public static List<ItemsWork> viewID()
        {
            string sql = $"select [ID_order] from dbo.[Order] where Executive_id = {IdForUser.user_id}";
            List<ItemsWork> table = new List<ItemsWork>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ItemsWork items = new ItemsWork();
                        items.ID_order = reader.GetValue(0).ToString();
                        table.Add(items);

                    }
                    reader.Close();
                    conn.Close();
                }
                return table;

            }
        }

        public static List<ItemsWork> viewStatus()
        {
            string sql = $"select [Name_status] from dbo.[Status]";
            List<ItemsWork> table = new List<ItemsWork>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ItemsWork items = new ItemsWork();
                        items.Status = reader.GetValue(0).ToString();
                        table.Add(items);

                    }
                    reader.Close();
                    conn.Close();
                }
                return table;

            }
        }

        public static List<ItemsWork> viewAll(string search = "")
        {
            string sql = $@"SELECT [Order].ID_order, [Fault_type].Fault_name, [Gear].Name_Gear, 
                    [Status].Name_status, [Order].Problem, [Order].Data_end, [Order].Comment
                    FROM dbo.[Order] 
                    JOIN [Status] ON [Order].Status_id = [Status].ID 
                    JOIN [Fault_type] ON [Order].Fault_type_id = [Fault_type].ID 
                    JOIN [Gear] ON [Order].Gear_id = [Gear].ID
                    WHERE [Order].Executive_id = {IdForUser.user_id}";

            if (!string.IsNullOrEmpty(search))
            {
                sql += "and [Order].Problem LIKE @search OR [Status].Name_status LIKE @search OR [Fault_type].Fault_name LIKE @search OR [Order].Comment LIKE @search";
            }

            List<ItemsWork> table = new List<ItemsWork>();
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
                    ItemsWork items = new ItemsWork()
                    {
                        ID_order = reader.GetValue(0).ToString(),
                        Fault_type = reader.GetValue(1).ToString(),
                        Gear = reader.GetValue(2).ToString(),
                        Status = reader.GetValue(3).ToString(),
                        Problem = reader.GetValue(4).ToString(),
                        Date_end = reader.GetValue(5).ToString(),
                        Comment = reader.GetValue(6).ToString(),


                    };
                    table.Add(items);
                }
                reader.Close();
            }
            return table;
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if (Status_cmb.SelectedValue != null)
            {
                string sql = $"UPDATE [Order] SET Status_id='{Status_cmb.SelectedIndex + 1}' WHERE [ID_order] = {Id_cmb.Text}";
                using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    conn.Close();
                }
                DataGrid.ItemsSource = viewAll();
                MessageBox.Show("Заявка изменена", "Заявка отредактированна", MessageBoxButton.OK, MessageBoxImage.Information);
                Status_cmb.SelectedValue = null;
                Id_cmb.SelectedValue = null;
            }else if (Comm_txt.Text != "")
            {
                string sql = $"UPDATE [Order] SET Comment='{Comm_txt.Text}' WHERE [ID_order] = {Id_cmb.Text}";
                using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    conn.Close();
                }
                DataGrid.ItemsSource = viewAll();
                MessageBox.Show("Заявка изменена", "Заявка отредактированна", MessageBoxButton.OK, MessageBoxImage.Information);
                Id_cmb.SelectedValue = null;
                Comm_txt.Text = "";
            }
            
            
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string search = search_txt.Text;
            DataGrid.ItemsSource = viewAll(search);
        }
    }
}
