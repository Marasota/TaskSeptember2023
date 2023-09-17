using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            //напиши реализацию не меняя сигнатуру функции

            List<char> inputChars = new List<char>();

            if (words.Count > 0)
            {
                HashSet<char> uniqueLetters = new HashSet<char>(String.Concat(words));
                int maxCount = 0;
                int currentCount = 0;
                foreach (var letter in uniqueLetters)
                { 
                    foreach (var word in words)
                    {
                      currentCount = word.Count(c => c == letter);
                      if (currentCount > maxCount) maxCount = currentCount;
                    }
                    while(maxCount-->0) inputChars.Add(letter);

                }
               return inputChars;
            }
            throw new NotImplementedException();
        }
    }
}