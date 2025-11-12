namespace JonasTodo.Api.Helpers
{
    public static class RouteNames
    {
        public const string ApiPrefix = "api";

        public static class Topics
        {
            public const string ControllerRoute = ApiPrefix + "/[controller]";
            public const string GetById = "GetTopicById";
        }

        public static class Subtopics
        {
            public const string ControllerRoute = ApiPrefix + "/[controller]";
            public const string GetById = "GetSubtopicById";
        }
    }
}