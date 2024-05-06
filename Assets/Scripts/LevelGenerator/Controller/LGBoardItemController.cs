using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoardItems;
using BoardItems.LevelData;
using BoardItems.Void;
using UnityEngine;

namespace LevelGenerator.Controller
{
    public class LGBoardItemController
    {
        public void CreateBoardItems(ref IBoardItem[,] boardItem, int rowLength, int columnLength)
        {
            var tempBoardItem = new IBoardItem[rowLength, columnLength];

            for (int i = 0; i < rowLength; i++)
            {
                if (i >= boardItem.GetLength(0))
                    continue;

                for (int j = 0; j < columnLength; j++)
                {
                    if (j >= boardItem.GetLength(1))
                        continue;

                    tempBoardItem[i, j] = boardItem[i, j];
                }
            }

            boardItem = tempBoardItem;

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    if (boardItem[i, j] == null)
                    {
                        boardItem[i, j] = CreateInstance<VoidArea>(i, j);
                    }
                }
            }
        }

        public void AssignBoardItem(LevelData levelData, IBoardItem[,] boardItem)
        {
            levelData.BoardItem = boardItem.Cast<IBoardItem>().ToArray();
            levelData.RowLength = boardItem.GetLength(0);
            levelData.ColumnLength = boardItem.GetLength(1);
        }

        //helpers

        public object CreateInstance(Type type, params object[] constructorArgs)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length != 0)
            {
                ParameterInfo[] paramsInfo = constructors[0].GetParameters();
                if (paramsInfo.Length < constructorArgs.Length)
                {
                    var newConst = new object[paramsInfo.Length];
                    for (int i = 0; i < newConst.Length; i++)
                        newConst[i] = constructorArgs[i];
                    constructorArgs = newConst;
                }
            }

            var instance = Activator.CreateInstance(type, constructorArgs);

            //// reflection
            //var flag = System.Reflection.BindingFlags.NonPublic |
            //           System.Reflection.BindingFlags.Instance;

            //var fieldInfo = instance.GetType().GetField("_color", flag);
            //fieldInfo?.SetValue(instance, color);

            return instance;
        }

        private T CreateInstance<T>(params object[] constructorArgs) where T : IBoardItem
        {
            return (T)CreateInstance(typeof(T), constructorArgs);
        }
    }
}