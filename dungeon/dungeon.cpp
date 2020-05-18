#include <iostream>
#include <ctime>
#include <unistd.h>

using namespace std;

#define MAP_SIZE 20
#define ROOM_NUMBER 20

int room[7][4] = {
    1, 3, 5, 7, // I
    2, 4, 5, 7, // Z
    3, 5, 4, 6, // S
    3, 5, 4, 7, // T
    2, 3, 5, 7, // L
    3, 5, 7, 6, // J
    2, 3, 4, 5, // O
};

struct point
{
    int x, y;
};

point a[4], b[4];

int map[MAP_SIZE][MAP_SIZE] = {0};

void print_map()
{
    for (int i = 0; i < MAP_SIZE; i++)
    {
        for (int j = 0; j < MAP_SIZE; j++)
        {
            cout << ((map[i][j] == 0) ? ' ' : char(64 + map[i][j]));
        }
        cout << endl;
    }
}

// base room
void init()
{
    map[MAP_SIZE / 2 - 1][MAP_SIZE / 2 - 1] = 1;
    map[MAP_SIZE / 2 - 1][MAP_SIZE / 2] = 1;
    map[MAP_SIZE / 2][MAP_SIZE / 2 - 1] = 1;
    map[MAP_SIZE / 2][MAP_SIZE / 2] = 1;
}

void generate_one_room()
{
    // generate
    int n = rand() % 7;
    for (int i = 0; i < 4; i++)
    {
        a[i].x = room[n][i] % 2;
        a[i].y = room[n][i] / 2;
    }

    // rotate
    n = rand() % 4;
    for (int i = 0; i < n; i++)
    {
        point p = a[1];
        for (int j = 0; j < 4; j++)
        {
            int x = a[j].y - p.y;
            int y = a[j].x - p.x;
            a[j].x = p.x - x;
            a[j].y = p.y + y;
        }
    }

    // move to base room
    point p = a[1];
    for (int i = 0; i < 4; i++)
    {
        a[i].x += MAP_SIZE / 2 - p.x;
        a[i].y += MAP_SIZE / 2 - p.y;
    }
}

point spread_direction;

void next_spread_direction()
{
    if (spread_direction.x == 0)
    {
        spread_direction.x = spread_direction.y;
        spread_direction.y = 0;
    }
    else
    {
        spread_direction.y = -spread_direction.x;
        spread_direction.x = 0;
    }
}

void check()
{
    bool over = false;
    while (!over)
    {
        for (int i = 0; i < 4; i++)
        {
            if (map[a[i].x][a[i].y] != 0)
            {
                over = false;
                break;
            }
            else
            {
                over = true;
            }
        }

        if (!over)
        {
            bool done_flag = true;
            for (int i = 0; i < 4; i++)
            {
                b[i].x = a[i].x + spread_direction.x;
                b[i].y = a[i].y + spread_direction.y;

                if (b[i].x < 0 || b[i].x >= MAP_SIZE || b[i].y < 0 || b[i].y >= MAP_SIZE)
                {
                    done_flag = false;
                }
            }

            if (done_flag)
            {
                for (int i = 0; i < 4; i++)
                {
                    a[i].x += spread_direction.x;
                    a[i].y += spread_direction.y;
                }
            }

            int n = rand() % 4;
            for (int j = 0; j < n; j++)
            {
                next_spread_direction();
            }
        }
    }
}

void spread(int num)
{
    usleep(500000);
    check();
    for (int i = 0; i < 4; i++)
    {
        map[a[i].x][a[i].y] = num;
    }

    print_map();
    cout << endl;
}

int main()
{
    srand(time(NULL));
    for (int i = 0; i < 10; i++)
    {
        rand();
    }

    spread_direction.x = 0;
    spread_direction.y = -1;

    init();
    print_map();
    cout << endl;
    for (int i = 0; i < ROOM_NUMBER; i++)
    {
        generate_one_room();
        spread(i + 2);
    }

    return 0;
}