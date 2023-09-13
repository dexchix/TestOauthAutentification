using System;

namespace CommitsManager.Domain.Entities
{
    public class CommitEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public string Author { get; set; }

        public int RepositoryId { get; set; }

        public RepositoryEntity Repository { get; set; }
    }
}
