using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.IO;

namespace WebApplication1.Util
{
    public class Util
    {
        public Util()
        {

        }

        public void InsertFileDB(string filePath)
        {
            Console.WriteLine($"\n{filePath}");



            GetInsertList(filePath);
        }

        public void GetInsertList(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);

            string connectionString = "Server=鯽魚的電腦;Database=ESMP;User Id=test;Password=test;";
            string insertHead = sr.ReadLine() + "\n";
            string insertCommand = insertHead;
            string line;
            int counter = 0;
            int rowCounter = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                while ((line = sr.ReadLine()) != null)
                {
                    insertCommand += line + "\n";
                    counter++;
                    if (counter == 1000)
                    {
                        insertCommand = insertCommand.TrimEnd('\n');
                        insertCommand = insertCommand.TrimEnd(',');
                        Console.WriteLine(insertCommand);
                        try
                        {
                            using (SqlCommand command = new SqlCommand(insertCommand, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 2627)
                            {
                                Console.WriteLine($"忽略重複鍵錯誤: {ex.Message}");
                            }
                            else
                            {
                                throw;
                            }
                        }
                        insertCommand = insertHead + "\n";
                        counter = 0;
                    }
                }

                if (counter > 0)
                {
                    insertCommand.TrimEnd(' ');
                    insertCommand.TrimEnd(',');
                }
                try
                {
                    using (SqlCommand command = new SqlCommand(insertCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        Console.WriteLine($"忽略重複鍵錯誤: {ex.Message}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }


    }
}
