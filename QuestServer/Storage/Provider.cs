using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using QuestServer.Models;
using QuestServer.Storage.QuestDbDataSetTableAdapters;

namespace QuestServer.Storage
{
    public class Provider
    {
        private readonly QuestDbDataSet _dataSet = new QuestDbDataSet();
        private readonly KeysTableAdapter _keysTableAdapter = new KeysTableAdapter();
        private readonly QuestsTableAdapter _questsTableAdapter = new QuestsTableAdapter();

        public int UpdateQuests()
        {
            _questsTableAdapter.Adapter.AcceptChangesDuringUpdate = true;
            return _questsTableAdapter.Update(_dataSet.Quests);
        }

        public int UpdateKeys()
        {
            _keysTableAdapter.Adapter.AcceptChangesDuringUpdate = true;
            return _keysTableAdapter.Update(_dataSet.Keys);
        }

        public QuestDbDataSet GetDataSet()
        {
            return _dataSet;
        }

        public Provider()
        {
            _questsTableAdapter.Fill(_dataSet.Quests);
            _keysTableAdapter.Fill(_dataSet.Keys);
        }

        public Quest GetQuest(int questId)
        {
            var qRow = _dataSet.Quests.FindById(questId);
            var keys = qRow.GetKeysRows().AsEnumerable().Select(x => new Key
            {
                Description = x.Description,
                QuestId = qRow.Id,
                TimeOffset = x.TimeOffset,
                Image = x.IsImageNull() ? null : x.Image,
                SensorId = x.SensorId
            }).ToArray();

            var quest = new Quest
            {
                Id = qRow.Id,
                Description = qRow.Desccription,
                Keys = keys,
                Sensors = keys.Select(x => new Sensor
                {
                    QuestId = qRow.Id,
                    Id = x.SensorId
                }).ToArray()
            };

            return quest;

        }

        //public Key[] GetQuestKeys(int questId)
        //{
        //    var keys = _dataSet.Quests.FindById(questId)
        //        .GetKeysRows()
        //        .Select(x => new Key
        //        {
        //            QuestId = x.QuestId,
        //            Description = x.Description,
        //            Image = x.Image,
        //            TimeOffset = x.TimeOffset
        //        }).ToArray();
            
        //    return keys;
        //}

        //public IEnumerable<int> GetSensorsIds(int questId)
        //{
        //    var ids = _dataSet.Quests.FindById(questId)
        //        .GetKeysRows().Select(x => x.SensorId);
        //    return ids;
        //}

        //public IEnumerable<Quest> GetAllQuests()
        //{
        //    var quests = _dataSet.Quests.AsEnumerable()
        //        .Select(x => new Quest
        //                            {
        //                                Id = x.Id,
        //                                Description = x.Desccription
        //                            });
        //    return quests;
        //}

        //internal object GetQuest(int questId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
