using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;
using System.Text.RegularExpressions;

namespace testdatabase
{
    partial class Form1 : Form //разделяемый класс на части, описание для формы в другом файле
    {
        public Form1()
        {
            InitializeComponent();
        }


        
        // https://www.muztorg.ru/category/akusticheskie-udarnye (276)
        // https://www.muztorg.ru/category/skripki (59)

        private void button1_Click(object sender, EventArgs e) //кнопка Start
        {
            //<a href="/product/A12890">YAMAHA F310</a>
            string name = "<div class=\"title\">(.*?)<a href=\"/product/(.*?)\">(.*?)</a>";

            //<meta itemprop="price" content="20890">
            string price = "<meta itemprop=\"price\" content=\"(.*?)\">";

            //<a href="/product/A130645" itemprop="image"..><img data-src="https://..." alt="FENDER CD-60 Black" title=".." class="img-responsive" itemprop="image" src="https://...">
            string image = "<a href=\"/product/(.*?)\" itemprop=\"image\" (.*?)>(.*?)<img (.*?)src=\"(.*?)\"(.*?)>";

            //<div class="product-existence"><div class=" pre-order-now" data-toggle="tooltip" data-placement="auto" data-original-title=".." title="">
            string exist = "<div class=\"product-existence\">(.*?)<div class=\"(.*?)\"(.*?)";

            List<string> info = new List<string>() { name, price, image, exist };


            listView.Columns.Add("Артикул, название товара", 150, HorizontalAlignment.Left);
            listView.Columns.Add("Цена товара (rub)", 100, HorizontalAlignment.Left);
            listView.Columns.Add("URL изображения", 400, HorizontalAlignment.Left);
            listView.Columns.Add("Наличие в магазине", 150, HorizontalAlignment.Left);
            listView.View = View.Details;
            listView.GridLines = true; //показ сетки таблицы



            //<li class="next"><a href="/category/akusticheskie-gitary?in-stock=1&amp;pre-order=1&amp;page=37" data-page="36">»</a></li>
            //<li class="next-disabled"></li>
            Regex rgx = new Regex(@"<li class=""next"">(.*?)"); // новый экземляр класса Regex


            int n = 1; //считает страницы
            int flag = 1; // выход из цикла while 
            int j1 = 0;  int j2 = 0; // счетчики для подэлементов 3 и 4 столбца

            

            while (flag == 1)
            {
                try
                {
                    WebClient w = new WebClient(); // новый экземляр класса WebClient
                    string page = w.DownloadString(textBox1.Text + "?in-stock=1&pre-order=1&page=" + n);
               
                

                for (int i = 0; i < info.Count; i++)
                {
                    
                    int j = 0; // cчетчик для подэлементов 2 столбца
                    
                    foreach (Match match in Regex.Matches(page, info[i], RegexOptions.Singleline | RegexOptions.Compiled))
                            // для каждого объекта matches из коллекции Match
                    {

                        if (i == 0)
                        {
                            string info1 = match.Groups[2].Value + " " + match.Groups[3].Value; // значение из пронумерованной коллекции групп
                            list1.Items.Add(info1);

                            if (n == 1 && radioButton1.Checked) { j1++; j2++; }
                            
                        }
                        else if (i == 1)
                        {
                            string info2 = match.Groups[1].Value;
                            info2 = info2.Substring(0, info2.IndexOf('"'));
                            listView.Items.Add(list1.Items[j].ToString()).SubItems.Add(info2); // добавление элемента в 1 столбец и его подэлемента во 2 столбец

                            j++;

                        }
                        else if (i == 2)
                        {
                            
                            string info3 = match.Groups[5].Value;

                            if (n > 1) 
                            {
                                listView.Items[j1].SubItems.Add(info3); // подэлемент 3 столбца

                                j1++;

                            }
                            else
                            {
                                listView.Items[j].SubItems.Add(info3);

                                j++;

                            }
                        }
                        else
                        {

                            string info4 = match.Groups[2].Value;

                            if (n > 1)
                            {
                                listView.Items[j2].SubItems.Add(info4); //подэлемент 4 столбца

                                j2++;
                            }
                            else
                            {
                                listView.Items[j].SubItems.Add(info4);

                                j++;

                            }
                        }
                        
                    }
                }

                
                 MatchCollection matches = rgx.Matches(page); // коллекция объектов matches, метод находит все вхождения строки page
                 if (matches.Count > 0 && radioButton1.Checked)
                 {
                     list1.Items.Clear();
                     n++;
                 }
                 else if (matches.Count == 0 && n == 1)
                 {
                        flag = 0;
                        MessageBox.Show("Unable to parse this URL", "Parsing", MessageBoxButtons.OK); //требуется изменение Regex 
                 }
                 else
                 {
                     flag = 0;
                     MessageBox.Show("Success!", "Parsing", MessageBoxButtons.OK);
                 }

                }
                catch
                {
                    flag = 0;
                    MessageBox.Show("Incorrect input", "Parsing", MessageBoxButtons.OK); 

                }

            }
        }

        private void button2_Click(object sender, EventArgs e) // кнопка Save Data
        {
            
            SQLiteConnection sqliteConnection;

            sqliteConnection = Database.CreateConnection(); // содержит строку подключения
            Database.CreateTable(sqliteConnection);
            Database.InsertData(sqliteConnection,listView);

        }

        
    }

    

}
