using GameBackend.Data;
using Milan.Common.Implementations.Exceptions;
using Milan.Common.SlotEngine.Models;
using Milan.XSlotEngine.Core.Helpers;
using Milan.XSlotEngine.Core.Models.WeightTables;
using Milan.XSlotEngine.Core.Utility.Constants;
using Milan.XSlotEngine.Interfaces.Core.WeightTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameBackend.Helpers
{
    public static class GeneralHelper
    {
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// State string helpers 
        
        public static string GetGameStateString(GameStates state)
        {
            return Enum.GetName(typeof(GameStates), state);
        }

        public static GameStates GetGameStateEnum(string state)
        {
            if (Enum.TryParse(state, true, out GameStates stateEnum))
                return stateEnum;
            throw new Exception(string.Concat(state, Error.DoesNotExist));
        }

        public static string[] GetGameStatesArray()
        {
            return Enum.GetNames(typeof(GameStates));
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Validation Helpers

        public static void StepExceptionOnNull(object caller, object obj, string objName)
        {
            if (obj == null) {
                throw new ExecutionStepException(
                    caller.GetType().FullName,
                    string.Concat(objName, Error.NotNull));
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Config helpers

        public static bool ConfigExist(string file)
        {
            string loc = Path.Combine("plugins", "backend", GameConstants.GameId, "Configuration", file);
            return File.Exists(loc);
        }

        public static string? GetPreferenceString(string name, string defaultValue = null)
        {
            if (Configurations.Preferences.ContainsKey(name)) {
                return Configurations.Preferences[name];
            }
            return defaultValue;
        }

        public static bool GetPreferenceBool(string name, bool defaultValue = false)
        {
            if (Configurations.Preferences.ContainsKey(name)) {
                if (bool.TryParse(Configurations.Preferences[name], out bool value)) {
                    return value;
                }
            }
            return defaultValue;
        }

        public static string[] GetPreferenceStringArray(string name, string[] defaultValue = null)
        {
            if (Configurations.Preferences.ContainsKey(name)) {
                return Configurations.Preferences[name].Split(',');
            }
            return defaultValue;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Random value helpers 

        public static ulong GetRandomValue(GameContext context, ulong max, string label)
        {
            if (max <= 1) {
                return 0;
            }
            var randomNumberQueue = context.PersistentData?.RandomNumberQueue ?? new Queue<ulong?>();
            ulong value = RngHelper.GetRandomValue(max, randomNumberQueue, context.SpinGaffeInfo);
            context.SpinGaffeInfo.Last().TableName = label;
            return ((value % max) + max) % max;
        }

        public static int GetRandomValue(GameContext context, int max, string label)
        {
            return (int)GetRandomValue(context, (ulong)max, label);
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Weight table helpers

        public static T GetRandomEntryFromTable<T>(GameContext context, string tableName)
        {
            WeightTableDefinition weighTableDefinition = new(context.MappedConfigurations.WeightTableDefinitions);
            T entry = weighTableDefinition.GetRandomEntry<T>(tableName, context.PersistentData.RandomNumberQueue, context.SpinGaffeInfo);
            return entry;
        }

        public static T GetRandomEntryFromList<T>(GameContext context, List<IEntry> entries, string label)
        {
            int index = context.SpinGaffeInfo.Count;
            T entry = WeightTable.GetRandomEntry<T>(entries, context.PersistentData.RandomNumberQueue, context.SpinGaffeInfo);
            context.SpinGaffeInfo[index].TableName = label;
            return entry;
        }

        public static void ConsumeQueuedNumber(Queue<ulong?> queue)
        {
            if (queue.Count > 0) {
                queue.Dequeue();
            }
        }

        public static IList<KeyValuePair<long, T>> GetEntriesForTable<T>(GameContext context, string tableName)
        {
            WeightTableDefinition weighTableDefinition = new(context.MappedConfigurations.WeightTableDefinitions);
            return weighTableDefinition.GetEntriesForTable<T>(tableName);
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Position interpretation helpers

        public static int GetWorldIndexByPosition(int col, int row)
        {
            return row * GameConstants.WindowMaxWidth + col;
        }

        public static PositionData GetPositionByIndex(int indexLRTB, int width = GameConstants.WindowMaxWidth)
        {
            int row = indexLRTB / (width == 0 ? 1 : width);
            int col = indexLRTB % (width == 0 ? 1 : width);
            return new PositionData(col, row);
        }

        public static PositionData GetClientPositionByWorldIndex(int worldIndex, int clientHeight)
        {
            int heightDiff = GameConstants.WindowMaxHeight - clientHeight;
            return GetPositionByIndex(worldIndex - heightDiff * GameConstants.WindowMaxWidth);
        }

        public static int GetClientIndexByWorldIndex(int worldIndex, int clientHeight, int clientWidth)
        {
            string formation = GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            int heightDiff = GameConstants.WindowMaxHeight - clientHeight;
            PositionData data = GetPositionByIndex(worldIndex - heightDiff * GameConstants.WindowMaxWidth);
            if (formation != GameConstants.ClientPositionFormationTBLR) {
                return data.Y * clientWidth + data.X;
            }
            return data.X * clientHeight + data.Y;
        }

        public static int GetWorldIndexByClientPosition(int col, int row, int clientHeight)
        {
            var heightDiff = GameConstants.WindowMaxHeight - clientHeight;
            var worldRow = row + heightDiff;
            return GetWorldIndexByPosition(col, worldRow);
        }

        public static int GetWorldIndexByClientIndex(int clientIndex, int clientHeight, int clientWidth)
        {
            string formation = GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            var heightDiff = GameConstants.WindowMaxHeight - clientHeight;
            if (formation != GameConstants.ClientPositionFormationTBLR) {
                var clientPos = GetPositionByIndex(clientIndex, clientWidth);
                return GetWorldIndexByPosition(clientPos.X, clientPos.Y + heightDiff);
            }
            int col = clientIndex / (clientHeight == 0 ? 1 : clientHeight);
            int row = clientIndex - (col * clientHeight);
            return GetWorldIndexByPosition(col, row + heightDiff);
        }

        public static List<int> GetClientIndexListByWorldIndexList(List<int> list, int clientHeight, int clientWidth)
        {
            // The contents of the list are the position indexes
            var clientList = new List<int>();

            int index = 0;
            while (index < list.Count) {
                int clientIndex = GetClientIndexByWorldIndex(list[index], clientHeight, clientWidth);
                clientList.Add(clientIndex);
                index++;
            }
            return clientList;
        }

        public static List<T> GetWorldIndexedListInClientFormation<T>(List<T> list, int clientHeight, int clientWidth)
        {
            // The indexes of the list are the position indexes
            var length = clientWidth * clientHeight;
            var mapped = new List<T>(new T[length]);

            int index = 0;
            while (index < list.Count) {
                int clientIndex = GetClientIndexByWorldIndex(index, clientHeight, clientWidth);
                if (clientIndex >= 0 && clientIndex < mapped.Count) {
                    mapped[clientIndex] = list.ElementAt(index);
                }
                index++;
            }
            return mapped;
        }
    }
}
