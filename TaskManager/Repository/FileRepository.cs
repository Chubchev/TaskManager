
using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using TaskManager.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.IO;
using static System.Net.WebRequestMethods;

namespace TaskManager.Repository
{
    public class FileRepository
    {
        private string? ConnectionString { get { return TaskRepository.ConnectionString; } }

        /// <summary>Список файлов (содержимое (Data) не читается для быстродействия)</summary>
        /// <param name="taskID">Идентификатор задания</param>
        /// <returns></returns>
        internal IEnumerable<MyFile> GetFiles(int taskID)
        {
            List<MyFile> Files = null;
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                Files = dbConnection.Query<MyFile>($"select FileID, TaskID, Name from test_chub.dbo.Files where TaskID={taskID}").ToList();
                if (!Files.Any())
                    throw new Exception($"Задание {taskID} не содержит файлов");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения списка файлов: " + ex.Message);
            }
            return Files;
        }

        /// <summary>Получение файла по идентификатору</summary>
        /// <param name="taskID">Идентификатор задания</param>
        /// <returns></returns>
        internal MyFile? GetFileByID(int FileID)
        {
            MyFile? myFile = null;
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                var FileInfo = dbConnection.QuerySingleOrDefault<MyFile>($"select fileID, taskID, Name, Data from test_chub.dbo.Files where FileID={FileID}");
                myFile = dbConnection.QuerySingleOrDefault<MyFile>($"select fileID, taskID, Name, Data from test_chub.dbo.Files where FileID={FileID}");
                if (myFile==null)
                    throw new Exception($"Файл с идентификатором {FileID} не найден");
                //сохранение файла на диск
                string dir = Environment.CurrentDirectory,
                    fileName = Path.Combine(dir, "Temp", myFile.Name);
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.Write(myFile.Data, 0, myFile.Data.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения списка файлов: " + ex.Message);
            }
            return myFile;
        }
        internal int AddFile(int TaskID, IFormFile file)
        {
            byte[] fileData = new byte[file.Length];
            int DBTaskID = 0, DBFileID, MaxFileID = 0;
            using (var fstream = file.OpenReadStream())
            {
                fstream.Read(fileData, 0, fileData.Length);
            }
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                DBTaskID = dbConnection.QuerySingleOrDefault<int>($"select TaskID from test_chub.dbo.Tasks where TaskID={TaskID}");
                if (DBTaskID == 0)
                    throw new Exception($"Задание с идентификатором {TaskID} отсутствует в таблице Tasks");
                DBFileID = dbConnection.QuerySingleOrDefault<int>($"select FileID from test_chub.dbo.Files where TaskID={TaskID} and Name='{file.FileName}'");
                if (DBFileID > 0)
                    throw new Exception($"Файл {file.FileName} уже есть в задании {TaskID}");
                MaxFileID = dbConnection.QuerySingleOrDefault<int>($"select FileID from test_chub.dbo.Files");
                MaxFileID++;
                dbConnection.Execute("insert test_chub.dbo.Files values (@FileID, @TaskID, @FileName, @fileData)",
                    new { FileID = MaxFileID , TaskID, FileName = file.FileName, fileData});
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка добавления файла в базу данных: " + ex.Message);
            }
            return fileData.Length;
        }
        /*internal int UpdateTask(int TaskID, MyTask task)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                dbConnection.Execute($"update test_chub.dbo.Tasks set Date = '{DateToSQLFormat(task.Date)}', Name = '{task.Name}', Status = {task.Status} where TaskID={TaskID}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка обновления задачи в базе данных: " + ex.Message);
            }
            return TaskID;
        }
       */ internal int DeleteFile(int FileID)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                dbConnection.Execute($"delete from test_chub.dbo.Files where FileID={FileID}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка удаления файла в базе данных: " + ex.Message);
            }
            return FileID;
        }
    }
}