using System;
using System.Collections.Generic;

namespace GameScripts
{
    public class TetrisDungeon
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public List<TetrisDungeonRoom> Rooms = new List<TetrisDungeonRoom>();

        private int _maxMapSize;
        private int _roomNumber;

        private int[,] _map;
        private TetrisDungeonPoint _spreadDirection = new TetrisDungeonPoint { x = 0, y = -1 };

        /// <summary>
        /// Init the base room
        /// </summary>
        private void Initialization()
        {
            _map[_maxMapSize / 2 - 1, _maxMapSize / 2 - 1] = 1;
            _map[_maxMapSize / 2 - 1, _maxMapSize / 2] = 1;
            _map[_maxMapSize / 2, _maxMapSize / 2 - 1] = 1;
            _map[_maxMapSize / 2, _maxMapSize / 2] = 1;
        }

        private void RotateSpreadDirection()
        {

            int rotationTimes = Random.Next(0, 4);
            for (int j = 0; j < rotationTimes; j++)
            {
                if (_spreadDirection.x == 0)
                {
                    _spreadDirection.x = _spreadDirection.y;
                    _spreadDirection.y = 0;
                }
                else
                {
                    _spreadDirection.y = -_spreadDirection.x;
                    _spreadDirection.x = 0;
                }
            }
        }

        private void SpreadRoom(TetrisDungeonRoom room, int roomID)
        {
            // Move the room to the center of the map
            TetrisDungeonPoint basePoint = new TetrisDungeonPoint();
            basePoint.x = room.Points[1].x;
            basePoint.y = room.Points[1].y;
            for (int i = 0; i < 4; i++)
            {
                room.Points[i].x += _maxMapSize / 2 - basePoint.x;
                room.Points[i].y += _maxMapSize / 2 - basePoint.y;
            }

            // Spread the room to the nearest empty place
            bool done = false;
            while (!done)
            {
                // Check the overlap
                for (int i = 0; i < 4; i++)
                {
                    if (_map[room.Points[i].x, room.Points[i].y] != 0)
                    {
                        done = false;
                        break;
                    }
                    else
                    {
                        done = true;
                    }
                }

                if (!done)
                {
                    TetrisDungeonRoom tempRoom = new TetrisDungeonRoom();
                    bool canMove = false;
                    while (!canMove)
                    {
                        canMove = true;
                        for (int i = 0; i < 4; i++)
                        {
                            tempRoom.Points[i].x = room.Points[i].x + _spreadDirection.x;
                            tempRoom.Points[i].y = room.Points[i].y + _spreadDirection.y;

                            if (tempRoom.Points[i].x < 0
                                || tempRoom.Points[i].x >= _maxMapSize
                                || tempRoom.Points[i].y < 0
                                || tempRoom.Points[i].y >= _maxMapSize)
                            {
                                RotateSpreadDirection();

                                canMove = false;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        room.Points[i].x = tempRoom.Points[i].x;
                        room.Points[i].y = tempRoom.Points[i].y;
                    }
                    RotateSpreadDirection();
                }
            }

            // Set room to map
            for (int i = 0; i < 4; i++)
            {
                _map[room.Points[i].x, room.Points[i].y] = roomID;
            }

            Rooms.Add(room);
        }

        private void GenerateDungeonMap()
        {
            Initialization();
            for (int i = 0; i < _roomNumber; i++)
            {
                SpreadRoom(TetrisDungeonRoom.RandomRoom(), i + 2);
            }
        }

        public TetrisDungeon(int maxMapSize, int roomNumber)
        {
            _maxMapSize = maxMapSize;
            _roomNumber = roomNumber;

            _map = new int[_maxMapSize, _maxMapSize];
            for (int i = 0; i < _maxMapSize; i++)
            {
                for (int j = 0; j < _maxMapSize; j++)
                {
                    _map[i, j] = 0;
                }
            }
            GenerateDungeonMap();
        }

        #region For Test
        public int[,] GetMapData()
        {
            return _map;
        }
        #endregion
    }

    public class TetrisDungeonPoint
    {
        public int x, y;
    }

    public class TetrisDungeonRoom
    {
        public TetrisDungeonPoint[] Points = new TetrisDungeonPoint[4];

        public static List<List<int>> RoomTemplates = new List<List<int>> {
            new List<int> { 1, 3, 5, 7 }, // I
            new List<int> { 2, 4, 5, 7 }, // Z
            new List<int> { 3, 5, 4, 6 }, // S
            new List<int> { 3, 5, 4, 7 }, // T
            new List<int> { 2, 3, 5, 7 }, // L
            new List<int> { 3, 5, 7, 6 }, // J
            new List<int> { 2, 3, 4, 5 }, // O
        };

        public TetrisDungeonRoom()
        {
            for (int i = 0; i < 4; i++)
            {
                Points[i] = new TetrisDungeonPoint();
            }
        }

        /// <summary>
        /// Generate a random room
        /// </summary>
        /// <returns>The room</returns>
        public static TetrisDungeonRoom RandomRoom()
        {
            TetrisDungeonRoom room = new TetrisDungeonRoom();

            // Generate with random template
            int template = TetrisDungeon.Random.Next(0, 7);
            for (int i = 0; i < 4; i++)
            {
                room.Points[i].x = RoomTemplates[template][i] % 2;
                room.Points[i].y = RoomTemplates[template][i] / 2;
            }

            // Rotation room
            int rotationTime = TetrisDungeon.Random.Next(0, 4);
            for (int i = 0; i < rotationTime; i++)
            {
                TetrisDungeonPoint center = new TetrisDungeonPoint();
                center.x = room.Points[1].x;
                center.y = room.Points[1].y;
                for (int j = 0; j < 4; j++)
                {
                    int x = room.Points[j].y - center.y;
                    int y = room.Points[j].x - center.x;
                    room.Points[j].x = center.x - x;
                    room.Points[j].y = center.y + y;
                }
            }

            return room;
        }
    }
}