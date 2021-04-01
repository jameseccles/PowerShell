﻿using PnP.Core.Model.SharePoint;
using PnP.Framework.Utilities;
using PnP.PowerShell.Commands.Base;
using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Model;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PnP.PowerShell.Commands.Syntex
{
    [Cmdlet(VerbsLifecycle.Request, "PnPSyntexClassifyAndExtract")]
    public class RequestSyntexClassifyAndExtract : PnPWebCmdlet
    {
        const string ParameterSet_LIST = "List";
        const string Parameterset_FILE = "File";

        [Parameter(Mandatory = true, ParameterSetName = ParameterSet_LIST)]
        public ListPipeBind List;

        [Parameter(Mandatory = false, ParameterSetName = ParameterSet_LIST)]
        public SwitchParameter Force = false;

        [Parameter(Mandatory = true, ParameterSetName = Parameterset_FILE)]
        public string FileUrl;

        [Parameter(Mandatory = false, ParameterSetName = Parameterset_FILE)]
        public PnPBatch Batch;

        protected override void ExecuteCmdlet()
        {
            var serverRelativeUrl = string.Empty;
            var ctx = PnPConnection.Current.PnPContext;

            if (ParameterSpecified(nameof(List)))
            {
                IList list = List.GetList(ctx);

                var classifyAndExtractResults = list.ClassifyAndExtract(force: Force.IsPresent);

                List<Model.Syntex.SyntexClassifyAndExtractResult> classifyAndExtractResultsOutput = new List<Model.Syntex.SyntexClassifyAndExtractResult>();
                if (classifyAndExtractResults != null && classifyAndExtractResults.Any())
                {
                    foreach (var classifyAndExtractResult in classifyAndExtractResults)
                    {
                        classifyAndExtractResultsOutput.Add(new Model.Syntex.SyntexClassifyAndExtractResult()
                        {
                            Created = classifyAndExtractResult.Created,
                            DeliverDate = classifyAndExtractResult.DeliverDate,
                            ErrorMessage = classifyAndExtractResult.ErrorMessage,
                            Id = classifyAndExtractResult.Id,
                            Status = classifyAndExtractResult.Status,
                            StatusCode = classifyAndExtractResult.StatusCode,
                            TargetServerRelativeUrl = classifyAndExtractResult.TargetServerRelativeUrl,
                            TargetSiteUrl = classifyAndExtractResult.TargetSiteUrl,
                            TargetWebServerRelativeUrl = classifyAndExtractResult.TargetWebServerRelativeUrl,
                            WorkItemType = classifyAndExtractResult.WorkItemType,
                        });
                    }
                }

                WriteObject(classifyAndExtractResultsOutput, true);
            }
            else
            {
                ctx.Web.EnsureProperties(w => w.ServerRelativeUrl);

                if (!FileUrl.ToLower().StartsWith(ctx.Web.ServerRelativeUrl.ToLower()))
                {
                    serverRelativeUrl = UrlUtility.Combine(ctx.Web.ServerRelativeUrl, FileUrl);
                }
                else
                {
                    serverRelativeUrl = FileUrl;
                }

                var file = ctx.Web.GetFileByServerRelativeUrl(serverRelativeUrl);

                if (ParameterSpecified(nameof(Batch)))
                {
                    file.ClassifyAndExtractBatch(Batch.Batch);
                }
                else
                {
                    var classifyAndExtractResult = file.ClassifyAndExtract();

                    if (classifyAndExtractResult != null)
                    {
                        WriteObject(new Model.Syntex.SyntexClassifyAndExtractResult()
                        {
                            Created = classifyAndExtractResult.Created,
                            DeliverDate = classifyAndExtractResult.DeliverDate,
                            ErrorMessage = classifyAndExtractResult.ErrorMessage,
                            Id = classifyAndExtractResult.Id,
                            Status = classifyAndExtractResult.Status,
                            StatusCode = classifyAndExtractResult.StatusCode,
                            TargetServerRelativeUrl = classifyAndExtractResult.TargetServerRelativeUrl,
                            TargetSiteUrl = classifyAndExtractResult.TargetSiteUrl,
                            TargetWebServerRelativeUrl = classifyAndExtractResult.TargetWebServerRelativeUrl,
                            WorkItemType = classifyAndExtractResult.WorkItemType,
                        });
                    }
                }
            }
        }
    }
}
