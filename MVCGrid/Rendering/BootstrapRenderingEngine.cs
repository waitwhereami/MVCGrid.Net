﻿using MVCGrid.Interfaces;
using MVCGrid.Models;
using MVCGrid.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MVCGrid.Rendering
{
    public class BootstrapRenderingEngine : IMVCGridRenderingEngine
    {
        private string DefaultTableCss;
        private string HtmlImageSortAsc;
        private string HtmlImageSortDsc;
        private string HtmlImageSort;

        public const string SettingNameTableClass = "TableClass";

        public BootstrapRenderingEngine()
        {
            DefaultTableCss = "table table-striped table-bordered";
        }

        public void PrepareResponse(HttpResponse response)
        {
        }

        public bool AllowsPaging
        {
            get { return true; }
        }

        public void Render(RenderingModel model, GridContext gridContext, TextWriter outputStream)
        {
            HtmlImageSortAsc = String.Format("<img src='{0}/sortup.png' class='pull-right' />", model.HandlerPath);
            HtmlImageSortDsc = String.Format("<img src='{0}/sortdown.png' class='pull-right' />", model.HandlerPath);
            HtmlImageSort = String.Format("<img src='{0}/sort.png' class='pull-right' />", model.HandlerPath);

            string tableCss = gridContext.GridDefinition.GetAdditionalSetting<string>(SettingNameTableClass, DefaultTableCss);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.AppendFormat("<table id='{0}'", model.TableHtmlId);
            AppendCssAttribute(tableCss, sbHtml);
            sbHtml.Append(">");

            RenderHeader(model, sbHtml);

            if (model.Rows.Count > 0)
            {
                RenderBody(model, sbHtml);
            }
            else
            {
                sbHtml.Append("<tbody>");
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td colspan='{0}'>", model.Columns.Count());
                sbHtml.Append(model.NoResultsMessage);
                sbHtml.Append("</td>");
                sbHtml.Append("</tr>");
                sbHtml.Append("</tbody>");
            }
            sbHtml.AppendLine("</table>");

            RenderPaging(model, sbHtml);

            outputStream.Write(sbHtml.ToString());
            outputStream.Write(model.ClientDataTransferHtmlBlock);
        }

        private void AppendCssAttribute(string classString, StringBuilder sbHtml)
        {
            if (!String.IsNullOrWhiteSpace(classString))
            {
                sbHtml.Append(String.Format(" class='{0}'", classString));
            }
        }

        private void AppendOnClickCall(string functionName, string functionParameter, StringBuilder sbHtml)
        {
            if (!String.IsNullOrWhiteSpace(functionName))
            {
                if (!String.IsNullOrWhiteSpace(functionParameter))
                {
                    sbHtml.Append(String.Format(" onclick='{0}(\"{1}\")'", functionName, functionParameter));
                }
                else
                {
                    sbHtml.Append(String.Format(" onclick='{0}'", functionName));
                }
            }
        }

        private void RenderBody(Models.RenderingModel model, StringBuilder sbHtml)
        {
            sbHtml.AppendLine("<tbody>");

            foreach (var row in model.Rows)
            {
                sbHtml.Append("<tr");
                AppendCssAttribute(row.CalculatedCssClass, sbHtml);
                AppendOnClickCall(row.SelectedRowFunction, row.SelectedRowParameter, sbHtml);
                sbHtml.AppendLine(">");

                foreach (var col in model.Columns)
                {
                    var cell = row.Cells[col.Name];

                    sbHtml.Append("<td");
                    AppendCssAttribute(cell.CalculatedCssClass, sbHtml);
                    sbHtml.Append(">");
                    sbHtml.Append(cell.HtmlText);
                    sbHtml.Append("</td>");
                }
                sbHtml.AppendLine("  </tr>");
            }

            sbHtml.AppendLine("</tbody>");
        }

        private void RenderHeader(Models.RenderingModel model, StringBuilder sbHtml)
        {
            sbHtml.AppendLine("<thead>");
            sbHtml.AppendLine("  <tr>");
            foreach (var col in model.Columns)
            {
                sbHtml.Append("<th");

                if (!String.IsNullOrWhiteSpace(col.Onclick))
                {
                    sbHtml.Append(" style='cursor: pointer;'");
                    sbHtml.AppendFormat(" onclick='{0}'", col.Onclick);
                }
                sbHtml.Append(">");

                sbHtml.Append(col.HeaderText);

                if (col.SortIconDirection.HasValue)
                {
                    switch (col.SortIconDirection)
                    {
                        case Models.SortDirection.Asc:
                            sbHtml.Append(" ");
                            sbHtml.Append(HtmlImageSortAsc);
                            break;
                        case Models.SortDirection.Dsc:
                            sbHtml.Append(" ");
                            sbHtml.Append(HtmlImageSortDsc);
                            break;
                        case Models.SortDirection.Unspecified:
                            sbHtml.Append(" ");
                            sbHtml.Append(HtmlImageSort);
                            break;
                    }
                }
                sbHtml.AppendLine("</th>");
            }
            sbHtml.AppendLine("  </tr>");
            sbHtml.AppendLine("</thead>");
        }

        private void RenderPaging(Models.RenderingModel model, StringBuilder sbHtml)
        {
            if (model.PagingModel == null)
            {
                return;
            }

            Models.PagingModel pagingModel = model.PagingModel;

            sbHtml.Append("<div class=\"row\">");
            sbHtml.Append("<div class=\"col-xs-6\">");
            sbHtml.AppendFormat(model.SummaryMessage,
                pagingModel.FirstRecord, pagingModel.LastRecord, pagingModel.TotalRecords
                );
            sbHtml.Append("</div>");


            sbHtml.Append("<div class=\"col-xs-6\">");
            int pageToStart;
            int pageToEnd;
            pagingModel.CalculatePageStartAndEnd(5, out pageToStart, out pageToEnd);

            sbHtml.Append("<ul class='pagination pull-right' style='margin-top: 0;'>");

            sbHtml.Append("<li");
            if (pageToStart == pagingModel.CurrentPage)
            {
                sbHtml.Append(" class='disabled'");
            }
            sbHtml.Append(">");

            sbHtml.AppendFormat("<a href='#' aria-label='{0}' ", model.PreviousButtonCaption);
            if (pageToStart < pagingModel.CurrentPage && pagingModel.PageLinks.Count > (pagingModel.CurrentPage - 1))
            {
                sbHtml.AppendFormat("onclick='{0}'", pagingModel.PageLinks[pagingModel.CurrentPage - 1]);
            }
            else
            {
                sbHtml.AppendFormat("onclick='{0}'", "return false;");
            }
            sbHtml.Append(">");
            sbHtml.AppendFormat("<span aria-hidden='true'>&laquo; {0}</span></a></li>", model.PreviousButtonCaption);

            for (int i = pageToStart; i <= pageToEnd; i++)
            {
                sbHtml.Append("<li");
                if (i == pagingModel.CurrentPage)
                {
                    sbHtml.Append(" class='active'");
                }
                sbHtml.Append(">");
                sbHtml.AppendFormat("<a href='#' onclick='{0}'>{1}</a></li>", pagingModel.PageLinks[i], i);
            }


            sbHtml.Append("<li");
            if (pageToEnd == pagingModel.CurrentPage)
            {
                sbHtml.Append(" class='disabled'");
            }
            sbHtml.Append(">");

            sbHtml.AppendFormat("<a href='#' aria-label='{0}' ", model.NextButtonCaption);
            if (pageToEnd > pagingModel.CurrentPage)
            {
                sbHtml.AppendFormat("onclick='{0}'", pagingModel.PageLinks[pagingModel.CurrentPage + 1]);
            }
            else
            {
                sbHtml.AppendFormat("onclick='{0}'", "return false;");
            }
            sbHtml.Append(">");
            sbHtml.AppendFormat("<span aria-hidden='true'>{0} &raquo;</span></a></li>", model.NextButtonCaption);

            sbHtml.Append("</ul>");
            sbHtml.Append("</div>");
            sbHtml.Append("</div>");
        }


        public void RenderContainer(Models.ContainerRenderingModel model, TextWriter outputStream)
        {
            outputStream.Write(model.InnerHtmlBlock);
        }
    }
}
