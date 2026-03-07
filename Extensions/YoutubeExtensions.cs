using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Routing;
using Npgsql.Replication;

namespace SumaryYoutubeBackend.Extensions
{
    public static class YoutubeExtencions
    {
        public static string ExtractVideoId(this string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? url;
        }


    }
}