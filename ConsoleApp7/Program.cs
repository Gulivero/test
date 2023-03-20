using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SelfHost;

namespace ConsoleApp7
{
    class Program
    {

        static Random rnd = new Random();
        static async void Send()
        {
            List<string> responses = new List<string>();

            HttpClient client = new HttpClient();

            client.Timeout = TimeSpan.FromMinutes(30);

            //http://web-test/P03430/api/primaryDoc/getPrimaryDocFilter?filterName=acceptedAccounting&periodFrom=01.10.2022&periodTo=23.01.2023&personalNumber=034296&transactionItemId=null&isManual=false

            //client.BaseAddress = new Uri("http://localhost:40378/api/primaryDoc/getPrimaryDocFilter?filterName=acceptedAccounting&periodFrom=01.10.2022&periodTo=23.01.2023&personalNumber=000598&transactionItemId=null&isManual=false");

            //client.BaseAddress = new Uri("http://localhost:40378/api/primaryDoc/getPrimaryDocFilter?filterName=acceptedAccounting&periodFrom=01.10.2022&periodTo=23.01.2023&personalNumber=000598&transactionItemId=null&isManual=false");

            client.BaseAddress = new Uri(
                "http://127.0.0.5:55555/api/TableData?path=\\\\NW2\\DBF\\TEST_DBF\\P91050\\&tableName=r233.dbf&" +
                "query=SELECT * FROM r233&app=03430");

            var res = await client.GetAsync(client.BaseAddress);
        }

        static bool Exist(string path)
        {
            return File.Exists(path);
        }

        public static DataTable GetDataTable(string path, string tableName, string query, string app = null)
        {
            int id = 125 + rnd.Next(1, 1200) + 10;
            FileInfo file = new FileInfo(path + tableName);

            using (OleDbConnection connection = new OleDbConnection())
            {
                try
                {
                    if (!file.Exists)
                    {
                        throw new Exception("Ошибка при получении данных о таблице: Указанный файл не найден. Проверьте правильность указанного пути к файлу и попробуйте снова.");
                    }

                    
                    string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + path;
                    Console.WriteLine($"_______________________Start - {id}______________________________");
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    Console.WriteLine($"===============Connection Open {id}=============");
                    DataTable table = new DataTable();
                    OleDbCommand command = new OleDbCommand(query, connection);
                    Console.WriteLine($"ComandText {id}  : " + command.CommandText);
                    using (OleDbDataAdapter adapter =
                        new OleDbDataAdapter(command))
                    {
                        adapter.FillError += (sender, args) => Console.WriteLine("Ошибка при заполнении DataTable");
                        Console.WriteLine($"start filltable - {id}");
                        adapter.AcceptChangesDuringFill = true;
                        adapter.Fill(table);
                    }
                    Console.WriteLine($"finish filltable - {id}");
                    Console.WriteLine($"_______________________FINISH {id}______________________________");
                    return table;
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Message  id={id}  " + ex.Message + " InnerException  " + ex.InnerException + " HResult " + ex.HResult + " ex = " + ex);
                    // уменьшаем счетчик если запрос отрабол не корректно
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Ошибка при получении данных о таблице: " + ex.Message)
                    };
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                        Console.WriteLine("===============Connection Close=============");
                    }
                    OleDbConnection.ReleaseObjectPool();
                }
            }
        }

        static void Main(string[] args)
        {

            //string select = $"SELECT * FROM p160936";

            //string path = "D://dbf/";

            //string tablename = "p160936.dbf";

            //string app = "consoleApp";

            //List<Thread> threads = new List<Thread>();

            for (int i = 0; i < 25; i++)
            {
                Thread thread = new Thread(() => Task.Run(() =>
                {
                    Send();
                }));
                //threads.Add(thread);
                thread.Start();
            }
            //responses.ForEach(x => Console.WriteLine(x));
            //Console.WriteLine("1");
            //Console.WriteLine("2");


            //Document document = new Document();
            //document.Header = new Header
            //{
            //    Name = "Заголовок",
            //    Date = DateTime.Now,
            //    City = "Минск",
            //    DocumentName = "Заявление"
            //};
            //document.Body = new Body
            //{
            //    Name = "Текст документа",
            //    BodyText = "Прошу повысить мне зп на 300%!"
            //};
            //document.Footer = new Footer
            //{
            //    Name = "Footer",
            //    FooterText = "Подпись, печать, ФИО"
            //};

            //Builder builder = new ReportBuilder();

            Console.WriteLine("Запросы отправлены!");

            Console.Read();
        }
    }

    class Director
    {
        Builder builder;
        public Director(Builder builder)
        {
            this.builder = builder;
        }
        public void Construct(Header header, Body body, Footer footer)
        {
            builder.SetHeader(header);
            builder.SetBody(body);
            builder.SetFooter(footer);
        }
    }

    abstract class Builder
    {
        public Document Document { get; set; }
        public abstract void SetHeader(Header header);
        public abstract void SetBody(Body body);
        public abstract void SetFooter(Footer footer);
        public abstract Document GetDocument();
    }

    class Document
    {
        public Header Header { get; set; }
        public Body Body { get; set; }
        public Footer Footer { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Header != null)
            {
                string nameHeader = "<---" + Header?.Name + "---> \n";
                string date = Header.Date + "\t\t\t";
                string city = Header.City + "\n";
                string documentName = Header.DocumentName + "\n";
                builder.Append(nameHeader);
                builder.Append(date);
                builder.Append(city);
                builder.Append(documentName);
            }
            if (Body != null)
            {
                string name = "<---" + Body?.Name + "---> \n";
                builder.Append(name);
                builder.Append(Body.BodyText);
            }
            if (Footer != null)
            {
                string name = "<---" + Footer?.Name + "---> \n";
                builder.Append(name);
                builder.Append(Footer.FooterText);
            }

            return builder.ToString();
        }
    }

    class ReportBuilder : Builder
    {
        private readonly Document _document = new Document();
        public override void SetHeader(Header header)
        {
            _document.Header = header;
        }

        public override void SetBody(Body body)
        {
            _document.Body = body;
        }

        public override void SetFooter(Footer footer)
        {

        }

        public override Document GetDocument()
        {
            return _document;
        }
    }


    class ClaimBuilder : Builder
    {
        private readonly Document _document = new Document();
        public override void SetHeader(Header header)
        {

        }

        public override void SetBody(Body body)
        {
            _document.Body = body;
        }

        public override void SetFooter(Footer footer)
        {
            _document.Footer = footer;
        }

        public override Document GetDocument()
        {
            return _document;
        }
    }

    class Header
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string DocumentName { get; set; }
        public string City { get; set; }

    }
    class Body
    {
        public string Name { get; set; }
        public string BodyText { get; set; }
    }
    class Footer
    {
        public string Name { get; set; }
        public string FooterText { get; set; }
    }
}
