namespace TaskManager.Model
{
    public class MyTask
    {
        public int TaskID { get; }
        public DateTime Date { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; }
        public string StatusName
        {
            get
            {
                return Status switch
                {
                    0 => "Статус 0",
                    1 => "Статус 1",
                    2 => "Статус 2",
                    _ => "Не определён",
                };
            }
        }

    }
}
