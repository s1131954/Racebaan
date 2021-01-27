﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Model;
using static Model.IParticipant;
using static Model.Section;

namespace Controller
{
    public static class Data
    {
        public static Competition Competition { get; set; }
        public static event EventHandler UpdateNextRace;
        public static Track Track { get; private set; }
        private static Car Car1 = new Car(3, 4, 300, false);
        private static Car Car2 = new Car(2, 5, 250, false);
        private static Car Car3 = new Car(1, 4, 290, false);
        public static Driver Driver1 = new Driver("Rosa", 43, Car1, TeamColors.Red);
        public static Driver Driver2 = new Driver("Jeroen", 20, Car2, TeamColors.Red);
        //public static Driver Driver3 = new Driver("Angela", 42, Car3, TeamColors.Red);
        

        public static Race CurrentRace { get; set; }

        public static void Initialize()
        {
            Competition = new Competition();
            AddParticipants();
            AddTracks();
        }

        public static void AddParticipants()
        {
            Competition.Participants.Add(Driver1);
            Competition.Participants.Add(Driver2);
            //Competition.Participants.Add(Driver3);
        }

        public static void AddTracks()
        {
           // SectionTypes[] sectionTypes = {SectionTypes.Straight, SectionTypes.RightCorner,SectionTypes.Straight,SectionTypes.RightCorner,SectionTypes.Straight,SectionTypes.RightCorner
//};
            Track = new Track("Circuit Elburg", new SectionTypes[] { SectionTypes.Straight, SectionTypes.Straight, SectionTypes.StartGrid, SectionTypes.FinishGrid, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner });
            //Track track1 = new Track("baan", sectionTypes);
            Competition.Tracks.Enqueue(Track);
        }

        public static void NextRace()
        {
            Track dequeuetrack = Competition.NextTrack();
            if (dequeuetrack != null)
            {
                CurrentRace = new Race(dequeuetrack, Competition.Participants);            
            }
        }
        //public static void RaceEnded(Object source, EventArgs e)
        //{

        //    Debug.WriteLine("Data.RaceEnded: next trackname: " + CurrentRace.Track.Name);
        //    Competition.AddScores(CurrentRace.GetDriversWithScore());
        //    Competition.AddTimes(CurrentRace.GetDriversWithTime());

        //    NextRace();

        //    string bestScore = Competition.GetBestParticipantScore();
        //    string bestTime = Competition.GetBestParticipantTime();

        //    UpdateNextRace?.Invoke(null, new DriversChangedEventArgs() { Track = CurrentRace.Track, BestScore = bestScore, BestTime = bestTime });
        //}
    }
}
