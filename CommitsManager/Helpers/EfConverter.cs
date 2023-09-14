using CommitsManager.Domain.Entities;
using Octokit;
using System.Collections;
using System.Collections.Generic;

namespace CommitsManager.Helpers
{
    public class EfConverter
    {
        public static ICollection<CommitEntity> ConvertCommits(IEnumerable<GitHubCommit> commits, RepositoryEntity repository)
        {
            var listCommits = new List<CommitEntity>();

            foreach (var comm in commits)
            {
                var login = comm.Author == null? null : comm.Author.Login;
                listCommits.Add(new CommitEntity() { Author = login, Message = comm.Commit.Message, Repository = repository, DateCreate = comm.Commit.Author.Date, SHA = comm.Sha});
            }
            return listCommits;
        }
    }
}
