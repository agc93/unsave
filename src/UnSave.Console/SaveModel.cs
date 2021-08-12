using UnSave.Extensions;
using UnSave.Types;

namespace UnSave
{
    public partial class SaveModel
    {
        protected GvasSaveData SaveData {get;}

        /*[SaveProperty("CampaignCredits", SaveData = nameof(SaveData))]
        public UEIntProperty _ccredits;*/
        
        [ViewProperty(ViewPropertyName = "Credits")]
        internal UEIntProperty CampaignCredits => SaveData.FindProperty<UEIntProperty>(p => p.Name == "CampaignCredits");

        public SaveModel() {
            // CampaignCredits?.Value = 0;
        }
    }
    
    [SaveProperty("CampaignCredits", typeof(UEIntProperty))]
    [SaveProperty("HasACampaign", typeof(UEBoolProperty), PropertyName = "CampaignActive")]
    public partial class SaveDataModel
    {
        public SaveDataModel(GvasSaveData saveData) {
            SaveData = saveData;
        }

        private GvasSaveData SaveData { get; }
    }
}