using System;
using System.Threading;

namespace GameOfLife.Core
{
    internal sealed class GameThread
    {
        private readonly IGameThread _game;

        private readonly bool[,] _life;
        private readonly bool[,] _next;
        private readonly bool[,] _remain;

        private int _generation;
        private int _population;
        private int _newborncell;

        private bool _startflag;

        public GameThread(IGameThread game)
        {
            _game = game;

            _life = new bool[_game.Cells, _game.Cells];
            _next = new bool[_game.Cells, _game.Cells];
            _remain = new bool[_game.Cells, _game.Cells];

            _generation = 0;
            _population = 0;
            _newborncell = 0;

            _startflag = false;
        }

        public int Generation { get { return _generation; } }
        public int Population { get { return _population; } }
        public int NewBornCell { get { return _newborncell; } }

        public bool StartFlag
        {
            get { return _startflag; }
            set { _startflag = value; }
        }

        public bool[,] Life { get { return _life; } }
        public bool[,] Remain { get { return _remain; } }


        public void AddDelLife(int i, int j)
        {
            if (i < 0 || i >= _game.Cells) return;
            if (j < 0 || j >= _game.Cells) return;

            if (_life[i, j]) _life[i, j] = false;
            else _life[i, j] = true;
            _game.View.InvokeRefresh();
        }

        public void ShowCellStatus(int i, int j)
        {
            if (i < 0 || i >= _game.Cells) return;
            if (j < 0 || j >= _game.Cells) return;

            string cellstat = (i + 1) + ", " + (i + 1) + ", " + _life[i, j];
            _game.View.ShowCellStatus(cellstat);
        }

        public void StartStopGame()
        {
            _startflag = !_startflag;
            _game.View.SetGameStatus(_startflag);
        }

        public void ClearCell()
        {
            if (_startflag)
            {
                _startflag = false;
                _game.View.SetGameStatus(_startflag);
            }

            for (int i = 0; i < _game.Cells; i++)
            {
                for (int j = 0; j < _game.Cells; j++)
                {
                    _life[i, j] = false;
                }
            }
            _generation = 0;
            _population = 0;
            _newborncell = 0;

            _game.View.InvokeRefresh();
        }

        public void RandomStart()
        {
            ClearCell();

            Random rnd = new Random();
            for (int i = 0; i < _game.Cells; i++)
            {
                for (int j = 0; j < _game.Cells; j++)
                {
                    if ((rnd.Next() * 100) % 3 == 0) _life[i, j] = true;
                    else _life[i, j] = false;
                }
            }
            _game.View.InvokeRefresh();
        }

        public void GliderStart()
        {
            ClearCell();
            _life[0, 1] = true;
            _life[1, 2] = true;
            for (int col = 0; col < 3; col++)
            {
                _life[2, col] = true;
            }
            _game.View.InvokeRefresh();
        }

        public void Run()
        {
            Random rnd = new Random();
            int cells = _game.Cells;
            while (true)
            {
                try
                {
                    if (_startflag)
                    {
                        _population = 0;
                        for (int i = 0; i < cells; i++)
                        {
                            for (int j = 0; j < cells; j++)
                            {
                                int a = 0;
                                if (_life[(i + cells - 1) % cells, j]) a++; //left
                                if (_life[(i + 1) % cells, j]) a++; //right
                                if (_life[i, (j + cells - 1) % cells]) a++; //up
                                if (_life[i, (j + 1) % cells]) a++; //down
                                if (_life[(i + cells - 1) % cells, (j + cells - 1) % cells]) a++; //upper left
                                if (_life[(i + 1) % cells, (j + cells - 1) % cells]) a++; //upper right
                                if (_life[(i + cells - 1) % cells, (j + 1) % cells]) a++; //downer left
                                if (_life[(i + 1) % cells, (j + 1) % cells]) a++; //downer right
                                if (_life[i, j])
                                {
                                    if (a == 2 || a == 3)
                                    {
                                        _next[i, j] = true;
                                        _population++;
                                    }
                                    else _next[i, j] = false;
                                }
                                else
                                {
                                    if (a == 3)
                                    {
                                        _next[i, j] = true;
                                        _population++;
                                    }
                                    else
                                    {
                                        if ((_game.Noise) && ((rnd.Next() * 10000) % 1000 == 0)) _next[i, j] = true;
                                        else _next[i, j] = false;
                                    }
                                }
                            }
                        }
                        JudgeRemain(cells);
                        NextGeneration(cells);
                        _generation++;
                        _game.View.InvokeRefresh();
                    }
                    Thread.Sleep(50);
                }
                catch { }
            }
        }

        private void JudgeRemain(int cells)
        {
            _newborncell = 0;

            for (int i = 0; i < cells; i++)
            {
                for (int j = 0; j < cells; j++)
                {
                    if (_life[i, j] == _next[i, j])
                    {
                        _remain[i, j] = true;
                    }
                    else
                    {
                        _remain[i, j] = false;
                        if (_next[i, j]) _newborncell++;
                    }
                }
            }
        }

        private void NextGeneration(int cells)
        {
            for (int i = 0; i < cells; i++)
            {
                for (int j = 0; j < cells; j++)
                {
                    _life[i, j] = _next[i, j];
                }
            }
        }
    }
}
