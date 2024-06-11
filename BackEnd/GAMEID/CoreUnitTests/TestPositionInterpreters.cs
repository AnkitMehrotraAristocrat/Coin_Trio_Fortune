using System.Collections.Generic;
using GameBackend.Helpers;

namespace GameBackend.UnitTests
{
    public static class TestPositionInterpreters
    {
        // Note that these tests require a valid WindowMaxWidth and WindowMaxHeight in GameConstants
        // Those can be anything greater or equal to the height and width passed through to the test functions as arguments 
        //
        // In these tests we use a world window of 5 Columns and 6 Rows, you must update constants to test properly
        // ClientPositionFormationValue in Configuratuon/Preferences.json also matters, for determining which tests are performed 
        //

        public static void RunAllTests()
        {
            #if DEBUG
            RunTest_GetWorldIndexByPosition();
            RunTest_GetPositionByIndex();
            RunTest_GetClientPositionByWorldIndex();
            RunTest_GetClientIndexByWorldIndex();
            RunTest_GetWorldIndexByClientPosition();
            RunTest_GetWorldIndexByClientIndex();
            RunTest_GetClientIndexListByWorldIndexList();
            RunTest_GetWorldIndexedListInClientFormation();
            #endif
        }

        private static void RunTest_GetWorldIndexByPosition()
        {
            var func = GeneralHelper.GetWorldIndexByPosition; //col, row
            DebugHelper.AssertAreEqual(func.Method.Name, func(2, 4), 22);
            DebugHelper.AssertAreEqual(func.Method.Name, func(0, 5), 25);
        }

        private static void RunTest_GetPositionByIndex()
        {
            var func = GeneralHelper.GetPositionByIndex; //indexLRTB, width
            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(22, 5).X, 2);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(22, 5).Y, 4);

            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(25, 5).X, 0);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(25, 5).Y, 5);

            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(29, 5).X, 4);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(29, 5).Y, 5);

            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(0, 5).X, 0);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(0, 5).Y, 0);
        }

        private static void RunTest_GetClientPositionByWorldIndex()
        {
            var func = GeneralHelper.GetClientPositionByWorldIndex; //worldIndex, clientHeight
            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(22, 4).X, 2);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(22, 4).Y, 2);

            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(22, 5).X, 2);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(22, 5).Y, 3);

            DebugHelper.AssertAreEqual($"{func.Method.Name}.X", func(22, 6).X, 2);
            DebugHelper.AssertAreEqual($"{func.Method.Name}.Y", func(22, 6).Y, 4);
        }

        private static void RunTest_GetClientIndexByWorldIndex()
        {
            var pref = GeneralHelper.GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            var func = GeneralHelper.GetClientIndexByWorldIndex; //worldIndex, clientHeight, clientWidth
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 4, 5), 12);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 5, 5), 17);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 4, 4), 10); 
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 5, 3), 11); 
            }
            else {
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 4, 5), 10);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 5, 5), 13);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 4, 4), 10);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 5, 3), 13);
            }
        }

        private static void RunTest_GetWorldIndexByClientPosition()
        {
            var func = GeneralHelper.GetWorldIndexByClientPosition; //col, row, clientHeight
            DebugHelper.AssertAreEqual(func.Method.Name, func(2, 2, 4), 22);
            DebugHelper.AssertAreEqual(func.Method.Name, func(2, 3, 5), 22);

            DebugHelper.AssertAreEqual(func.Method.Name, func(0, 3, 4), 25);
            DebugHelper.AssertAreEqual(func.Method.Name, func(0, 4, 5), 25);
        }

        private static void RunTest_GetWorldIndexByClientIndex()
        {
            var pref = GeneralHelper.GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            var func = GeneralHelper.GetWorldIndexByClientIndex; //clientIndex, clientHeight, clientWidth
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                DebugHelper.AssertAreEqual(func.Method.Name, func(12, 4, 5), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(17, 5, 5), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(10, 4, 4), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(11, 5, 3), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(22, 6, 5), 22);

                DebugHelper.AssertAreEqual(func.Method.Name, func(14, 5, 3), 27);
                DebugHelper.AssertAreEqual(func.Method.Name, func(2, 4, 4), 12);
                DebugHelper.AssertAreEqual(func.Method.Name, func(3, 5, 3), 10);
                DebugHelper.AssertAreEqual(func.Method.Name, func(0, 5, 3), 5);
                DebugHelper.AssertAreEqual(func.Method.Name, func(0, 6, 5), 0);
            }
            else {
                DebugHelper.AssertAreEqual(func.Method.Name, func(10, 4, 5), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(13, 5, 5), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(10, 4, 4), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(13, 5, 3), 22);
                DebugHelper.AssertAreEqual(func.Method.Name, func(16, 6, 5), 22);

                DebugHelper.AssertAreEqual(func.Method.Name, func(14, 5, 3), 27);
                DebugHelper.AssertAreEqual(func.Method.Name, func(8, 4, 4), 12);
                DebugHelper.AssertAreEqual(func.Method.Name, func(1, 5, 3), 10);
                DebugHelper.AssertAreEqual(func.Method.Name, func(0, 5, 3), 5);
                DebugHelper.AssertAreEqual(func.Method.Name, func(0, 6, 5), 0);
            }
        }

        private static void RunTest_GetClientIndexListByWorldIndexList()
        {
            var pref = GeneralHelper.GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            var func = GeneralHelper.GetClientIndexListByWorldIndexList; //list, clientHeight, clientWidth

            // The contents represent the position indexes 
            var listOfWorldIndexes = new List<int> {
                10, 11, 12
            };

            var results5Wx4H = func(listOfWorldIndexes, 4, 5);
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                var expected = new List<int> {
                    0, 1, 2
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results5Wx4H, expected);
            }
            else {
                var expected = new List<int> {
                    0, 4, 8
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results5Wx4H, expected);
            }

            var results3Wx5H = func(listOfWorldIndexes, 5, 3);
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                var expected = new List<int> {
                    3, 4, 5
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results3Wx5H, expected);
            }
            else {
                var expected = new List<int> {
                    1, 6, 11
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results3Wx5H, expected);
            }
        }

        private static void RunTest_GetWorldIndexedListInClientFormation()
        {
            var pref = GeneralHelper.GetPreferenceString(GameConstants.ClientPositionFormationPreferenceKey);
            var func = GeneralHelper.GetWorldIndexedListInClientFormation<int>; //list, clientHeight, clientWidth

            // The indexes themselves represent the positions
            // the contents are for testing purposes, to allow an understading of where each worldIndex's contents will end up on a client window
            var worldIndexedList = new List<int> { 
                0, 1, 2, 3, 4, 
                5, 6, 7, 8, 9, 
                10, 11, 12, 13, 14, 
                15, 16, 17, 18, 19, 
                20, 21, 22, 23, 24, 
                25, 26, 27, 28, 29 
            };
            
            var results5Wx4H = func(worldIndexedList, 4, 5);
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                var expected = new List<int> { 
                    10, 11, 12, 13, 14, 
                    15, 16, 17, 18, 19, 
                    20, 21, 22, 23, 24, 
                    25, 26, 27, 28, 29 
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results5Wx4H, expected);
            }
            else {
                var expected = new List<int> { 
                    10, 15, 20, 25, 11, 
                    16, 21, 26, 12, 17, 
                    22, 27, 13, 18, 23, 
                    28, 14, 19, 24, 29 
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results5Wx4H, expected);
            }

            var results3Wx5H = func(worldIndexedList, 5, 3);
            if (pref != GameConstants.ClientPositionFormationTBLR) {
                var expected = new List<int> { 
                    5, 6, 7, 
                    10, 11, 12, 
                    15, 16, 17,
                    20, 21, 22,
                    25, 26, 27
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results3Wx5H, expected);
            }
            else {
                var expected = new List<int> { 
                    5, 10, 15, 
                    20, 25, 6, 
                    11, 16, 21, 
                    26, 7, 12, 
                    17, 22, 27 
                };
                DebugHelper.AssertAreAllEqual(func.Method.Name, results3Wx5H, expected);
            }
        }
    }
}
