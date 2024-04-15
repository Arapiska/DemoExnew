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
    /// Логика взаимодействия для edit.xaml
    /// </summary>
    /// 
    public class ItemsEdit
    {
        public string Gear { get; set; }
        public string Fault_type { get; set; }
        public string ID_order { get; set; }
        public string Status { get; set; }
        public string Problem { get; set; }
        public string Date_add { get; set; }
        public string Date_end { get; set; }
        public string Executer { get; set; }
        public string IDExecuter { get; set; }
        public string FIOUser { get; set; }
    }
    public partial class edit : Page
    {
        public edit()
        {
            InitializeComponent();
            Id_cmb.ItemsSource = viewID();
            Exec_cmb.ItemsSource = viewExec();
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

        public static List<ItemsEdit> viewID()
        {
            string sql = $"select [ID_order] from dbo.[Order]";
            List<ItemsEdit> table = new List<ItemsEdit>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ItemsEdit items = new ItemsEdit();
                        items.ID_order = reader.GetValue(0).ToString();
                        table.Add(items);

                    }
                    reader.Close();
                    conn.Close();
                }
                return table;

            }
        }

        public static List<ItemsEdit> viewExec()
        {
            string sql = $"select [ID], [FIO] from dbo.[User] where ID_role = 2";
            List<ItemsEdit> table = new List<ItemsEdit>();
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ItemsEdit items = new ItemsEdit();
                        items.IDExecuter = reader.GetValue(0).ToString();
                        items.Executer = reader.GetValue(1).ToString();
                        table.Add(items);

                    }
                    reader.Close();
                    conn.Close();
                }
                return table;

            }
        }

        public static List<ItemsEdit> viewAll(string search = "")
        {
            string sql = $@"SELECT [Order].ID_order, [Fault_type].Fault_name, [Gear].Name_Gear, 
                    [Status].Name_status, [Order].Problem, [User].FIO, [Order].Executive_id, [Order].FIO, [Order].Data_add, [Order].Data_end
                    FROM dbo.[Order] 
                    JOIN [Status] ON [Order].Status_id = [Status].ID 
                    JOIN [Fault_type] ON [Order].Fault_type_id = [Fault_type].ID 
                    JOIN [Gear] ON [Order].Gear_id = [Gear].ID
                    JOIN [User] ON [Order].Executive_id = [User].ID";

            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE [Order].FIO LIKE @search OR [Order].Problem LIKE @search OR [Status].Name_status LIKE @search OR [Order].Executive_id LIKE @search";
            }

            List<ItemsEdit> table = new List<ItemsEdit>();
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
                    ItemsEdit items = new ItemsEdit()
                    {
                        ID_order = reader.GetValue(0).ToString(),
                        Fault_type = reader.GetValue(1).ToString(),
                        Gear = reader.GetValue(2).ToString(),
                        Status = reader.GetValue(3).ToString(),
                        Problem = reader.GetValue(4).ToString(),
                        Executer = reader.GetValue(5).ToString(),
                        IDExecuter = reader.GetValue(6).ToString(),
                        FIOUser = reader.GetValue(7).ToString(),
                        Date_add = reader.GetValue(8).ToString(),
                        Date_end = reader.GetValue(9).ToString(),

                    };
                    table.Add(items);
                }
                reader.Close();
            }
            return table;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Exec_cmb.SelectedValue != null)
            {
                string sql = $"UPDATE [Order] SET Executive_id='{Exec_cmb.SelectedValue}', Status_id = 2 FROM [Order] JOIN [User] ON [User].ID = [Order].Executive_id WHERE [ID_order] = {Id_cmb.Text}";
                using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    conn.Close();
                }
                DataGrid.ItemsSource = viewAll();
                MessageBox.Show("Заявка изменена", "Заявка отредактированна", MessageBoxButton.OK, MessageBoxImage.Information);
                Exec_cmb.SelectedValue = null;
                Id_cmb.SelectedValue = null;
            }
            else if (Date_txt.Text != "")
            {
               if(!Date_txt.Text.Contains("."))
                {
                    MessageBox.Show("Не все поля заполнены!", "Назначте дату окончания работ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string sql = $"UPDATE [Order] SET Data_end='{Date_txt.Text}' WHERE [ID_order] = {Id_cmb.Text}";
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
                    Date_txt.Text = "";
                }
                
            }else if (Problem_txt.Text != "")
            {
                string sql = $"UPDATE [Order] SET Problem = '{Problem_txt.Text}' WHERE [ID_order] = {Id_cmb.Text}";
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
                Problem_txt.Text = "";

            }
            else if (Exec_cmb.SelectedValue == null || Date_txt.Text == "" || Problem_txt.Text == "" || Id_cmb.SelectedValue == null)
            {
                MessageBox.Show("Не все поля заполнены!", "Заполните необходимые поля!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string search = search_txt.Text;
            DataGrid.ItemsSource = viewAll(search);

        }

        private void Static(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Static());
        }
    }
}
