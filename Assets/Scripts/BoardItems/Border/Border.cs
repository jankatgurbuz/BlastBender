namespace BoardItems.Border
{
    [System.Serializable]
    public class Border 
    {
        public int Row;
        public int Column;
        public bool HasNeighbors;
        public bool IsEmpty;

        public string CornerKey;
        public string LeftRightKey;
        public string TopBottomKey;

        public Border(bool hasNeighbors, bool isEmpty, int row, int column)
        {
            HasNeighbors = hasNeighbors;
            IsEmpty = isEmpty;
            Row = row;
            Column = column;
        }
    }
}
