using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;
using static Model.IParticipant;
using static Model.Section;

namespace Racebaan_Scherm
{
    public static class Visualize
    {
        #region graphics
        private const string finish = ".\\Plaatjes\\Finish.png";
        private const string startgrid = ".\\Plaatjes\\startgrid.png";

        private const string turnLeft = ".\\Platjes\\turn_Left.png";
        private const string turnDown = ".\\Plaatjes\\turn_Down.png";
        private const string turnUp = ".\\Plaatjes\\turn_Up.png";
        private const string turnRight = ".\\Plaatjes\\turn_Right.png";
        private const string horizontal = ".\\Plaatjes\\horizontal.png";
        private const string vertical = ".\\Plaatjes\verticalp.png";

        private const string Driver_vertical_red = ".\\Plaatjes\\auto_vertical_rood.png";
        private const string Driver_horizontal_red = ".\\Plaatjes\\auto_horizontal_rood.png";

        private const string Driver_vertical_blauw = ".\\Plaatjes\\auto_horizontal_blauw.png";
        private const string Driver_horizontal_blauw = ".\\Plaatjes\\auto_horizontal_blauw.png";

        #endregion

        #region graphics track
        private static string[] _blank = { "     ", "     ", "     ", "     ", "     " };
        private static string[] _finishHorizontal =
        {   "-----",
                "  1# ",
                "   # ",
                "  2# ",
                "-----" };
        private static string[] _horizontal =
            {   "-----",
                "  1  ",
                "     ",
                "  2  ",
                "-----"};
        private static string[] _vertical =
        {   "|   |",
            "|   |",
            "|2 1|",
            "|   |",
            "|   |"
        };

        private static string[] _start =
        {       "-----",
                "  1] ",
                "     ",
                "  2] ",
                "-----"};

        private static string[] _turnRight =
            {   "/----",
                "|    ",
                "| 1  ",
                "|  2 ",
                "|   /",};

        private static string[] _turnLeft =
            {   "----\\" ,
                "  1 |",
                "    |",
                " 2  |",
                "\\   |" };

        private static string[] _turnUp =
            {   "|   \\",
                "|   2",
                "|    ",
                "| 1  ",
                "\\---"};

        private static string[] _turnDown =
            {   "/   |",
                " 2  |",
                "    |",
                "  1 |",
                "----/"};

        #endregion 
        public enum Dir
        {
            North,
            South,
            West,
            East
        }
       
            public static void Initialize()
            {
                Data.newRace += onNewRace;
            }

            private static int minHeight, maxHeight, minWidth, maxWidth, currentX, currentY, startX, startY, trackWidth, trackHeight;
            private static Dir direction = Dir.East;


        public static BitmapSource drawTrack(Model.Track track)
        {
            startX = minWidth * -1;
            startY = minHeight * -1;
            trackWidth = startX + maxWidth;
            trackHeight = startY + maxHeight;
            Calculate(track);
            return make_images.CreateBitmapSourceFromGdiBitmap(convertTrack(track));
        }
        public static void clearConsole()
            {
                Console.Clear();
            }

            private static void drawToConsole(List<List<string[]>> raceTrack)
            {
                foreach (List<string[]> y in raceTrack)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        foreach (string[] x in y)
                        {

                            Console.Write(x[i]);
                        }
                        Console.WriteLine();
                    }
                }
            }

        private static string getCornerDir(Model.Section.SectionTypes type, Dir direction)
        {
            if (type == Model.Section.SectionTypes.LeftCorner)
            {
                switch (direction)
                {
                    case Dir.North:
                        return turnLeft;
                    case Dir.East:
                        return turnDown;
                    case Dir.South:
                        return turnUp;
                    case Dir.West:
                        return turnRight;
                }
            }
            else
            {
                switch (direction)
                {
                    case Dir.North:
                        return turnRight;
                    case Dir.East:
                        return turnLeft;
                    case Dir.South:
                        return turnDown;
                    case Dir.West:
                        return turnUp;
                }
            }

            return null;
        }

        //private static void displayBestParticipant()
        //{
        //    Console.WriteLine();
        //    Console.WriteLine($"Most points: {Data.Competition.points.findWinner()}");
        //    Console.WriteLine($"Best laptime: {Data.competition.lapTime.findWinner()}");
        //    Console.WriteLine();
        //}

        public static void Move()
            {
                switch (direction)
                {
                    case Dir.North:
                        currentY--;
                        break;
                    case Dir.East:
                        currentX++;
                        break;
                    case Dir.South:
                        currentY++;
                        break;
                    case Dir.West:
                        currentX--;
                        break;
                }
            }

            public static void RotateR()
            {
                switch (direction)
                {
                    case Dir.North:
                        direction = Dir.East;
                        break;
                    case Dir.East:
                        direction = Dir.South;
                        break;
                    case Dir.South:
                        direction = Dir.West;
                        break;
                    case Dir.West:
                        direction = Dir.North;
                        break;
                }
            }

            public static void RotateL()
            {
                switch (direction)
                {
                    case Dir.North:
                        direction = Dir.West;
                        break;
                    case Dir.East:
                        direction = Dir.North;
                        break;
                    case Dir.South:
                        direction = Dir.East;
                        break;
                    case Dir.West:
                        direction = Dir.South;
                        break;
                }
            }

            public static void Calculate(Track track)
            {
                foreach (var section in track.Sections)
                {
                    if (currentY < minHeight)
                    {
                        minHeight = currentY;
                    }

                    if (currentY > maxHeight)
                    {
                        maxHeight = currentY;
                    }

                    if (currentX < minWidth)
                    {
                        minWidth = currentX;
                    }

                    if (currentX > maxWidth)
                    {
                        maxWidth = currentX;
                    }
                    switch (section.SectionType)
                    {
                        case SectionTypes.StartGrid:
                        case SectionTypes.FinishGrid:
                        case SectionTypes.Straight:
                            Move();
                            break;
                        case SectionTypes.RightCorner:
                            RotateR();
                            Move();

                            break;
                        case SectionTypes.LeftCorner:
                            RotateL();
                            Move();
                            break;
                    }
                }
            }

        public static Bitmap convertTrack(Model.Track track)
        {
            Bitmap b = make_images.createEmpty((trackWidth + 1) * 70, (trackHeight + 1) * 70);
            Graphics g = Graphics.FromImage(b);
            IParticipant p1;
            IParticipant p2;
            currentX = startX;
            currentY = startY;
            foreach (var section in track.Sections)
            {
                p1 = Data.CurrentRace.GetSectionData(section).Left;
                p2 = Data.CurrentRace.GetSectionData(section).Right;
                switch (section.SectionType)
                {
                    case Model.Section.SectionTypes.StartGrid:
                        if (direction == Dir.East || direction == Dir.West)
                        {
                            g.DrawImage(make_images.returnBitmap(startgrid), new Point(currentX * 70, currentY * 70));
                            DrawPlayer(g, p1, p2, direction);
                        }
                        else
                        {
                            g.DrawImage(make_images.returnBitmap(startgrid), new Point(currentX * 70, currentY * 70));
                            DrawPlayer(g, p1, p2, direction);
                        }
                        Move();
                        break;
                    case Model.Section.SectionTypes.FinishGrid:
                        if (direction == Dir.East || direction == Dir.West)
                        {
                            g.DrawImage(make_images.returnBitmap(finish), new Point(currentX * 70, currentY * 70));
                        }
                        else
                        {
                            g.DrawImage(make_images.returnBitmap(finish), new Point(currentX * 70, currentY * 70));
                        }
                        DrawPlayer(g, p1, p2, direction);
                        Move();
                        break;
                    case Model.Section.SectionTypes.Straight:
                        if (direction == Dir.East || direction == Dir.West)
                        {
                            g.DrawImage(make_images.returnBitmap(horizontal), new Point(currentX * 70, currentY * 70));
                        }
                        else
                        {
                            g.DrawImage(make_images.returnBitmap(vertical), new Point(currentX * 70, currentY * 70));
                        }
                        DrawPlayer(g, p1, p2, direction);
                        Move();
                        break;
                    case Model.Section.SectionTypes.LeftCorner:

                        g.DrawImage(make_images.returnBitmap(getCornerDir(section.SectionType, direction)), new Point(currentX * 70, currentY * 70));
                        DrawPlayer(g, p1, p2, direction);
                        RotateL();
                        Move();
                        break;
                    case Model.Section.SectionTypes.RightCorner:
                        g.DrawImage(make_images.returnBitmap(getCornerDir(section.SectionType, direction)), new Point(currentX * 70, currentY * 70));
                        DrawPlayer(g, p1, p2, direction);
                        RotateR();
                        Move();
                        break;

                }
            }


            return b;
        }
       

        public static void DrawPlayer(Graphics g, IParticipant driver1, IParticipant driver2, Dir d)
        {
            if (driver1 != null)
            {
                {
                    switch (d)
                    {
                        case Dir.North:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver1, d)), 20, 20), new Point(currentX * 70, currentY * 70));
                            break;
                        case Dir.South:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver1, d)), 20, 20), new Point(currentX * 70, currentY * 70));
                            break;
                        case Dir.East:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver1, d)), 20, 20), new Point(currentX * 70, currentY * 70));
                            break;
                        case Dir.West:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver1, d)), 20, 20), new Point(currentX * 70, currentY * 70));
                            break;
                    }
                }
                
            }
            if (driver2 != null)
            {
                {
                    switch (d)
                    {
                        case Dir.North:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver2, d)), 20, 20), new Point(currentX * 70 + 35, currentY * 70));
                           
                            break;
                        case Dir.South:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver2, d)), 20, 20), new Point(currentX * 70 + 35, currentY * 70));
                           
                            break;
                        case Dir.East:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver2, d)), 20, 20), new Point(currentX * 70, currentY * 70 + 35));
                            
                            break;
                        case Dir.West:
                            g.DrawImage(new Bitmap(make_images.returnBitmap(GetBitmap(driver2, d)), 20, 20), new Point(currentX * 70, currentY * 70 + 35));
                           
                            break;
                    }
                }
            }
        }

        public static string GetBitmap(IParticipant p, Dir d)
        {
            if (p.TeamColor == TeamColors.Red)
            {
                switch (d)
                {
                    case Dir.North:
                        return Driver_vertical_red;
                        break;
                    case Dir.South:
                        return Driver_vertical_red;
                        break;
                    case Dir.East:
                        return Driver_horizontal_red;
                        break;
                    case Dir.West:
                        return Driver_horizontal_red;
                        break;
                }
            }
            else if (p.TeamColor == TeamColors.Blue)
            {
                switch (d)
                {
                    case Dir.North:
                        return Driver_vertical_blauw;
                        break;
                    case Dir.South:
                        return Driver_vertical_blauw;
                        break;
                    case Dir.East:
                        return Driver_horizontal_blauw;
                        break;
                    case Dir.West:
                        return Driver_horizontal_blauw;
                        break;
                }
            }
           
           
            return "";
        }
        public static List<List<string[]>> blankList()
            {
                List<List<string[]>> trackList = new List<List<string[]>>();
                for (int i = 0; i <= trackHeight; i++)
                {
                    List<string[]> innerList = new List<string[]>();
                    for (int j = 0; j <= trackWidth; j++)
                    {
                        innerList.Add(_blank);
                    }
                    trackList.Add(innerList);
                }

                return trackList;
            }

            public static string replaceNumbers(string input, IParticipant character1, IParticipant character2)
            {
                if (character1 != null)
                {
                    if (character1.Equipment.IsBroken)
                    {
                        input = input.Replace('1', '!');
                    }
                    else
                    {
                        input = input.Replace('1', character1.Name[0]);
                    }
                }
                else
                {
                    input = input.Replace('1', ' ');
                }

                if (character2 != null)
                {
                    if (character2.Equipment.IsBroken)
                    {
                        input = input.Replace('2', '!');
                    }
                    else
                    {
                        input = input.Replace('2', character2.Name[0]);
                    }
                }
                else
                {
                    input = input.Replace('2', ' ');
                }
                return input;
            }

            public static string[] replaceSection(string[] input, IParticipant character1, IParticipant character2)
            {
                string[] result = new string[4];
                input.CopyTo(result, 0);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = replaceNumbers(result[i], character1, character2);
                }

                return result;
            }


            public static void onDriversChanged(object o, DriversChangedEventArgs e)
            {
                drawTrack(e.Track);
            }

            public static void onNewRace(object o, EventArgs e)
            {
                //displayBestParticipant();
                Data.CurrentRace.DriversChanged += onDriversChanged;

            }
        
    }
}
