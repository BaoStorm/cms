﻿using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "投票", Description = "通过 stl:vote 标签在模板中实现投票功能")]
    public class StlVote
    {
        private StlVote() { }
        public const string ElementName = "stl:vote";

        public static SortedList<string, string> AttributeList => null;

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            string inputTemplateString;
            string loadingTemplateString;
            string successTemplateString;
            string failureTemplateString;
            StlInnerUtility.GetTemplateLoadingYesNo(contextInfo.InnerXml, out inputTemplateString, out loadingTemplateString, out successTemplateString, out failureTemplateString);

            return ParseImpl(pageInfo, contextInfo, inputTemplateString);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string inputTemplateString)
        {
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            pageInfo.AddPageScriptsIfNotExists("SiteServer.CMS.Parser.StlElement",
                $@"<link href=""{SiteFilesAssets.Vote.GetStyleUrl(pageInfo.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
");

            var contentInfo = contextInfo.ContentInfo as VoteContentInfo;
            if (contentInfo == null && contextInfo.ContentId > 0)
            {
                //contentInfo = DataProvider.VoteContentDao.GetContentInfo(pageInfo.PublishmentSystemInfo.AuxiliaryTableForVote, contextInfo.ContentId);
                contentInfo = VoteContent.GetContentInfo(pageInfo.PublishmentSystemInfo.AuxiliaryTableForVote, contextInfo.ContentId);
            }

            if (contentInfo != null)
            {
                var voteTemplate = new VoteTemplate(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo);
                var contentBuilder = new StringBuilder(voteTemplate.GetTemplate(inputTemplateString));

                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
                parsedContent = contentBuilder.ToString();
            }

            return parsedContent;
        }
    }
}
