namespace SplitExpense.SharedResource
{
    public static class ApiPaths
    {
        public const string ApiPrefix = "api/v1/[controller]/";
        #region Group APIs
        public const string GroupCreate = ApiPrefix + "create";
        public const string GroupGetAll = ApiPrefix + "get/all";
        public const string GroupGetRecent = ApiPrefix + "get/recent";
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
            public const string ExpenseGetAll = ApiPrefix + "get/all";
            public const string ExpenseSearch = ApiPrefix + "search";
        #endregion

        #region Contact API Path
        public const string ContactCreate = ApiPrefix + "create";
        public const string ContactAddInList = ApiPrefix + "add/in/contact/list/{id}";
        public const string ContactGetAll = ApiPrefix + "get/all";
        public const string ContactGetById = ApiPrefix + "get/{id}";
        public const string ContactUpdate = ApiPrefix + "update";
        public const string ContactDelete = ApiPrefix + "delete/{id}";
        public const string ContactSearch = ApiPrefix + "search";
        public const string ContactSearchUser = ApiPrefix + "search/user";
        #endregion

        #region GroupType
        public const string GroupTypeCreate = ApiPrefix + "create";
        public const string GroupTypeUpdate = ApiPrefix + "update";
        public const string GroupTypeDelete = ApiPrefix + "delete/{id}";
        public const string GroupTypeGetById = ApiPrefix + "get/{id}";
        public const string GroupTypeGetAll = ApiPrefix + "get/all";
        public const string GroupTypeSearch = ApiPrefix + "search";
        #endregion

        #region Auth
        public const string Register = ApiPrefix + "register";
        public const string Login = ApiPrefix + "login";
        public const string Logout = ApiPrefix + "logout";
        public const string ForgotPassword = ApiPrefix + "forgot-password";
        public const string ResetPassword = ApiPrefix + "reset-password";
        public const string ForgotUsername = ApiPrefix + "forgot-username";
        public const string UpdateUserProfile = ApiPrefix + "update/profile";
        public const string UpdateUserProfilePicture = ApiPrefix + "update/profile-picture";
        public const string DeleteUserProfilePicture = ApiPrefix + "delete/profile-picture";
        public const string DeleteUserProfile = ApiPrefix + "delete/profile";
        #endregion

        #region ExpenseActivity
        public const string ExpenseActivityCreate = ApiPrefix + "create";
        public const string ExpenseActivityGetAll = ApiPrefix + "get/all";
        public const string ExpenseActivitySearch = ApiPrefix + "search";
        #endregion

        #region Email Template API
        public const string EmailTemplateCreate = ApiPrefix + "create";
        public const string EmailTemplateGetAll = ApiPrefix + "get/all";
        public const string EmailTemplateSearch = ApiPrefix + "search";
        public const string EmailTemplateUpdate = ApiPrefix + "update";
        public const string EmailTemplateDelete = ApiPrefix + "delete/{id}";
        #endregion
    }
}
