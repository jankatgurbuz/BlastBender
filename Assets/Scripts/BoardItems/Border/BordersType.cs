namespace BoardItems.Border
{
    public enum BordersType
    {
        Corner_Center = 1111, //,
        Corner_LeftBottom = 1000,
        Corner_Bottom = 1100,
        Corner_InnerLeftTop = 1110,
        Corner_InnerRightTop = 1101,
        Corner_InnerLeftBottom = 1011,
        Corner_Left = 1001,
        Corner_ConcaveRightBottomLeftTop = 1010,
        Corner_LeftTop = 1, //0001,
        Corner_InnerRightBottom = 111, //0111,
        Corner_Top = 11, //0011,
        Corner_RightTop = 10, //0010,
        Corner_Right = 110, //0110,
        Corner_ConcaveRightTopLeftBottom = 101, //0101,
        Corner_RightBottom = 100, //0100

        Horizontal_Left = 102, // 10-2
        Horizontal_Center = 112, //  00-2 
        Horizontal_Right = 12, //  01-2

        Vertical_Bottom = 103, // 10-3
        Vertical_Center = 113, // 00-3
        Vertical_Top = 13 //01-3
    }
}