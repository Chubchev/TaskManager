using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Model;
using TaskManager.Repository;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private TaskRepository taskRepository { get { return new TaskRepository(); } }

        public TaskController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTask")]
        public IEnumerable<MyTask> Get()
        {
            return taskRepository.GetTasks();
        }
        [HttpPost(Name = "AddTask")]
        public int Post(MyTask task)
        {
            return taskRepository.AddTask(task);
        }
        [HttpPut(Name = "UpdateTask")]
        public int Put(int TaskID, MyTask task)
        {
            return taskRepository.UpdateTask(TaskID, task);
        }
        [HttpDelete(Name = "DeleteTask")]
        public int Delete(int TaskID)
        {
            return taskRepository.DeleteTask(TaskID);
        }
    }
}
