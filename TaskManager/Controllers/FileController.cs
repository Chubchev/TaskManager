using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Model;
using TaskManager.Repository;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private FileRepository fileRepository { get { return new FileRepository(); } }

        public FileController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }

        [HttpGet("File list by TaskID")]
        public IEnumerable<MyFile> Get(int TaskID)
        {
            return fileRepository.GetFiles(TaskID);
        }
        [HttpGet("File by FileID")]
        public MyFile? GetFile(int FileID)
        {
            return fileRepository.GetFileByID(FileID);
        }
        [HttpPost(Name = "AddFile"), DisableRequestSizeLimit]
        public int Post(int TaskID, IFormFile file)
        {
            return fileRepository.AddFile(TaskID, file);
        }
        [HttpDelete(Name = "DeleteFile")]
        public int Delete(int FileID)
        {
            return fileRepository.DeleteFile(FileID);
        }
    }
}
