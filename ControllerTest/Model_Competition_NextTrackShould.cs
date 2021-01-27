using Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static Model.Section;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {
        private Competition _competition;
        public Track Track2 { get; set; }
        public Track Track3 { get; set; }
        public SectionTypes[] Sectiontypes2 { get; set; }
        public SectionTypes[] Sectiontypes3 { get; set; }
        [SetUp]
        public void SetUp()
        {
            _competition = new Competition();
            Sectiontypes2 = new SectionTypes[] { SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.FinishGrid };
            Track2 = new Track("baan2", Sectiontypes2);
            Sectiontypes3 = new SectionTypes[] { SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.FinishGrid };
            Track Track3 = new Track("baan3", Sectiontypes3);
        }

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()

        {
            Track result;
            result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {

            _competition.Tracks.Enqueue(Track2);
            Track result = _competition.NextTrack();
            Assert.AreEqual(Track2, result);
        }
        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {

            _competition.Tracks.Enqueue(Track2);
            Track result = _competition.NextTrack();
            result = _competition.NextTrack();
            Assert.IsTrue(result == null);
        }
        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {

            _competition.Tracks.Enqueue(Track2);
            _competition.Tracks.Enqueue(Track3);
            _competition.NextTrack();

        }



    }
}
