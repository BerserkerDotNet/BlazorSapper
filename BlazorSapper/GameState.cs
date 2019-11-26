using System;
using System.Threading;

namespace BlazorSapper
{
    public class GameState
    {
        private Timer _timer;

        private Random _random = new Random();
        private TileModel[,] _mineField;
        private int _flaggedMines = 0;

        public GameState()
        {
            _mineField = new TileModel[0, 0];
            _timer = new Timer(_ =>
            {
                TimeElapsed += TimeSpan.FromSeconds(1);
            }, null, 0, Timeout.Infinite);
        }

        public void Start()
        {
            TotalFlagsAvailable = (Difficulty + 1) * 10;
            FieldSize = (int)(Math.Max((Difficulty - 0.8), 1) * 10);
            InitializeField();
            for (int i = 0; i < MinesCount; i++)
            {
                while (true)
                {
                    var x = _random.Next(0, FieldSize);
                    var y = _random.Next(0, FieldSize);

                    if (!_mineField[x, y].IsMine)
                    {
                        _mineField[x, y].Rank = -1;
                        UpdateAdjesantScores(x, y);
                        break;
                    }
                }
            }

            _flaggedMines = 0;
            IsRunning = true;
            IsOver = false;
            TimeElapsed = TimeSpan.Zero;
            _timer.Change(0, 1000);
        }

        private void InitializeField()
        {
            _mineField = new TileModel[FieldSize, FieldSize];
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    _mineField[i, j] = new TileModel(0, TileState.None, (i, j));
                }
            }
        }

        public int Difficulty { get; set; } = 1;

        public int FieldSize { get; private set; }

        public TimeSpan TimeElapsed { get; set; }

        public int TotalFlagsAvailable { get; private set; }

        public int MinesCount => Difficulty * 10;

        public bool IsRunning { get; set; }

        public bool IsOver { get; set; }

        public bool IsVictory { get; set; }

        public TileModel GetTile(int x, int y) => _mineField[x, y];

        public void OnFlagPlaced(TileModel tile)
        {
            TotalFlagsAvailable--;

            if (tile.IsMine)
            {
                _flaggedMines++;
            }

            if (_flaggedMines == MinesCount || TotalFlagsAvailable == 0)
            {
                End(_flaggedMines == MinesCount);
            }
        }

        public void OnFlagRemoved(TileModel tile)
        {
            if (tile.IsMine)
            {
                _flaggedMines--;
            }
        }

        public void OpenEmptyAdjesantTiles(int x, int y)
        {
            ForEachAdjesantTile(x, y, (dx, dy) =>
            {
                if (_mineField[dx, dy].State == TileState.None)
                {
                    _mineField[dx, dy].State = TileState.Open;
                    if (_mineField[dx, dy].Rank == 0)
                    {
                        OpenEmptyAdjesantTiles(dx, dy);
                    }
                }
            });
        }

        public void UpdateAdjesantScores(int x, int y)
        {
            ForEachAdjesantTile(x, y, (dx, dy) =>
            {
                if (!_mineField[dx, dy].IsMine)
                {
                    _mineField[dx, dy].Rank++;
                }
            });
        }

        public void ForEachAdjesantTile(int x, int y, Action<int, int> action)
        {
            var adjecentTiles = new[] { (-1, -1), (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0) };

            foreach (var adjecentTile in adjecentTiles)
            {
                var dx = x + adjecentTile.Item1;
                var dy = y + adjecentTile.Item2;
                if (dx >= 0 && dy >= 0 && dx < _mineField.GetLength(0) && dy < _mineField.GetLength(1))
                {
                    action(dx, dy);
                }
            }
        }

        public void ShowMines()
        {
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    if (_mineField[i, j].IsMine && _mineField[i, j].State != TileState.Flagged)
                    {
                        _mineField[i, j].State = TileState.Open;
                    }
                }
            }
        }

        public void Defeat() => End(false);

        public void Victory() => End(true);

        private void End(bool victory)
        {
            IsVictory = victory;
            IsOver = true;
            IsRunning = false;
            _timer.Change(0, System.Threading.Timeout.Infinite);
            ShowMines();
        }
    }
}
