namespace SplitExpense.SharedResource
{
    public static class ApiPaths
    {
        public const string ApiPrefix = "api/v1/[controller]/";
        public const string GroupCreate = ApiPrefix + "create";
        public const string GroupGetAll = ApiPrefix + "get/all";
        public const string GroupGetById = ApiPrefix + "get/{id}";
        public const string GroupUpdate = ApiPrefix + "update";
        public const string GroupDelete = ApiPrefix + "delete/{id}";
        public const string GroupSearch = ApiPrefix + "search";

    }
}
