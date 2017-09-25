namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using KeyboardSplitter.Models;

    public static class GameDataManager
    {
        private const string GameDataFilename = "splitter_games.xml";

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
            if (!System.IO.File.Exists(GameDataFilename))
            {
                return data;
            }

            try
            {
                data = GameData.Deserialize(GameDataFilename);
            }
            catch (Exception e)
            {
                data.Games = new ObservableCollection<Game>();
                data.Games.Add(new Game("Error occured, while trying to get data.", GameDataFilename, e.Message, new ObservableCollection<SlotData>()));
            }

            return data;
        }

        public static void WriteGameDataToFile()
        {
            GameDataManager.data.Serialize(GameDataFilename);
        }
    }
}
