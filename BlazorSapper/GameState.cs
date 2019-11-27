using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorSapper
{
    public class GameState
    {
        private Timer _timer;

        private Random _random = new Random();
        private TileModel[,] _mineField;
        private readonly SoundHelper _sounds;

        public GameState(Action stateChangedCallback, SoundHelper sounds)
        {
            _mineField = new TileModel[0, 0];
            _timer = new Timer(_ =>
            {
                TimeElapsed += TimeSpan.FromSeconds(1);
                stateChangedCallback();
            }, null, 0, Timeout.Infinite);
            _sounds = sounds;
        }

        public int Difficulty { get; set; } = 1;

        public int FieldSize { get; private set; }

        public TimeSpan TimeElapsed { get; set; }

        public int TotalFlagsAvailable { get; private set; }

        public int MinesCount => Difficulty * 10;

        public int FlagsPlaced { get; private set; } = 0;

        public bool IsRunning { get; set; }

        public bool IsOver { get; set; }

        public bool IsVictory { get; set; }

        public void Start()
        {
            TotalFlagsAvailable = (Difficulty + 1) * 10;
            FieldSize = Difficulty < 3 ? 10 : 13;
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

            FlagsPlaced = 0;
            IsRunning = true;
            IsOver = false;
            TimeElapsed = TimeSpan.Zero;
            _timer.Change(0, 1000);
        }

        public TileModel GetTile(int x, int y) => _mineField[x, y];

        public async ValueTask OnFlagPlaced(TileModel tile)
        {
            TotalFlagsAvailable--;

            if (tile.IsMine)
            {
                FlagsPlaced++;
            }

            if (FlagsPlaced == MinesCount || TotalFlagsAvailable == 0)
            {
                End(FlagsPlaced == MinesCount);
                await _sounds.GameWin();
            }
        }

        public ValueTask OnFlagRemoved(TileModel tile)
        {
            if (tile.IsMine)
            {
                FlagsPlaced--;
            }

            return new ValueTask(Task.CompletedTask);
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
    }
}
