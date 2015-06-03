using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using QuestServer.Storage.QuestDbDataSetTableAdapters;

namespace QuestServer.Storage
{
    public class Provider
    {
        private readonly QuestDbDataSet _dataSet = new QuestDbDataSet();
        private readonly KeysTableAdapter _keysTableAdapter = new KeysTableAdapter();
        private readonly QuestsTableAdapter _questsTableAdapter = new QuestsTableAdapter();

        public Provider()
        {
            _questsTableAdapter.Fill(_dataSet.Quests);
            _keysTableAdapter.Fill(_dataSet.Keys);
        }

        public IEnumerable<Key> GetQuestKeys(int questId)
        {
            var keys = _dataSet.Quests.FindById(questId)
                .GetKeysRows()
                .Select(x => new Key
                {
                    QuestId = x.QuestId,
                    Description = x.Description,
                    Image = x.Image,
                    TimeOffset = x.TimeOffset
                }).ToArray();
            
            return keys;
        }
    }
}
