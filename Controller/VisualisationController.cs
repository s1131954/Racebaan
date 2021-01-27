using System;
using System.Collections.Generic;
using System.Text;
using Model;

namespace Controller
{
    public static class VisualisationController
    {
       
        public static int[,] CalcTrackCursorPositions(int[] TrackSectionOrientations , int trackLenght, int startOrientation)
        {
            int[,] TrackCursorPositions = new int[trackLenght, 2];
            int orientation = startOrientation;
            int column = 0;
            int row = 1; //1 to show track title

            for (int i = 0; i < trackLenght; i++)
            {
                if (i == 0)
                {
                    //Inialize first array item with choosen values. Start of track.
                    orientation = TrackSectionOrientations[0];
                    TrackCursorPositions[i, 0] = column;
                    TrackCursorPositions[i, 1] = row;
                }
                else
                {
                    orientation = TrackSectionOrientations[i - 1];

                    switch (orientation)
                    {
                        case 1:
                            column += 5;
                            break;
                        case 2:
                            row += 5;
                            break;
                        case 3:
                            column -= 5;
                            break;
                        case 4:
                            row -= 5;
                            break;
                    }
                }
                TrackCursorPositions[i, 0] = column;
                TrackCursorPositions[i, 1] = row;
            }
            return TrackCursorPositions;
        }

        public static int[,] Fix2dArrayTrackCursorPositionsColumn(int[,] trackCursorPositions, Array[] trackSections)
        {
            int lowestInt = trackCursorPositions[0, 0];
            int[,] result = trackCursorPositions;
            for (int i = 0; i < trackSections.Length; i++)
            {
                int next = trackCursorPositions[i, 0];
                if (next < lowestInt)
                {
                    lowestInt = trackCursorPositions[i, 0];
                }
            }

            if (lowestInt < 0)
            {
                for (int i = 0; i < trackSections.Length; i++)
                {
                    result[i, 0] = trackCursorPositions[i, 0] + (-lowestInt);
                }
            }
            return result;
        }

        public static int[,] Fix2dArrayTrackCursorPositionsRow(int[,] trackCursorPositions, Array[] trackSections)
        {
            int lowestInt = trackCursorPositions[0, 1];
            int[,] result = trackCursorPositions;
            for (int i = 0; i < trackSections.Length; i++)
            {
                int next = trackCursorPositions[i, 1];
                if (next < lowestInt)
                {
                    lowestInt = trackCursorPositions[i, 1];
                }
            }
            if (lowestInt < 0)
            {
                for (int i = 0; i < trackSections.Length; i++)
                {

                    trackCursorPositions[i, 1] = trackCursorPositions[i, 1] + (-lowestInt) + 1;
                }
            }
            return result;
        }

    }
}
