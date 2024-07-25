using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
namespace testdatabase
{
    public class Database
    {

        // доступ в одной сборке
        internal static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqliteConn;
            sqliteConn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True;"); // процесс нового подключения 
            try
            {
                sqliteConn.Open(); //метод открытия подключения, использует заданную строку

            }
            catch
            {
                MessageBox.Show("error", "SaveData", MessageBoxButtons.OK);
            }
            return sqliteConn;
        }

        internal static void CreateTable(SQLiteConnection sqliteConnection)
        {

                if (File.Exists("database.db")) //определяет существование файла
                {
                    SQLiteCommand sqliteCommand; // класс позволяет создавать запросы языка SQL
                    string createSQL = "CREATE TABLE SampleTable(Артикул_название VARCHAR, Цена INT, URL_изображения VARCHAR, Наличие_в_магазине VARCHAR)";
                    sqliteCommand = sqliteConnection.CreateCommand(); //создание новой команды, связывает с соединением
                    sqliteCommand.CommandText = createSQL; //в текст команды передается строка

                    sqliteCommand.ExecuteNonQuery(); //выполняет sql-выражение и возвращает количество измененных записей


            }
                else
                {
                    MessageBox.Show("error", "SaveData", MessageBoxButtons.OK);
                    return;
                }
   
        }


        internal static void InsertData(SQLiteConnection conn, ListView listView)
        {
            SQLiteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            for (int i = 0; i < listView.Items.Count; i++)
            {      
            
                sqliteCommand.CommandText = @"INSERT INTO SampleTable(Артикул_название,Цена,URL_изображения,Наличие_в_магазине) VALUES(@info1,@info2,@info3,@info4)";

            
                string info1 = listView.Items[i].Text;  //в строку передается текст элемента из списка         
                sqliteCommand.Parameters.AddWithValue("@info1", info1);
           
                int info2 = Convert.ToInt32(listView.Items[i].SubItems[1].Text);            
                sqliteCommand.Parameters.AddWithValue("@info2", info2); // добавляет параметр и его значение в коллекцию, возвращает созданный объект при выполнении команды
            
                string info3 = listView.Items[i].SubItems[2].Text;           
                sqliteCommand.Parameters.AddWithValue("@info3", info3);
            
                string info4 = listView.Items[i].SubItems[3].Text;         
                sqliteCommand.Parameters.AddWithValue("@info4", info4);

            
                sqliteCommand.ExecuteNonQuery();

            }
            
            MessageBox.Show("Success", "SaveData", MessageBoxButtons.OK);

            
        }

    }
}
