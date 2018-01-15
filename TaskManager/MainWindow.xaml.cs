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
        static List<string> Priority = new List<string>(new string[] { "niski", "normalny", "wysoki" });    //lista priorytetów do wyboru
        static List<string> Status = new List<string>(new string[] { "nowy", "w realizacji", "zakończony" });   //lista statusów do wyboru
        String ConnectionString = @"Server=localhost\SQLEXPRESS;Database=TaskManager;Trusted_Connection=True;";
        public MainWindow()
        {
            InitializeComponent();
            comboBoxPriority.ItemsSource = Priority;    //przypisanie listy do kontrolki
            comboBoxStatus.ItemsSource = Status;    //przypisanie listy do kontrolki
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
            if (textBoxTask.Text != null && textBoxTask.Text.Length < 1000 && datePickerDate.SelectedDate != null) //zabezpieczenie przed dodaniem pustego wydarzenia, dłuższego niż 1000 znaków o raz bez daty 
            {
                AddToDataBase();
                LoadDataBase();
            }
            else
            {
                MessageBox.Show("Fill all boxes");
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e) //Obsługa naciśnięcia przycisku "Delete"
        {
            DeleteFromDataBase();
            LoadDataBase();
        }

        private void dataGridTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)  //Podwójne wciśniecie wiersza pozwala na jego edycje
        {
                DataRowView row = (DataRowView)dataGridTasks.SelectedItems[0];
                textBoxTask.Text = row[1].ToString();
                comboBoxPriority.SelectedItem = row[2].ToString();
                datePickerDate.SelectedDate = (DateTime)row[3];
                comboBoxStatus.SelectedItem = row[4].ToString();
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataBase();
            LoadDataBase();
        }

        public void LoadDataBase() //Pobranie i wyświetlenie wartości z bazy danych
        {
            try
            {
                SqlConnection Connection = new SqlConnection(ConnectionString);
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
                SqlConnection Connection = new SqlConnection(ConnectionString);
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

        public void DeleteFromDataBase()    //Usunięcie wiersza o danym indeksie
        {
            try
            {
                DataRowView row = (DataRowView)dataGridTasks.SelectedItems[0];
                SqlConnection Connection = new SqlConnection(ConnectionString);
                Connection.Open();
                SqlCommand Command = new SqlCommand("delete from Tasks where ID = @number", Connection);
                Command.Parameters.Add(new SqlParameter("number", row[0]));
                Command.ExecuteNonQuery();
                Connection.Close();
                MessageBox.Show("Item deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void UpdateDataBase()
        {
            try
            {
                DataRowView row = (DataRowView)dataGridTasks.SelectedItems[0];
                SqlConnection Connection = new SqlConnection(ConnectionString);
                Connection.Open();
                SqlCommand Command = new SqlCommand("update Tasks set Task = @Task, Priority = @Priority, Date = @Date, Status = @Status where ID = @number", Connection);
                Command.Parameters.Add(new SqlParameter("number", row[0]));
                Command.Parameters.Add(new SqlParameter("Task", textBoxTask.Text));
                Command.Parameters.Add(new SqlParameter("Priority", comboBoxPriority.SelectedValue.ToString()));
                Command.Parameters.Add(new SqlParameter("Date", datePickerDate.SelectedDate));
                Command.Parameters.Add(new SqlParameter("Status", comboBoxStatus.SelectedValue.ToString()));
                Command.ExecuteNonQuery();
                Connection.Close();
                MessageBox.Show("Item updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

    }
}
