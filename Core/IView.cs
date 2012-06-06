namespace GameOfLife.Core
{
    public interface IView
    {
        void InvokeRefresh();

        void ShowCellStatus(string msg);

        void SetGameStatus(bool start);
    }
}
