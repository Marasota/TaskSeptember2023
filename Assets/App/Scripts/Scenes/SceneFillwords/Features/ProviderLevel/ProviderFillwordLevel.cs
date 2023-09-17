using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;
using System.IO;
using System.Linq;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        public GridFillWords LoadModel(int index)
        {
            //напиши реализацию не меняя сигнатуру функции
            if (index > 0)
            {
                Dictionary<string, string[]> WordPosition = new Dictionary<string, string[]>();
                string word;
                string level = ReadLevel(index) + ' ';
                int root;

                while(level.Length > 0)
                {
                    word = ReadWord(Convert.ToInt32(level.Substring(0, level.IndexOf(' '))));
                    level = level.Substring(level.IndexOf(' ') + 1);
                    WordPosition.Add(word, level.Substring(0, level.IndexOf(' ')).Split(';'));
                    level = level.Substring(level.IndexOf(' ') + 1);
                }

                root = GetRoot(WordPosition);
                GridFillWords model = new GridFillWords(new Vector2Int(root,root));
                if (isCorrectLevel(WordPosition))
                {
                    fillGridFillWords(model, WordPosition, root);
                    return model;
                }
                else
                {
                    return null;
                } 
            }
            throw new NotImplementedException();
        }
        public int GetRoot(Dictionary<string, string[]> WordPosition)
        {
            string letters = "";
            foreach(var key in WordPosition.Keys)
            {
                letters += key;
            }
            return (int)Math.Sqrt(letters.Length);
        }
        public string ReadWord(int index)
        {
            return File.ReadLines(Application.dataPath + $"/App/Resources/Fillwords/words_list.txt").Skip(index).First();
           
        }
        public string ReadLevel(int index)
        {
           return File.ReadLines((Application.dataPath + $"/App/Resources/Fillwords/pack_0.txt")).Skip(index).First();
        }
        bool isCorrectLevel(Dictionary<string, string[]> WordPosition)
        {
            foreach(var key in WordPosition.Keys)
            {
                if (key.Length != WordPosition[key].Count())
                    return false;
            }
            return true;
        }
        public void fillGridFillWords(GridFillWords model, Dictionary<string, string[]> WordPosition, int root)
        {
            string[] value;
            foreach(var key in WordPosition.Keys)
            {
                value = WordPosition[key];
                for (int i = 0; i < key.Length; i++)
                {
                    model.Set(Convert.ToInt32(value[i]) / (root), Convert.ToInt32(value[i]) % (root), new CharGridModel(key[i]));
                }
            }
        }

    }
}

