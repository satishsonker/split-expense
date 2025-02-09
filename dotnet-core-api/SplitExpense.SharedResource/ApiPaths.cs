namespace SplitExpense.SharedResource
{
    public static class ApiPaths
    {
        public const string ApiPrefix = "api/v1/[controller]/";
        #region Group APIs
        public const string GroupCreate = ApiPrefix + "create";
        public const string GroupGetAll = ApiPrefix + "get/all";
        public const string GroupGetById = ApiPrefix + "get/{id}";
        public const string GroupUpdate = ApiPrefix + "update";
        public const string GroupDelete = ApiPrefix + "delete/{id}";
        public const string GroupSearch = ApiPrefix + "search";
        public const string GroupAddUser = ApiPrefix + "add/friend";
        #endregion

        #region SplitType APIs
        public const string SplitTypeCreate = ApiPrefix + "create";
        public const string SplitTypeGetAll = ApiPrefix + "get/all";
        public const string SplitTypeGetById = ApiPrefix + "get/{id}";
        public const string SplitTypeUpdate = ApiPrefix + "update";
        public const string SplitTypeDelete = ApiPrefix + "delete/{id}";
        public const string SplitTypeSearch = ApiPrefix + "search";
        #endregion


        #region Email APIs
        public const string EmailGetQueue = ApiPrefix + "get/queue";
        #endregion

        #region Expense API Path
            public const string ExpenseCreate = ApiPrefix + "create";
            public const string ExpenseUpdate = ApiPrefix + "update";
            public const string ExpenseDelete = ApiPrefix + "delete/{id}";
        #endregion

    }
}
