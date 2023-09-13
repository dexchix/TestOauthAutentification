using System;

namespace CommitsManager.Domain.Entities
{
    public class Commit
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public string Author { get; set; }

        public int RepositoryId { get; set; }

        public Repository Repository { get; set; }
    }
}
