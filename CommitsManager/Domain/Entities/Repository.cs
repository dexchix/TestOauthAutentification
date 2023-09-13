using System.Collections.Generic;

namespace CommitsManager.Domain.Entities
{
    public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public ICollection<Commit> Commits { get; set; }
    }
}
