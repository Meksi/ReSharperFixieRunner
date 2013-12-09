﻿using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class TaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        public TaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var nodeRunner = new NodeRunner(Server);
            nodeRunner.RunNode(node);
        }
    }
}
