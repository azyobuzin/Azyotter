﻿/*
 * AssemblyVersionInfoGenerator
 * 
 * バージョン番号にGitのコミット数を追加します。
 */

/*
 * 参考：https://github.com/takeshik/build-tools/blob/master/UpdateVersionInfo/Program.cs
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitSharp.Core;

namespace Azyobuzi.Azyotter.AssemblyVersionInfoGenerator
{
    class Program
    {
        const string VersionBase = "0.1.*";

        const string OutText = @"// Generated by AssemblyVersionInfoGenerator

using System.Reflection;

[assembly: AssemblyVersion(""{0}"")]
";

        const string OutFile = "Properties\\VersionInfo.cs";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = args[0].TrimEnd('\\');//プロジェクトフォルダ

            using (var repo = Repository.Open(".."))
            {
                var count = WalkCommitTree(repo, repo.OpenCommit(repo.Head.ObjectId)).Count();
                var version = VersionBase.Replace("*", count.ToString());
                File.WriteAllText(OutFile, string.Format(OutText, version));
            }
        }

        private static IEnumerable<Commit> WalkCommitTree(Repository repository, Commit commit)
        {
            var commits = new List<ObjectId>()
            {
                commit.CommitId
            };
            EnumerableEx.Generate(
                commit.ParentIds,
                _ => _.Any(i => !commits.Contains(i)),
                _ => _.Where(i => !commits.Contains(i)).Do(commits.Add).SelectMany(i => repository.OpenCommit(i).ParentIds).ToArray(),
                _ => _
            ).ForEach(_ =>
            {
            });
            return commits
                .Select(repository.OpenCommit)
                .OrderByDescending(c => c.Committer.When);
        }
    }
}