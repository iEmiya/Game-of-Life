using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameOfLife.Core
{
    internal sealed class GameInfo
    {
        private const string Bug = "bug occured!";

        private const string Name = "C Sharp game of life";

        private const string NewBornCellEn = "new born cell";
        private const string RemainCellEn = "remain cell";

        private const string GenerationInfoEn = "generation: ";
        private const string PopulationInfoEn = "population: ";
        private const string PercentageInfoEn = "percentage: ";
        
        private const string NewBornCellDetailEn = "new born cell: ";
        private const string RemainCellDetailEn = "remain cell: ";


        private readonly string[] _info = new string[3];
        private readonly string[] _detail = new string[2];
        private readonly string[] _label = new string[2];
        private readonly Font _font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));

        private readonly IGameDraw _game;

        public GameInfo(IGameDraw game)
        {
            _game = game;

            _label[0] = NewBornCellEn;
            _label[1] = RemainCellEn;
        }

        public void Draw()
        {
            using (GraphicsPath path = RoundedRectangle.Create(_game.Size.Width - 110, 60, Game.m_size, Game.m_size, Game.m_radius))
            {
                _game.BufGraphics.FillPath(Brushes.Green, path);
            }
            using (GraphicsPath path = RoundedRectangle.Create(_game.Size.Width - 110, 80, Game.m_size, Game.m_size, Game.m_radius))
            {
                _game.BufGraphics.FillPath(Brushes.Blue, path);
            }

            _info[0] = GenerationInfoEn + _game.Generation;
            _info[1] = PopulationInfoEn + _game.Population;
            _info[2] = PercentageInfoEn + (int)((double)_game.Population / Math.Pow((double)_game.Cells, 2) * 100);

            _detail[0] = NewBornCellDetailEn + _game.NewBornCell;

            int remaincell = _game.Population - _game.NewBornCell;
            _detail[1] = RemainCellDetailEn + remaincell;


            _game.BufGraphics.DrawString(Name, _font, Brushes.Black, _game.Size.Width - 130, 30);

            for (int number = 0; number < _label.Length; number++)
            {
                _game.BufGraphics.DrawString(_label[number], _font, Brushes.Black, _game.Size.Width - 90, 60 + number * 20);
            }
            for (int number = 0; number < _info.Length; number++)
            {
                _game.BufGraphics.DrawString(_info[number], _font, Brushes.Black, _game.Size.Width - 110, 130 + number * 20);
            }
            for (int number = 0; number < _detail.Length; number++)
            {
                _game.BufGraphics.DrawString(_detail[number], _font, Brushes.Black, _game.Size.Width - 110, 200 + number * 20);
            }

            if (!_game.Noise)
            {
                if (_game.NewBornCell < 0 || remaincell < 0)
                {
                    _game.BufGraphics.DrawString(Bug, _font, Brushes.Red, _game.Size.Width - 110, 250);
                }
            }
        }
    }
}
