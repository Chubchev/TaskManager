
using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using TaskManager.Model;

namespace TaskManager.Repository
{
    public class TaskRepository
    {
        public static string? ConnectionString { get; set; }
        /// <summary>Список заданий</summary>
        /// <returns></returns>
        internal List<MyTask> GetTasks()
        {
            List<MyTask> Tasks = null;
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                Tasks = dbConnection.Query<MyTask>(@"select TaskID, Date, Name, Status from test_chub.dbo.Tasks").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения списка задач: " + ex.Message);
            }
            return Tasks;
        }
        internal int AddTask(MyTask task)
        {
            int MaxTaskID = 0;
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                MaxTaskID = dbConnection.QuerySingle<int>(@"select Max(TaskID) from test_chub.dbo.Tasks");
                MaxTaskID++;
                dbConnection.Execute($"insert test_chub.dbo.Tasks select {MaxTaskID}, '{DateToSQLFormat(task.Date)}', '{task.Name}', {task.Status}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка добавления задачи в базу данных: " + ex.Message);
            }
            return MaxTaskID;
        }
        private static string DateToSQLFormat(DateTime Date) => Date.ToString("yyyy-MM-dd HH:mm");

        internal int UpdateTask(int TaskID, MyTask task)
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
        internal int DeleteTask(int TaskID)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
                dbConnection.Execute($"delete from test_chub.dbo.Tasks where TaskID={TaskID}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка удаления задачи в базе данных: " + ex.Message);
            }
            return TaskID;
        }

    }
}