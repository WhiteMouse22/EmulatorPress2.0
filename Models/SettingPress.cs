using LiteDB;

namespace EmulatorPress.Models
{
    public class SettingPress
    {
        public SignalType Type { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }

        public void SaveSettings()
        {
            using var db = new LiteDatabase(@"SettingDB.db");
            var col = db.GetCollection<SettingPress>("settingpress");
            var setting = new SettingPress
            {
                Type = Type,
                MinValue = MinValue,
                MaxValue = MaxValue
            };
            col.DeleteAll();
            col.Insert(setting);
        }
        public void LoadSettings()
        {
            using var db = new LiteDatabase(@"SettingDB.db");
            var col = db.GetCollection<SettingPress>("settingpress");
            var result = col.FindAll();
            foreach (var item in result)
            {
                Type = item.Type;
                MinValue = item.MinValue;
                MaxValue = item.MaxValue;
            }
        }
    }
    public enum SignalType
    {
        Constant,
        Randoms,
        Step
    }
}
