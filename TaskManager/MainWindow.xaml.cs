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
using System.Data;

namespace TaskManager
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<string> Priority = new List<string>(new string[] { "niski", "normalny", "wysoki" });
        static List<string> Status = new List<string>(new string[] { "nowy", "w realizacji", "zakończony" });
        public MainWindow()
        {
            InitializeComponent();
            comboBoxPriority.ItemsSource = Priority;
            comboBoxStatus.ItemsSource = Status;
            LoadDataBase();
        }


        private void textBoxTask_GotFocus(object sender, RoutedEventArgs e) //Wyczyszczenie pola do wpisywania treści zadania
        {
            if (textBoxTask.Text == "Nowe zadanie...") //Zabezpieczenie przed każdorazowym czyszczeniem
            {
                textBoxTask.Text = string.Empty;
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)  //Obsługa naciśnięcia przycisku "Add"
        {
            if (textBoxTask.Text != null && textBoxTask.Text.Length < 1000 && datePickerDate.SelectedDate != null)
            {
                AddToDataBase();
                LoadDataBase();
            }
            else
            {
                MessageBox.Show("Fill all boxes");
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridTasks.SelectedItems[0];
                SqlConnection Connection = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=TaskManager;Trusted_Connection=True;");
                Connection.Open();
                SqlCommand Command = new SqlCommand("delete from Tasks where Task = '@Task'", Connection);
                Command.Parameters.Add(new SqlParameter("Task", row[0].ToString()));
                Command.ExecuteNonQuery();
                Connection.Close();
                MessageBox.Show("Item deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            LoadDataBase();
        }

        public void LoadDataBase() //Pobranie i wyświetlenie wartości z bazy danych
        {
            try
            {
                SqlConnection Connection = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=TaskManager;Trusted_Connection=True;");
                Connection.Open();
                SqlCommand Command = new SqlCommand("select * from Tasks", Connection);
                SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                DataTable dt = new DataTable("Tasks");
                DataAdapter.Fill(dt);
                dataGridTasks.ItemsSource = dt.DefaultView;
                Connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void AddToDataBase() //Dodanie nowego zadania
        {
            try
            {
                SqlConnection Connection = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=TaskManager;Trusted_Connection=True;");
                Connection.Open();
                SqlCommand Command = new SqlCommand("insert into Tasks values (@Task, @Priority, @Date, @Status)", Connection);
                Command.Parameters.Add(new SqlParameter("Task", textBoxTask.Text));
                Command.Parameters.Add(new SqlParameter("Priority", comboBoxPriority.SelectedValue.ToString()));
                Command.Parameters.Add(new SqlParameter("Date", datePickerDate.SelectedDate));
                Command.Parameters.Add(new SqlParameter("Status", comboBoxStatus.SelectedValue.ToString()));
                Command.ExecuteNonQuery();
                Connection.Close();
                MessageBox.Show("New item added");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
