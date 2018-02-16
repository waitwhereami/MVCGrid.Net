﻿using MVCGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCGrid.Models
{
    public class GridDefaults : IMVCGridDefinition
    {
        public GridDefaults()
        {
            PreloadData = true;
            QueryOnPageLoad = true;
            Paging = false;
            ItemsPerPage = 20;
            Sorting = false;
            DefaultSortColumn = null;
            DefaultSortDirection = SortDirection.Unspecified;
            NoResultsMessage = "No results.";
            NextButtonCaption = "Next";
            PreviousButtonCaption = "Previous";
            SummaryMessage = "Showing {0} to {1} of {2} entries";
            ProcessingMessage = "Processing";
            ClientSideLoadingMessageFunctionName = null;
            ClientSideLoadingCompleteFunctionName = null;
            Filtering = false;
            SelectedRowFunction = String.Empty;
            //RenderingEngine = typeof(MVCGrid.Rendering.BootstrapRenderingEngine);
            TemplatingEngine = typeof(MVCGrid.Templating.SimpleTemplatingEngine);
            AdditionalSettings = new Dictionary<string, object>();
            RenderingMode = Models.RenderingMode.RenderingEngine;
            ViewPath = "~/Views/MVCGrid/_Grid.cshtml";
            ContainerViewPath = null;
            ErrorMessageHtml= @"<div class=""alert alert-warning"" role=""alert"">There was a problem loading the grid.</div>";
            AdditionalQueryOptionNames = new HashSet<string>();
            PageParameterNames = new HashSet<string>();
            AllowChangingPageSize = false;
            MaxItemsPerPage = null;
            AuthorizationType = Models.AuthorizationType.AllowAnonymous;

            RenderingEngines = new ProviderSettingsCollection();
            RenderingEngines.Add(new ProviderSettings("BootstrapRenderingEngine", "MVCGrid.Rendering.BootstrapRenderingEngine, MVCGrid"));
            RenderingEngines.Add(new ProviderSettings("Export", "MVCGrid.Rendering.CsvRenderingEngine, MVCGrid"));
            DefaultRenderingEngineName = "BootstrapRenderingEngine";
        }

        public bool PreloadData { get; set; }
        public bool QueryOnPageLoad { get; set; }
        public bool Paging { get; set; }
        public int ItemsPerPage { get; set; }
        public bool Sorting { get; set; }
        public string DefaultSortColumn { get; set; }
        public SortDirection DefaultSortDirection { get; set; }

        public string NoResultsMessage { get; set; }
        public string NextButtonCaption { get; set; }
        public string PreviousButtonCaption { get; set; }
        public string SummaryMessage { get; set; }
        public string ProcessingMessage { get; set; }

        public string ClientSideLoadingMessageFunctionName { get; set; }
        public string ClientSideLoadingCompleteFunctionName { get; set; }
        public bool Filtering { get; set; }
        public string SelectedRowFunction { get; set; }

        [Obsolete("RenderingEngine is obsolete. Please user RenderingEngines and DefaultRenderingEngineName")]
        public Type RenderingEngine {
            get
            {
                if (RenderingEngines[DefaultRenderingEngineName] == null)
                {
                    return null;
                }
                string typeName = RenderingEngines[DefaultRenderingEngineName].Type;

                Type t = Type.GetType(typeName, true);
                return t;
            }
            set {
                string fullyQualifiedName = value.AssemblyQualifiedName;
                string name = value.Name;

                RenderingEngines.Add(new ProviderSettings(name, fullyQualifiedName));
                DefaultRenderingEngineName = name;
            }
        }

        public Type TemplatingEngine { get; set; }
        public Dictionary<string, object> AdditionalSettings { get; set; }
        public RenderingMode RenderingMode { get; set; }
        public string ViewPath { get; set; }
        public string ContainerViewPath { get; set; }
        public string QueryStringPrefix { get; set; }

        public IEnumerable<IMVCGridColumn> GetColumns()
        {
            throw new NotImplementedException();
        }

        public string ErrorMessageHtml { get; set; }


        public HashSet<string> AdditionalQueryOptionNames { get; set; }
        public HashSet<string> PageParameterNames { get; set; }

        public bool AllowChangingPageSize { get; set; }
        public int? MaxItemsPerPage { get; set; }


        public T GetAdditionalSetting<T>(string name, T defaultValue)
        {
            throw new NotImplementedException();
        }

        public AuthorizationType AuthorizationType { get; set; }

        public ProviderSettingsCollection RenderingEngines { get; set; }
        public string DefaultRenderingEngineName { get; set; }
    }
}
