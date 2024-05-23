namespace VkBank.Domain.Contstants
{
    public class ResultMessages
    {
        // Menu
        public const string MenuNoDatas = "No menu datas found";
        public const string MenuNoData = "No menu data found for the given Id";

        public const string MenuCreateSuccess = "Menu created";
        public const string MenuCreateError = "Menu creation failed";

        public const string MenuUpdateSuccess = "Menu updated";
        public const string MenuUpdateError = "Menu update failed. Ensure that the given Id is valid.";

        public const string MenuDeleteSuccess = "Menu deleted";
        public const string MenuDeleteError = "Menu delete failed. Ensure that the given Id is valid.";

        public const string MenuIdNotExist = "Menu Id does not exist";
        public const string MenuIdNotExistInHistory = "Menu Id does not exist in history table. No history found.";
        public const string MenuScreenCodeNotExist = "Menu ScreenCode does not exist";
        public const string MenuScreenCodeNotExistInHistory = "Menu ScreenCode does not exist in history table. No history found.";
        public const string MenuParentIdNotExist = "Menu ParentId does not exist";

        public const string MenuRollbackSuccess = "Menu rollbacked";
        public const string MenuRollbackError = "Menu rollback failed";

        public const string MenuCacheNotExist = "Menu cache already not exist";
        public const string MenuCacheRemoved = "Menu Cache removed";
    }
}
