namespace UsefulTools.Editor.RefHelper
{
    public interface IRefHelperSaveLoad
    {
        void SaveData(RefHelperData data);
        RefHelperData LoadData();
    }
}