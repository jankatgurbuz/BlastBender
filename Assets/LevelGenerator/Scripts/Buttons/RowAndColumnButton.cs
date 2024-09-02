using LevelGenerator.Scripts.Controller;
using TMPro;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Scripts.Buttons
{
    public class RowAndColumnButton : MonoBehaviour
    {
        [SerializeField] private TextMeshPro tmp;
        [Inject] private readonly ILevelGeneratorController _levelGeneratorController;

        public void IncreaseRow()
        {
            _levelGeneratorController.RowLength = ClampAndWriteOnTextMeshPro(_levelGeneratorController.RowLength, 1);
        }

        public void DecreaseRow()
        {
            _levelGeneratorController.RowLength = ClampAndWriteOnTextMeshPro(_levelGeneratorController.RowLength, -1);
        }

        public void IncreaseColumn()
        {
            _levelGeneratorController.ColumnLength = ClampAndWriteOnTextMeshPro(_levelGeneratorController.ColumnLength, 1);
        }

        public void DecreaseColumn()
        {
            _levelGeneratorController.ColumnLength = ClampAndWriteOnTextMeshPro(_levelGeneratorController.ColumnLength, -1);
        }
        private int ClampAndWriteOnTextMeshPro(int number, int positiveOrNegativeNumber)
        {
            number += positiveOrNegativeNumber;
            number = Mathf.Clamp(number, ILevelGeneratorController.MinLength, ILevelGeneratorController.MaxLength);
            tmp.text = number.ToString();
            return number;
        }
    }
}
