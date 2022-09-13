using System;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperDataWrapper : IDisposable
    {
        private IRefHelperSaveLoad _saveLoad;

        private IRefHelperSaveLoad SaveLoad => _saveLoad ??= RefHelperSaveLoadScriptable.Instance;

        public RefHelperData Data { get; }

        public RefHelperDataWrapper() => Data = SaveLoad.LoadData();

        public void Dispose() => SaveLoad.SaveData(Data);
    }
}