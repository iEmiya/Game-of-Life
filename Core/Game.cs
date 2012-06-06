using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

namespace GameOfLife.Core
{
    public interface IGame
    {
        void Destroy();
        void AddDelLife(int px, int py);
        Image PaintImage();
        void ShowCellStatus(int px, int py);
        void StartStopGame();
        void ClearCell();
        void RandomStart();
        void GliderStart();
        void SelectOrDeselectNoise(bool noise);

    }

    internal interface IGameDraw
    {
        Size Size { get; }
        Graphics BufGraphics { get; }

        int Cells { get; }
        int Generation { get; }
        int Population { get; }
        int NewBornCell { get; }
        bool Noise { get; }
    }

    internal interface IGameThread
    {
        IView View { get; }
        int Cells { get; }

        bool Noise { get; }
    }

    public sealed class Game : IGame, IGameDraw, IGameThread
    {
        private const int m_widthInfo = 150;
        internal const int m_size = 16;
        internal const int m_radius = 4;

        public static IGame Create(IView view, Size size)
        {
            int cells = GetCountCells(size);
            Game game = new Game(view, cells);
            game.Init(size);
            return game;
        }

        private static int GetCountCells(Size size)
        {
            int w = size.Width - m_widthInfo;
            int h = size.Height;
            int min = (w > h) ? h : w;
            return min / m_size;
        }

        private readonly IView _view;
        private readonly int _cells;
        private readonly GameInfo _info;
        private readonly GameThread _thread;

        private Thread _th = null;
        private Bitmap _bufImage;
        private Graphics _bufGraphics;
        private Size _size;

        private bool _noise = false;

        private Game(IView view, int cells)
        {
            _view = view;

            _cells = cells;

            _info = new GameInfo(this);
            _thread = new GameThread(this);
        }

        public IView View { get { return _view; } }

        public Size Size { get { return _size; } }
        public Graphics BufGraphics { get { return _bufGraphics; } }

        public int Cells { get { return _cells; } }
        public int Generation { get { return _thread.Generation; } }
        public int Population { get { return _thread.Population; } }
        public int NewBornCell { get { return _thread.NewBornCell; } }
        public bool Noise { get { return _noise; } }

        public void Destroy()
        {
            Stop();

            if (_bufGraphics != null)
            {
                _bufGraphics.Dispose();
                _bufGraphics = null;
            }
            if (_bufImage != null)
            {
                _bufImage.Dispose();
                _bufImage = null;
            }
        }

        public void AddDelLife(int px, int py)
        {
            int i = (px - px % m_size) / m_size;
            int j = (py - py % m_size) / m_size;

            _thread.AddDelLife(i, j);
        }

        public Image PaintImage()
        {
            InitBitmap();
            DrawGrid();
            DrawInfo();

            Brush b;
            for (int i = 0; i < _cells; i++)
            {
                for (int j = 0; j < _cells; j++)
                {
                    if (_thread.Life[i, j])
                    {
                        if (_thread.Remain[i, j]) b = Brushes.Blue;
                        else b = Brushes.Green;

                        using (GraphicsPath path = RoundedRectangle.Create(i * m_size, j * m_size, m_size, m_size, m_radius))
                        {
                            _bufGraphics.FillPath(b, path);
                        }
                    }
                }
            }
            _bufGraphics.DrawImage(_bufImage, 0, 0);
            return _bufImage;
        }

        public void ShowCellStatus(int px, int py)
        {
            int i = (px - px % m_size) / m_size;
            int j = (py - py % m_size) / m_size;

            _thread.ShowCellStatus(i, j);
        }

        public void StartStopGame()
        {
            _thread.StartStopGame();
        }

        public void ClearCell()
        {
            _thread.ClearCell();
        }

        public void RandomStart()
        {
            _thread.RandomStart();
        }

        public void GliderStart()
        {
            _thread.GliderStart();
        }

        public void SelectOrDeselectNoise(bool noise)
        {
            _noise = noise;
        }

        //..

        private void Init(Size size)
        {
            _size = size;

            _bufImage = new Bitmap(_size.Width, _size.Height, PixelFormat.Format32bppArgb);
            _bufGraphics = Graphics.FromImage(_bufImage);

            Start();
        }

        private void Start()
        {
            if (_th == null)
            {
                _th = new Thread(_thread.Run);
                _th.Start();
            }
        }

        private void Stop()
        {
            if (_th != null)
            {
                _thread.StartFlag = false;
                if (!_th.Join(200))
                    _th.Abort();
                _th = null;
            }
        }
        
        private void InitBitmap()
        {
            _bufGraphics.FillRectangle(Brushes.White, 0, 0, _size.Width, _size.Height);
        }

        private void DrawGrid()
        {
            for (int i = 0; i <= _cells; i++)
            {
                _bufGraphics.DrawLine(Pens.Gray, 0, i * m_size, m_size * _cells, i * m_size);
                _bufGraphics.DrawLine(Pens.Gray, i * m_size, 0, i * m_size, m_size * _cells);
            }
        }

        private void DrawInfo()
        {
            _info.Draw();
        }
    }
}
