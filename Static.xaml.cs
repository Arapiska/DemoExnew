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
    /// Логика взаимодействия для Static.xaml
    /// </summary>
    public partial class Static : Page
    {
        public Static()
        {
            InitializeComponent();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверенны, что хотите вернутья назад?", "Закрыть?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }


        private void UpdateCompletedOrders()
        {
            string sql = "SELECT COUNT(*) AS Completed FROM [Order] WHERE Status_id = 3;";

            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                int completedOrders = (int)cmd.ExecuteScalar();
                kol_txt.Text = $"{completedOrders}";
                
            }
        }

        private void UpdateAvgTime()
        {
            string sql = "SELECT AVG(DATEDIFF(day, Data_add, Data_end)) AS AvgTime FROM [Order] WHERE Status_id = 3;";

            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null) // Проверка на DBNull и null
                {
                    double avgTime = Convert.ToDouble(result); // Используйте Convert.ToDouble для приведения
                    time_txt.Text = $"{avgTime} д.";
                }
                else
                {
                    time_txt.Text = "Нет данных";
                }

            }
        }

        private void UpdateFaults()
        {
            StringBuilder results = new StringBuilder();
            string sql = "SELECT ft.Fault_name, COUNT(o.ID_order) AS EquipmentCount FROM Fault_type ft LEFT JOIN [Order] o ON ft.ID = o.Fault_type_id GROUP BY ft.Fault_name;";

            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=demoex;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string faultName = reader["Fault_name"].ToString();
                        int equipmentCount = (int)reader["EquipmentCount"];
                        results.AppendLine($"{faultName}: {equipmentCount}");
                    }
                Static_txt.Text = results.ToString();
                
            }
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            UpdateCompletedOrders();
            UpdateAvgTime();
            UpdateFaults();
        }
    }
}
