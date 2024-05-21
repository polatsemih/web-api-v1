namespace VkBank.Domain.Contstants
{
    public class ResultMessages
    {
        // Menu
        public const string MenuNoDatas = "No menu datas found";
        public const string MenuNoData = "No menu data found for the given Id";

        public const string MenuCreated = "Menu created";
        public const string MenuCreateFailed = "Menu creation failed";

        public const string MenuUpdated = "Menu updated";
        public const string MenuUpdateFailed = "Menu update failed. Ensure that the given Id is valid.";

        public const string MenuDeleted = "Menu deleted";
        public const string MenuDeleteFailed = "Menu delete failed. Ensure that the given Id is valid.";

        public const string MenuIdNotExist = "Menu Id does not exist";
        public const string MenuParentIdNotExist = "Menu ParentId does not exist";
        public const string MenuScreenCodeNotExist = "Menu ScreenCode does not exist";

        public const string MenuRollbacked = "Menu rollbacked";
        public const string MenuRollbackFailed = "Menu rollback failed";
    }
}
