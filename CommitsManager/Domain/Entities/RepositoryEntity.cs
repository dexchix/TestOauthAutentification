using System.Collections.Generic;

namespace CommitsManager.Domain.Entities
{
    public class RepositoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public ICollection<CommitEntity> Commits { get; set; }
    }
}
