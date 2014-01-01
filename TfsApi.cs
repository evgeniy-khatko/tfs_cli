﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    interface TfsApi
    {
        ITestManagementTeamProject GetProject(string name);
        ITestPlan GetTestPlan(ITestManagementTeamProject project, string testplan_name);
        ITestConfiguration GetTestConfiguration(ITestManagementTeamProject project, string config_name);
        ITestRun CreateRun(ITestPlan plan, string title, string comment, string buildNum, string attach);
        String UpdateResult(ITestCaseResult result, string outcome, string duration, string comment, string failure_type, string error_message, string attach);
    }
}
