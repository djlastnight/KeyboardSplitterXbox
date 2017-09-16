namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using KeyboardSplitter.Models;

    public static class GameDataManager
    {
        private const string GameDataFilename = "games.xml";

        private static GameData data;

        static GameDataManager()
        {
            data = ReadGameDataFromFile();
        }

        public static ObservableCollection<Game> Games
        {
            get
            {
                return data.Games;
            }
        }

        public static GameData ReadGameDataFromFile()
        {
            var data = new GameData();
            try
            {
                data = GameData.Deserialize(GameDataFilename);
            }
            catch (Exception e)
            {
                data.Games = new ObservableCollection<Game>();
                data.Games.Add(new Game("Error occured, while trying to get data.", GameDataFilename, e.Message, new List<SlotData>()));
            }

            return data;
        }

        public static void WriteGameDataToFile()
        {
            GameDataManager.data.Serialize(GameDataFilename);
        }
    }
}
