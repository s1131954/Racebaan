using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Model.Section;

namespace Racebaan
{
    public static class Visualisation
    {
        private static Race _race;
        private static Array[] _trackSections;
         private static int[] _trackSectionOrientations;
        private static int[,] _trackCursorPositions;
        public static void Initialize(Track track)
        {

            _race = Data.CurrentRace;
            Console.WriteLine(Data.CurrentRace.Track.Name);

            //Init arrays
            _trackSections = new Array[track.Sections.Count];
            _trackSectionOrientations = new int[track.Sections.Count];

            //Convert Sections to string[]  in _trackSections array
            SectionConverter(track);

            _trackCursorPositions = VisualisationController.CalcTrackCursorPositions(_trackSectionOrientations, _trackSections.Length, 1);
            //Fix negative values in row and columns
            _trackCursorPositions = VisualisationController.Fix2dArrayTrackCursorPositionsColumn(_trackCursorPositions, _trackSections);
            _trackCursorPositions = VisualisationController.Fix2dArrayTrackCursorPositionsRow(_trackCursorPositions, _trackSections);

            DrawTrack();
        }

         #region graphics
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

        public static void DrawTrack ()//Track track)
        {
            for (int i = 0; i < _trackSections.Length; i++)
            {
                
                DrawSection(_trackCursorPositions[i, 0], _trackCursorPositions[i, 1], (string[])_trackSections[i]);
            }
        }

        public static void DrawSection(int left, int top, string[] section)
        {
            for (int i = 0; i < section.Length; i++)
            {

                Console.CursorLeft = left;
                Console.CursorTop = top + i;
                Console.WriteLine(section[i]);
            }
          
        }
        #region secties
        private static void SectionConverter(Track track)
        {
            int arrayPosition = 0;
            int orientation = 1;
            
           
            for (LinkedListNode<Section> node = track.Sections.First; node != null; node = node.Next)
            {
                ResetGraphics();
                SectionData sectionData = _race.GetSectionData(node.Value);
                switch (node.Value.SectionType)
                {
                    case SectionTypes.Straight:
                        if (orientation == 1 || orientation == 3)
                        {
                            if (orientation == 1)
                            {
                                _horizontal = ReplaceSection(_horizontal, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_horizontal.Clone();
                            }
                            else if (orientation == 3)
                            {
                                _horizontal = GetParticipantsOnSections(_horizontal, false);
                                _horizontal = ReplaceSection(_horizontal, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_horizontal.Clone();
                            }
                            _trackSectionOrientations[arrayPosition] = orientation;
                        }
                        else
                        {
                            if (orientation == 4)
                            {
                                _vertical = GetParticipantsOnSections(_vertical, true);
                                _vertical = ReplaceSection(_vertical, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_vertical.Clone();
                            }
                            else if (orientation == 2)
                            {
                                _vertical = ReplaceSection(_vertical, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_vertical.Clone();
                                _trackSectionOrientations[arrayPosition] = orientation;
                            }
                            _trackSectionOrientations[arrayPosition] = orientation;
                        }
                        break;
                    case SectionTypes.LeftCorner:
                        switch (orientation)
                        {
                            case 1:
                                _turnDown = ReplaceSection(_turnDown, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnDown.Clone();
                                _trackSectionOrientations[arrayPosition] = 4;
                                orientation = 4;
                                break;
                            case 4:
                                _turnLeft = ReplaceSection(_turnLeft, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnLeft.Clone();
                                _trackSectionOrientations[arrayPosition] = 3;
                                orientation = 3;
                                break;
                            case 3:
                                _turnRight = ReplaceSection(_turnRight, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnRight.Clone();
                                _trackSectionOrientations[arrayPosition] = 2;
                                orientation = 2;
                                break;
                            case 2:
                                _turnUp = ReplaceSection(_turnUp, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnUp.Clone();
                                _trackSectionOrientations[arrayPosition] = 1;
                                orientation = 1;
                                break;
                        }
                        break;
                    case SectionTypes.RightCorner:
                        switch (orientation)
                        {
                            case 1:
                                _turnLeft = ReplaceSection(_turnLeft, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnLeft.Clone();
                                _trackSectionOrientations[arrayPosition] = 2;
                                orientation = 2;
                                break;
                            case 2:
                                _turnDown = ReplaceSection(_turnDown, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnDown.Clone();
                                _trackSectionOrientations[arrayPosition] = 3;
                                orientation = 3;
                                break;
                            case 3:
                                _turnUp = ReplaceSection(_turnUp, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnUp.Clone();
                                _trackSectionOrientations[arrayPosition] = 4;
                                orientation = 4;
                                break;
                            case 4:
                                _turnRight = ReplaceSection(_turnRight, sectionData.Left, sectionData.Right);
                                _trackSections[arrayPosition] = (string[])_turnRight.Clone();
                                _trackSectionOrientations[arrayPosition] = 1;
                                orientation = 1;
                                break;
                        }
                        break;
                    case SectionTypes.StartGrid:
                        _start = ReplaceSection(_start, sectionData.Left, sectionData.Right);
                        _trackSections[arrayPosition] = (string[])_start.Clone();
                        _trackSectionOrientations[arrayPosition] = orientation;
                        break;
                    case SectionTypes.FinishGrid:
                        _finishHorizontal = ReplaceSection(_finishHorizontal, sectionData.Left, sectionData.Right);
                        _trackSections[arrayPosition] = (string[])_finishHorizontal.Clone();
                        _trackSectionOrientations[arrayPosition] = orientation;
                        break;
                    default:
                        break;
                }
                arrayPosition++;
            }
        }
        #endregion
        public static string[] ReplaceSection(string[] section, IParticipant p1, IParticipant p2) { 
            string[] result = new string[5];
            bool driversOnSection = (p1 != null || p2 != null);

            for (int i = 0; i < 5; i++)
            {
                result[i] = (string)ReplaceString(section[i], p1, p2, driversOnSection).Clone();
            }

            return result;
        }

        public static string ReplaceString(string toReplace, IParticipant p1, IParticipant p2, bool DriverPosition)
        {
            string result = toReplace;
            //Driver or drivers on section replace with driver first letter of name
            if (DriverPosition)
            {
                //Check if only one side of the section is occupied by a driver.
                if (p1 != null && p2 == null)
                {
                    result = toReplace.Replace("1", p1.Name.Substring(0, 1)).Replace("2", " ");
                }
                else if (p1 == null && p2 != null)
                {
                    result = toReplace.Replace("1", " ").Replace("2", p2.Name.Substring(0, 1));
                }
                else if (p1 != null && p2 != null)
                {
                    result = toReplace.Replace("1", p1.Name.Substring(0, 1)).Replace("2", p2.Name.Substring(0, 1));
                }
            }
            else
            {
                //No drivers on section, change placeholders with a space
                result = toReplace.Replace("1", " ").Replace("2", " ");
            }

            return result;
        }
        public static string[] GetParticipantsOnSections(string[] sectionString, bool orientation)
        {
            
            string[] result = sectionString;
            if (orientation)
            {
                if (sectionString[2] != "|2 1|")
                {
                    string a = sectionString[2];
                    a = a.Replace("1", "3");
                    a = a.Replace("2", "1");
                    a = a.Replace("3", "2");
                    result[2] = a;
                }
            }
            else
            {
                string a = sectionString[1].Replace("2", "1");
                string b = sectionString[2].Replace("1", "2");
                result[1] = a;
                result[2] = b;
            }
            return result;
        }
        public static void ReDrawTrack(Object source, EventArgs e)
        {
          
            SectionConverter(((DriversChangedEventArgs)e).Track);
            _trackCursorPositions = VisualisationController.CalcTrackCursorPositions(_trackSectionOrientations, _trackSections.Length, 1);
            _trackCursorPositions = VisualisationController.Fix2dArrayTrackCursorPositionsColumn(_trackCursorPositions, _trackSections);
            _trackCursorPositions = VisualisationController.Fix2dArrayTrackCursorPositionsRow(_trackCursorPositions, _trackSections);
            DrawTrack();
        }

                                                                                                                        
        public static void ResetGraphics()
        {
            _start = (string[])VisualisationController._start.Clone();
            _finishHorizontal = (string[])VisualisationController._finish.Clone();
            _horizontal = (string[])VisualisationController._horizontal.Clone();
            _vertical = (string[])VisualisationController._vertical.Clone();
            _turnDown = (string[])VisualisationController._turnS1.Clone();
            _turnUp = (string[])VisualisationController._turnW2.Clone();
            _turnRight = (string[])VisualisationController._turnN3.Clone();
            _turnLeft = (string[])VisualisationController._turnE4.Clone();
        }
    }
}
