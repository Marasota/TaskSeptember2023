using System;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            //напиши реализацию не меняя сигнатуру функции
            if (levelIndex > 0)
            {
                var info = new LevelInfo();
                info.words = ReadFromJson(levelIndex);
                return info;
            }
            throw new NotImplementedException();
        }

        public List<string> ReadFromJson(int levelIndex)
        {
            string json = File.ReadAllText(Application.dataPath + $"/App/Resources/WordSearch/Levels/{levelIndex}.json");
            return JsonUtility.FromJson<LevelInfo>(json).words;
        }
    }
}