using System.Text;
using PuzzleSolverLib.Common;
using Spectre.Console;

namespace PuzzleSolverLib.Puzzles.Y2024;

using MapPoint = (int x, int y);

public class Day21c : PuzzleBaseClass
{
    public class LayoutNames
    {
        public const string Human = "HumanPanel";
        public const string HumanPressedKeys = "Pressed (human)";
        public const string HumanInput = "Input (human)";
        public const string FreezingRobot = "FreezingPanel";
        public const string FreezingRobotPressedKeys = "Pressed (freezing)";
        public const string FreezingRobotInput = "Input (freezing)";
        public const string RadiationRobot = "RadiationPanel";
        public const string RadiationPressedKeys = "Pressed (radiation)";
        public const string RadiationInput = "Input (radiation)";
        public const string DepressurizedRobot = "DepressurizedPanel";
        public const string DepressurizedPressedKeys = "Pressed (depressurized)";
        public const string DepressurizedInput = "Input (depressurized)";
        public const string HumanHeader = "Human";
        public const string FreezingHeader = "Freezing";
        public const string RadiationHeader = "Radiation";
        public const string DepressurizedHeader = "Depressurized";
    }

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {




        var layout = new Layout();
        layout.SplitRows(
            new Layout("Robots & Keypads").SplitColumns(
                new Layout(LayoutNames.Human).SplitRows(
                    new Layout(LayoutNames.HumanHeader),
                    new Layout(LayoutNames.HumanPressedKeys)
                    ),
                new Layout(LayoutNames.FreezingRobot).SplitRows(
                    new Layout(LayoutNames.FreezingHeader),
                    new Layout(LayoutNames.FreezingRobotInput),
                    new Layout(LayoutNames.FreezingRobotPressedKeys)
                    ),
                new Layout(LayoutNames.RadiationRobot).SplitRows(
                    new Layout(LayoutNames.RadiationHeader),
                    new Layout(LayoutNames.RadiationInput),
                    new Layout(LayoutNames.RadiationPressedKeys)
                    ),
                new Layout(LayoutNames.DepressurizedRobot).SplitRows(
                    new Layout(LayoutNames.DepressurizedHeader),
                    new Layout(LayoutNames.DepressurizedInput),
                    new Layout(LayoutNames.DepressurizedPressedKeys)
                    )
            ));

        layout[LayoutNames.HumanHeader].Update(new Text("Human"));
        layout[LayoutNames.FreezingHeader].Update(new Text("Freezing robot"));
        layout[LayoutNames.RadiationHeader].Update(new Text("Radiation robot"));
        layout[LayoutNames.DepressurizedHeader].Update(new Text("Depressurized robot"));

        //layout[LayoutNames.Human].Update(new Panel("Human"));
        //layout[LayoutNames.FreezingRobot].Update(new Panel("Freezing Robot"));
        //layout[LayoutNames.RadiationRobot].Update(new Panel("Radiation Robot"));
        //layout[LayoutNames.DepressurizedRobot].Update(new Panel("Depressurized Robot"));

        AnsiConsole.Write(layout);


        var controller = new Controller(layout);

        controller.GetUserInput("<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>Av<<A>>^A<A>AvA<^AA>A<vAAA>^A<A^A>^^AvvvA");


        return "wip";
    }

    public interface IKeyPad
    {
        void MoveDown();
        void MoveLeft();
        void MoveRight();
        void MoveUp();
        void Push();
        void PressButton(ConsoleKeyInfo key);
        void PressButton(ConsoleKey key);

    }

    public class NumericalKeyPad(Action<char> buttonPressedCallback) : IKeyPad
    {
        private readonly char[,] _keyPad = new char[4, 3]
        {
            {'7', '8', '9'},
            {'4', '5', '6'},
            {'1', '2', '3'},
            {' ', '0', 'A'}
        };

        private MapPoint _currentPosition = (2, 3);

        private MapPoint _startPosition = (2, 3);

        private void SetPosition(MapPoint position)
        {
            _currentPosition.x += position.x;
            _currentPosition.y += position.y;

            if (_currentPosition.x < 0)
                _currentPosition.x = 0;
            if (_currentPosition.x > _keyPad.GetUpperBound(1))
                _currentPosition.x = _keyPad.GetUpperBound(1);

            if (_currentPosition.y < 0)
                _currentPosition.y = 0;
            if (_currentPosition.y > _keyPad.GetUpperBound(0))
                _currentPosition.y = _keyPad.GetUpperBound(0);

            if (_currentPosition.x == 0 && _currentPosition.y == 3)
                throw new Exception("PANIC!");
        }

        #region

        public void MoveUp()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Up));
        }

        public void MoveDown()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Down));
        }

        public void MoveLeft()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Left));
        }

        public void MoveRight()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Right));
        }

        public void Push()
        {
            buttonPressedCallback.Invoke(_keyPad[_currentPosition.y, _currentPosition.x]);
        }

        public void PressButton(ConsoleKey key)
        {
            var ch = ' ';
            switch (key)
            {
                case ConsoleKey.NumPad0:
                case ConsoleKey.NumPad1:
                case ConsoleKey.NumPad2:
                case ConsoleKey.NumPad3:
                case ConsoleKey.NumPad4:
                case ConsoleKey.NumPad5:
                case ConsoleKey.NumPad6:
                case ConsoleKey.NumPad7:
                case ConsoleKey.NumPad8:
                case ConsoleKey.NumPad9:
                    ch = (char)('0' + key - ConsoleKey.NumPad0);
                    break;
                case ConsoleKey.D0:
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.D4:
                case ConsoleKey.D5:
                case ConsoleKey.D6:
                case ConsoleKey.D7:
                case ConsoleKey.D8:
                case ConsoleKey.D9:
                    ch = (char)('0' + key - ConsoleKey.D0);
                    break;
                case ConsoleKey.Enter:
                    ch = 'A';
                    break;
                default:
                    return;
            }
            buttonPressedCallback.Invoke(ch);
        }
        public void PressButton(ConsoleKeyInfo key)
        {
            PressButton(key.Key);
        }

        #endregion
    }

    public class DirectionalKeyPad(Action<char> buttonPressedCallback) : IKeyPad
    {
        private readonly char[,] _keyPad = new char[2, 3] {{' ', '^', 'A'}, {'<', 'v', '>'}};
        private MapPoint _currentPosition = (2, 0);

        private MapPoint _startPosition = (2, 0);

        private void SetPosition(MapPoint position)
        {
            _currentPosition.x += position.x;
            _currentPosition.y += position.y;

            if (_currentPosition.x < 0)
                _currentPosition.x = 0;
            if (_currentPosition.x > _keyPad.GetUpperBound(1))
                _currentPosition.x = _keyPad.GetUpperBound(1);

            if (_currentPosition.y < 0)
                _currentPosition.y = 0;
            if (_currentPosition.y > _keyPad.GetUpperBound(0))
                _currentPosition.y = _keyPad.GetUpperBound(0);

            if (_currentPosition.x == 0 && _currentPosition.y == 0)
                throw new Exception("PANIC!");
        }

        #region

        public void MoveUp()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Up));
        }

        public void MoveDown()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Down));
        }

        public void MoveLeft()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Left));
        }

        public void MoveRight()
        {
            SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Right));
        }

        public void Push()
        {
            buttonPressedCallback.Invoke(_keyPad[_currentPosition.y, _currentPosition.x]);
        }

        public void PressButton(ConsoleKeyInfo key)  => PressButton(key.Key);
        public void PressButton(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    buttonPressedCallback.Invoke('^');
                    break;
                case ConsoleKey.DownArrow:
                    buttonPressedCallback.Invoke('v');
                    break;
                case ConsoleKey.LeftArrow:
                    buttonPressedCallback.Invoke('<');
                    break;
                case ConsoleKey.RightArrow:
                    buttonPressedCallback.Invoke('>');
                    break;
                case ConsoleKey.Enter:
                    buttonPressedCallback.Invoke('A');
                    break;
            }
        }

        #endregion
    }

    public class Controller
    {
        private readonly Layout _layout;
        private readonly IKeyPad _depressurizedRoom;

        private readonly IKeyPad _frozenStorage;

        private readonly IKeyPad _radiationRoom;

        public Controller(Layout layout)
        {
            _layout = layout;
            DirectionalController = new DirectionalKeyPad(OnKeyPadPressed);
            _frozenStorage = new DirectionalKeyPad(OnFrozenKeyPadPressed);
            _radiationRoom = new DirectionalKeyPad(OnRadiationKeyPadPressed);
            _depressurizedRoom = new NumericalKeyPad(OnNumericalKeyPadPressed);
        }

        public void GetUserInput(string inputAsString, int delay = 1000)
        {
            foreach (var key in inputAsString)
            {
                switch (key)
                {
                    case '<':
                        DirectionalController.PressButton(ConsoleKey.LeftArrow);
                        break;
                    case '>':
                        DirectionalController.PressButton(ConsoleKey.RightArrow);
                        break;
                    case '^':
                        DirectionalController.PressButton(ConsoleKey.UpArrow);
                        break;
                    case 'v':
                        DirectionalController.PressButton(ConsoleKey.DownArrow);
                        break;
                    case 'A':
                        DirectionalController.PressButton(ConsoleKey.Enter);
                        break;

                }

                Thread.Sleep(delay);
            }

            Console.ReadKey(true);
        }
        public void GetUserInput()
        {
            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                    return;

                DirectionalController.PressButton(key);
            }
        }


        public IKeyPad DirectionalController { get; }


        private StringBuilder _frozenPressedKeys = new StringBuilder();
        private void OnFrozenKeyPadPressed(char obj)
        {
            _frozenPressedKeys.Append(obj);
            _layout[LayoutNames.FreezingRobotPressedKeys].Update(
                new Paragraph(_frozenPressedKeys.ToString()));

            var radiation = new StringBuilder();

            switch (obj)
            {
                case '^':
                    _radiationRoom.MoveUp();
                    radiation.Append("up");
                    break;
                case '<':
                    _radiationRoom.MoveLeft();
                    radiation.Append("left");
                    break;
                case '>':
                    _radiationRoom.MoveRight();
                    radiation.Append("right");
                    break;
                case 'v':
                    _radiationRoom.MoveDown();
                    radiation.Append("down");
                    break;
                case 'A':
                    _radiationRoom.Push();
                    radiation.Append("push");
                    break;
            }
            _layout[LayoutNames.RadiationInput].Update(
                new Paragraph(radiation.ToString()));

            AnsiConsole.Write(_layout);
        }

        private StringBuilder _humanPressedKeys = new StringBuilder();

        private void OnKeyPadPressed(char obj)
        {
            _humanPressedKeys.Append(obj);

            _layout[LayoutNames.HumanPressedKeys].Update(
                new Paragraph(_humanPressedKeys.ToString()));

           var frozen = new StringBuilder();

            switch (obj)
            {
                case '^':
                    _frozenStorage.MoveUp();
                    frozen.Append("up");
                    break;
                case '<':
                    _frozenStorage.MoveLeft();
                    frozen.Append("left");
                    break;
                case '>':
                    _frozenStorage.MoveRight();
                    frozen.Append("right");
                    break;
                case 'v':
                    _frozenStorage.MoveDown();
                    frozen.Append("down");
                    break;
                case 'A':
                    _frozenStorage.Push();
                    frozen.Append("push");
                    break;
            }

            _layout[LayoutNames.FreezingRobotInput].Update(
                new Paragraph(frozen.ToString()));


            AnsiConsole.Write(_layout);
        }

        private StringBuilder _depressurizedPressedKeys = new StringBuilder();
        private void OnNumericalKeyPadPressed(char obj)
        {
            _depressurizedPressedKeys.Append(obj);
            _layout[LayoutNames.DepressurizedPressedKeys].Update(
                new Paragraph(_depressurizedPressedKeys.ToString()));
            AnsiConsole.Write(_layout);
        }

        private StringBuilder _radiationPressedKeys = new StringBuilder();
        private void OnRadiationKeyPadPressed(char obj)
        {
            _radiationPressedKeys.Append(obj);

            _layout[LayoutNames.RadiationPressedKeys].Update(
                new Paragraph(_radiationPressedKeys.ToString()));

            var depressurized = new StringBuilder();

            switch (obj)
            {
                case '^':
                    _depressurizedRoom.MoveUp();
                    depressurized.Append("up");
                    break;
                case '<':
                    _depressurizedRoom.MoveLeft();
                    depressurized.Append("left");
                    break;
                case '>':
                    _depressurizedRoom.MoveRight();
                    depressurized.Append("right");
                    break;
                case 'v':
                    _depressurizedRoom.MoveDown();
                    depressurized.Append("down");
                    break;
                case 'A':
                    _depressurizedRoom.Push();
                    depressurized.Append("push");
                    break;
            }
            _layout[LayoutNames.DepressurizedInput].Update(
                new Paragraph(depressurized.ToString()));

            AnsiConsole.Write(_layout);
        }
    }
}