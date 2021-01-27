﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Competition
    {
        public List<IParticipant> Participants { get; set; }
        public Queue<Track> Tracks { get; set; }
        public Competition()
        {
            Participants = new List<IParticipant>();
            Tracks = new Queue<Track>();
        }


        public Track NextTrack()
        {
            if (Tracks.Count == 0)
            {
                return null;
            }
            else
            {
                return Tracks.Dequeue();
            }
        }
        public void AddParticipant(IParticipant participant)
        {
            Participants.Add(participant);
        }

        public void AddTrack(Track track)
        {
            Tracks.Enqueue(track);
        }

    }
}
